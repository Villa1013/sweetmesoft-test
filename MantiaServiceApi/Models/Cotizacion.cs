namespace MantiaServiceAPI.Models;

public enum EstadoCotizacion
{
    Borrador,
    Enviada,
    Aceptada,
    Rechazada
}

public class Cotizacion
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime ValidaHasta { get; set; }
    public int? SolicitudId { get; set; }
    public Solicitud? Solicitud { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string VersionTarifa { get; set; } = string.Empty;
    public EstadoCotizacion Estado { get; set; } = EstadoCotizacion.Borrador;
    public decimal Subtotal { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public decimal PorcentajeIVA { get; set; } = 19m;
    public string Observaciones { get; set; } = string.Empty;
    public bool EsHistorica { get; set; }

    public ICollection<LineaCotizacion> Lineas { get; set; } = new List<LineaCotizacion>();
}
