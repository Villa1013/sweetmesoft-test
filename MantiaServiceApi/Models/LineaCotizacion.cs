namespace MantiaServiceAPI.Models;

public class LineaCotizacion
{
    public int Id { get; set; }
    public int CotizacionId { get; set; }
    public Cotizacion Cotizacion { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitarioCOP { get; set; }
    public decimal ValorLineaCOP { get; set; }
}
