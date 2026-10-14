namespace MantiaServiceAPI.Models;

public enum TipoTarifa
{
    Servicio,
    Recargo,
    Desplazamiento
}

public class Tarifa
{
    public int Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public string Unidad { get; set; } = string.Empty;
    public decimal PrecioUnitarioCOP { get; set; }
    public TipoTarifa Tipo { get; set; }
    public bool Activa { get; set; } = true;
}
