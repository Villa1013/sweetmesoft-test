using MantiaServiceAPI.Models;

namespace MantiaServiceAPI.Services.Cotizacion;

public class LineaCalculada
{
    public string Codigo { get; init; } = string.Empty;
    public string Concepto { get; init; } = string.Empty;
    public decimal Cantidad { get; init; }
    public decimal PrecioUnitarioCOP { get; init; }
    public decimal ValorLineaCOP { get; init; }
    public TipoTarifa Tipo { get; init; }
}

public class ResultadoCalculo
{
    public bool Ok { get; init; }
    public List<string> Errores { get; init; } = [];
    public string VersionTarifa { get; init; } = string.Empty;
    public DateTime FechaEmision { get; init; }
    public DateTime ValidaHasta { get; init; }
    public decimal Subtotal { get; init; }
    public decimal IVA { get; init; }
    public decimal Total { get; init; }
    public List<LineaCalculada> Lineas { get; init; } = [];
}
