using MantiaServiceAPI.Data;
using MantiaServiceAPI.DTOs;
using MantiaServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MantiaServiceAPI.Services.Cotizacion;

public class CotizacionService
{
    private readonly AppDbContext _db;
    private readonly CotizacionCalculator _calculator;

    public CotizacionService(AppDbContext db, CotizacionCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    public async Task<List<CotizacionListadoDto>> ListarAsync(int? solicitudId, CancellationToken ct = default)
    {
        var query = _db.Cotizaciones.AsNoTracking().AsQueryable();
        if (solicitudId.HasValue)
            query = query.Where(c => c.SolicitudId == solicitudId.Value);

        var cotizaciones = await query
            .OrderByDescending(c => c.FechaEmision)
            .ThenBy(c => c.Numero)
            .ToListAsync(ct);
        return cotizaciones.Select(Mapeos.AListado).ToList();
    }

    public async Task<CotizacionDetalleDto?> ObtenerAsync(int id, CancellationToken ct = default)
    {
        var cotizacion = await _db.Cotizaciones
            .AsNoTracking()
            .Include(c => c.Lineas)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        return cotizacion is null ? null : Mapeos.ADetalle(cotizacion);
    }

    public async Task<(CotizacionDetalleDto? Cotizacion, IReadOnlyList<string>? Errores)> GenerarAsync(
        int solicitudId,
        CancellationToken ct = default)
    {
        var solicitud = await _db.Solicitudes
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == solicitudId, ct);

        if (solicitud is null)
            return (null, ["No existe la solicitud."]);

        var tarifas = await _db.Tarifas.AsNoTracking().ToListAsync(ct);
        var fechaEmision = new DateTime(2026, 10, 13);
        var calculo = _calculator.Calcular(solicitud, tarifas, fechaEmision);
        if (!calculo.Ok)
            return (null, calculo.Errores);

        var siguiente = await _db.Cotizaciones.CountAsync(ct) + 1;
        var cotizacion = new Models.Cotizacion
        {
            Numero = $"COT-{siguiente:000}",
            FechaEmision = calculo.FechaEmision,
            ValidaHasta = calculo.ValidaHasta,
            SolicitudId = solicitud.Id,
            Cliente = solicitud.Cliente,
            Municipio = solicitud.Municipio,
            Direccion = solicitud.Direccion,
            VersionTarifa = calculo.VersionTarifa,
            Estado = EstadoCotizacion.Borrador,
            Subtotal = calculo.Subtotal,
            IVA = calculo.IVA,
            Total = calculo.Total,
            PorcentajeIVA = ReglasComerciales.PorcentajeIva,
            Observaciones = "Borrador para revisión de Laura. No se envía al cliente.",
            EsHistorica = false,
            Lineas = calculo.Lineas.Select(l => new LineaCotizacion
            {
                Codigo = l.Codigo,
                Concepto = l.Concepto,
                Cantidad = l.Cantidad,
                PrecioUnitarioCOP = l.PrecioUnitarioCOP,
                ValorLineaCOP = l.ValorLineaCOP
            }).ToList()
        };

        _db.Cotizaciones.Add(cotizacion);
        await _db.SaveChangesAsync(ct);
        return (Mapeos.ADetalle(cotizacion), null);
    }

    public async Task<CotizacionDetalleDto?> MarcarEnviadaAsync(int id, CancellationToken ct = default)
    {
        var cotizacion = await _db.Cotizaciones.Include(c => c.Lineas).FirstOrDefaultAsync(c => c.Id == id, ct);
        if (cotizacion is null)
            return null;

        cotizacion.Estado = EstadoCotizacion.Enviada;
        cotizacion.Observaciones = "Marcada como enviada en el prototipo. No hay envío real al cliente.";
        await _db.SaveChangesAsync(ct);
        return Mapeos.ADetalle(cotizacion);
    }
}
