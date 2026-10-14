using MantiaServiceAPI.Models;

namespace MantiaServiceAPI.DTOs;

public record TarifaDto(
    string Version,
    string Codigo,
    string Concepto,
    string Unidad,
    decimal PrecioUnitarioCOP,
    TipoTarifa Tipo);
