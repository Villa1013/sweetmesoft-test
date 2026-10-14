namespace MantiaServiceAPI.Models;

public class ItemSolicitud
{
    public int Id { get; set; }
    public int SolicitudId { get; set; }
    public Solicitud Solicitud { get; set; } = null!;
    public string CodigoTarifa { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Observacion { get; set; } = string.Empty;
}
