using MantiaServiceAPI.Models;

namespace MantiaServiceAPI.Services.Extraccion;

public class ItemExtraido
{
    public string CodigoTarifa { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Observacion { get; set; } = string.Empty;
}

public class ResultadoExtraccion
{
    public string Motor { get; set; } = "reglas";
    public string Cliente { get; set; } = string.Empty;
    public string Contacto { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string EmailContacto { get; set; } = string.Empty;
    public string Referencia { get; set; } = string.Empty;
    public DateTime? FechaVisita { get; set; }
    public string FranjaHoraria { get; set; } = string.Empty;
    public bool FinDeSemana { get; set; }
    public bool FueraDeHorario { get; set; }
    public EstadoSolicitud Estado { get; set; }
    public string CamposFaltantes { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public List<ItemExtraido> Items { get; set; } = [];
}
