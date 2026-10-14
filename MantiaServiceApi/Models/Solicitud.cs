namespace MantiaServiceAPI.Models;

public enum EstadoSolicitud
{
    ListaParaCotizar,
    FaltaInformacion,
    RequiereValoracionTecnica
}

public enum OrigenSolicitud
{
    Correo,
    Mensajeria,
    PDF
}

public class Solicitud
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime FechaRecepcion { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Contacto { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string TelefonoContacto { get; set; } = string.Empty;
    public string EmailContacto { get; set; } = string.Empty;
    public OrigenSolicitud Origen { get; set; }
    public string Referencia { get; set; } = string.Empty;
    public string TextoOriginal { get; set; } = string.Empty;
    public EstadoSolicitud Estado { get; set; }
    public string CamposFaltantes { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public DateTime? FechaVisita { get; set; }
    public string FranjaHoraria { get; set; } = string.Empty;
    public bool FinDeSemana { get; set; }
    public bool FueraDeHorario { get; set; }

    public ICollection<ItemSolicitud> Items { get; set; } = new List<ItemSolicitud>();
    public ICollection<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();
}
