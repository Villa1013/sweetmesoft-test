using MantiaServiceAPI.Models;

namespace MantiaServiceAPI.DTOs;

public record ItemSolicitudDto(string CodigoTarifa, string Concepto, decimal Cantidad, string Observacion);

public record SolicitudListadoDto(
    int Id,
    string Numero,
    DateTime FechaRecepcion,
    string Cliente,
    string Municipio,
    OrigenSolicitud Origen,
    EstadoSolicitud Estado,
    string Referencia,
    string CamposFaltantes);

public record SolicitudDetalleDto(
    int Id,
    string Numero,
    DateTime FechaRecepcion,
    string Cliente,
    string Contacto,
    string Municipio,
    string Direccion,
    string EmailContacto,
    OrigenSolicitud Origen,
    string Referencia,
    string TextoOriginal,
    EstadoSolicitud Estado,
    string CamposFaltantes,
    string Observaciones,
    DateTime? FechaVisita,
    string FranjaHoraria,
    bool FinDeSemana,
    bool FueraDeHorario,
    List<ItemSolicitudDto> Items);

public record AnalizarTextoRequest(string Texto, OrigenSolicitud Origen = OrigenSolicitud.Correo);

public record ActualizarSolicitudRequest(
    string? Cliente,
    string? Contacto,
    string? Municipio,
    string? Direccion,
    string? EmailContacto,
    string? Referencia,
    DateTime? FechaVisita,
    string? FranjaHoraria,
    EstadoSolicitud? Estado,
    string? Observaciones,
    List<ItemSolicitudDto>? Items);

public record ResultadoExtraccionDto(
    string Motor,
    string Cliente,
    string Contacto,
    string Municipio,
    string Direccion,
    string EmailContacto,
    string Referencia,
    DateTime? FechaVisita,
    string FranjaHoraria,
    bool FinDeSemana,
    bool FueraDeHorario,
    EstadoSolicitud Estado,
    string CamposFaltantes,
    string Observaciones,
    List<ItemSolicitudDto> Items);
