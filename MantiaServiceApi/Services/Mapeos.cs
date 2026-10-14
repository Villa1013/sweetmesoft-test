using MantiaServiceAPI.DTOs;
using MantiaServiceAPI.Models;
using MantiaServiceAPI.Services.Extraccion;

namespace MantiaServiceAPI.Services;

public static class Mapeos
{
    public static SolicitudListadoDto AListado(Solicitud s) => new(
        s.Id,
        s.Numero,
        s.FechaRecepcion,
        s.Cliente,
        s.Municipio,
        s.Origen,
        s.Estado,
        s.Referencia,
        s.CamposFaltantes);

    public static SolicitudDetalleDto ADetalle(Solicitud s) => new(
        s.Id,
        s.Numero,
        s.FechaRecepcion,
        s.Cliente,
        s.Contacto,
        s.Municipio,
        s.Direccion,
        s.EmailContacto,
        s.Origen,
        s.Referencia,
        s.TextoOriginal,
        s.Estado,
        s.CamposFaltantes,
        s.Observaciones,
        s.FechaVisita,
        s.FranjaHoraria,
        s.FinDeSemana,
        s.FueraDeHorario,
        s.Items.Select(i => new ItemSolicitudDto(i.CodigoTarifa, i.Concepto, i.Cantidad, i.Observacion)).ToList());

    public static ResultadoExtraccionDto ADto(ResultadoExtraccion r) => new(
        r.Motor,
        r.Cliente,
        r.Contacto,
        r.Municipio,
        r.Direccion,
        r.EmailContacto,
        r.Referencia,
        r.FechaVisita,
        r.FranjaHoraria,
        r.FinDeSemana,
        r.FueraDeHorario,
        r.Estado,
        r.CamposFaltantes,
        r.Observaciones,
        r.Items.Select(i => new ItemSolicitudDto(i.CodigoTarifa, i.Concepto, i.Cantidad, i.Observacion)).ToList());

    public static CotizacionListadoDto AListado(Models.Cotizacion c) => new(
        c.Id,
        c.Numero,
        c.FechaEmision,
        c.Cliente,
        c.Municipio,
        c.Estado,
        c.Subtotal,
        c.IVA,
        c.Total,
        c.VersionTarifa,
        c.EsHistorica,
        c.SolicitudId);

    public static CotizacionDetalleDto ADetalle(Models.Cotizacion c) => new(
        c.Id,
        c.Numero,
        c.FechaEmision,
        c.ValidaHasta,
        c.SolicitudId,
        c.Cliente,
        c.Municipio,
        c.Direccion,
        c.VersionTarifa,
        c.Estado,
        c.Subtotal,
        c.IVA,
        c.Total,
        c.PorcentajeIVA,
        c.Observaciones,
        c.EsHistorica,
        c.Lineas.Select(l => new LineaCotizacionDto(l.Codigo, l.Concepto, l.Cantidad, l.PrecioUnitarioCOP, l.ValorLineaCOP)).ToList());
}
