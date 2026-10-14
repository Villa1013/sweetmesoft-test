using MantiaServiceAPI.Models;

namespace MantiaServiceAPI.DTOs;

public record LineaCotizacionDto(
    string Codigo,
    string Concepto,
    decimal Cantidad,
    decimal PrecioUnitarioCOP,
    decimal ValorLineaCOP);

public record CotizacionListadoDto(
    int Id,
    string Numero,
    DateTime FechaEmision,
    string Cliente,
    string Municipio,
    EstadoCotizacion Estado,
    decimal Subtotal,
    decimal IVA,
    decimal Total,
    string VersionTarifa,
    bool EsHistorica,
    int? SolicitudId);

public record CotizacionDetalleDto(
    int Id,
    string Numero,
    DateTime FechaEmision,
    DateTime ValidaHasta,
    int? SolicitudId,
    string Cliente,
    string Municipio,
    string Direccion,
    string VersionTarifa,
    EstadoCotizacion Estado,
    decimal Subtotal,
    decimal IVA,
    decimal Total,
    decimal PorcentajeIVA,
    string Observaciones,
    bool EsHistorica,
    List<LineaCotizacionDto> Lineas);

public record ErrorCalculoDto(IReadOnlyList<string> Errores);
