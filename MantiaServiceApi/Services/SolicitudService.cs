using MantiaServiceAPI.Data;
using MantiaServiceAPI.DTOs;
using MantiaServiceAPI.Models;
using MantiaServiceAPI.Services.Cotizacion;
using MantiaServiceAPI.Services.Extraccion;
using Microsoft.EntityFrameworkCore;

namespace MantiaServiceAPI.Services;

public class SolicitudService
{
    private readonly AppDbContext _db;
    private readonly IExtraccionService _extraccion;

    public SolicitudService(AppDbContext db, IExtraccionService extraccion)
    {
        _db = db;
        _extraccion = extraccion;
    }

    public async Task<List<SolicitudListadoDto>> ListarAsync(EstadoSolicitud? estado, CancellationToken ct = default)
    {
        var query = _db.Solicitudes.AsNoTracking().AsQueryable();
        if (estado.HasValue)
            query = query.Where(s => s.Estado == estado.Value);

        var solicitudes = await query.OrderBy(s => s.FechaRecepcion).ToListAsync(ct);
        return solicitudes.Select(Mapeos.AListado).ToList();
    }

    public async Task<SolicitudDetalleDto?> ObtenerAsync(int id, CancellationToken ct = default)
    {
        var solicitud = await _db.Solicitudes
            .AsNoTracking()
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
        return solicitud is null ? null : Mapeos.ADetalle(solicitud);
    }

    public async Task<ResultadoExtraccionDto> AnalizarAsync(AnalizarTextoRequest request, CancellationToken ct = default)
    {
        var resultado = await _extraccion.ExtraerAsync(request.Texto, ct);
        return Mapeos.ADto(resultado);
    }

    public async Task<SolicitudDetalleDto> RegistrarDesdeTextoAsync(AnalizarTextoRequest request, CancellationToken ct = default)
    {
        var extraido = await _extraccion.ExtraerAsync(request.Texto, ct);
        var consecutivo = await _db.Solicitudes.CountAsync(ct) + 1;
        var solicitud = new Solicitud
        {
            Numero = $"SOL-{consecutivo:000}",
            FechaRecepcion = DateTime.Now,
            Cliente = extraido.Cliente,
            Contacto = extraido.Contacto,
            Municipio = extraido.Municipio,
            Direccion = extraido.Direccion,
            EmailContacto = extraido.EmailContacto,
            Origen = request.Origen,
            Referencia = extraido.Referencia,
            TextoOriginal = request.Texto,
            Estado = extraido.Estado,
            CamposFaltantes = extraido.CamposFaltantes,
            Observaciones = extraido.Observaciones,
            FechaVisita = extraido.FechaVisita,
            FranjaHoraria = extraido.FranjaHoraria,
            FinDeSemana = extraido.FinDeSemana,
            FueraDeHorario = extraido.FueraDeHorario,
            Items = extraido.Items.Select(i => new ItemSolicitud
            {
                CodigoTarifa = i.CodigoTarifa,
                Concepto = i.Concepto,
                Cantidad = i.Cantidad,
                Observacion = i.Observacion
            }).ToList()
        };

        _db.Solicitudes.Add(solicitud);
        await _db.SaveChangesAsync(ct);
        return Mapeos.ADetalle(solicitud);
    }

    public async Task<SolicitudDetalleDto?> ActualizarAsync(int id, ActualizarSolicitudRequest request, CancellationToken ct = default)
    {
        var solicitud = await _db.Solicitudes.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id, ct);
        if (solicitud is null)
            return null;

        if (request.Cliente is not null) solicitud.Cliente = request.Cliente;
        if (request.Contacto is not null) solicitud.Contacto = request.Contacto;
        if (request.Municipio is not null) solicitud.Municipio = ReglasComerciales.NormalizarMunicipio(request.Municipio);
        if (request.Direccion is not null) solicitud.Direccion = request.Direccion;
        if (request.EmailContacto is not null) solicitud.EmailContacto = request.EmailContacto;
        if (request.Referencia is not null) solicitud.Referencia = request.Referencia;
        if (request.FechaVisita.HasValue) solicitud.FechaVisita = request.FechaVisita;
        if (request.FranjaHoraria is not null) solicitud.FranjaHoraria = request.FranjaHoraria;
        if (request.Observaciones is not null) solicitud.Observaciones = request.Observaciones;
        if (request.Estado.HasValue) solicitud.Estado = request.Estado.Value;

        solicitud.FinDeSemana = ReglasComerciales.EsFinDeSemana(solicitud.FechaVisita);
        solicitud.FueraDeHorario = ReglasComerciales.EsFueraDeHorario(solicitud.FranjaHoraria);

        if (request.Items is not null)
        {
            _db.ItemsSolicitud.RemoveRange(solicitud.Items);
            solicitud.Items = request.Items.Select(i => new ItemSolicitud
            {
                CodigoTarifa = i.CodigoTarifa,
                Concepto = i.Concepto,
                Cantidad = i.Cantidad,
                Observacion = i.Observacion
            }).ToList();
        }

        if (!request.Estado.HasValue)
        {
            var faltantes = new List<string>();
            if (string.IsNullOrWhiteSpace(solicitud.Municipio)) faltantes.Add("municipio");
            if (string.IsNullOrWhiteSpace(solicitud.Direccion)) faltantes.Add("dirección");
            if (solicitud.Items.Count == 0) faltantes.Add("ítems");
            solicitud.CamposFaltantes = string.Join(", ", faltantes);
            if (solicitud.Estado != EstadoSolicitud.RequiereValoracionTecnica)
            {
                solicitud.Estado = faltantes.Count == 0
                    ? EstadoSolicitud.ListaParaCotizar
                    : EstadoSolicitud.FaltaInformacion;
            }
        }

        await _db.SaveChangesAsync(ct);
        return Mapeos.ADetalle(solicitud);
    }
}
