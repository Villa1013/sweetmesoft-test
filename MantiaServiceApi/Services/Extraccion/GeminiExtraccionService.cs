using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MantiaServiceAPI.Models;
using MantiaServiceAPI.Services.Cotizacion;

namespace MantiaServiceAPI.Services.Extraccion;

public class GeminiExtraccionService : IExtraccionTexto
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiExtraccionService> _logger;

    public GeminiExtraccionService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GeminiExtraccionService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public bool TieneApiKey => !string.IsNullOrWhiteSpace(ApiKey);

    private string? ApiKey => _configuration["Gemini:ApiKey"];
    private string Modelo => _configuration["Gemini:Model"] ?? "gemini-flash-lite-latest";

    private static readonly string[] ModelosRespaldo =
    {
        "gemini-flash-lite-latest",
        "gemini-3-flash-preview",
    };

    public async Task<ResultadoExtraccion> ExtraerAsync(string texto, CancellationToken cancellationToken = default)
    {
        if (!TieneApiKey)
            throw new InvalidOperationException("Gemini no está configurado.");

        var prompt = $$"""
            Extrae datos de una solicitud de mantenimiento para Mantia (Bogotá/Chía).
            Devuelve SOLO JSON con esta forma:
            {
              "cliente": "",
              "contacto": "",
              "municipio": "Bogotá|Chía|",
              "direccion": "",
              "emailContacto": "",
              "referencia": "",
              "fechaVisita": "YYYY-MM-DD o null",
              "franjaHoraria": "HH:mm - HH:mm",
              "estado": "ListaParaCotizar|FaltaInformacion|RequiereValoracionTecnica",
              "camposFaltantes": "",
              "observaciones": "",
              "items": [{ "codigoTarifa": "AA-01|AA-02|PI-01|PL-01|EL-01", "concepto": "", "cantidad": 0, "observacion": "" }]
            }
            Reglas:
            - AA-01 preventivo split <= 24000 BTU; AA-02 diagnóstico (no incluye reparación).
            - PI-01 pintura interior m² de pared; PL-01 grifería sencilla; EL-01 punto eléctrico.
            - Ascensor u otro trabajo no listado => RequiereValoracionTecnica y items vacíos.
            - Reparación de aire sin ficha => RequiereValoracionTecnica.
            - Si hay corrección de cantidad, usa la última.
            - "urgente" no es recargo.
            Texto:
            {{texto}}
            """;

        Exception? ultimoError = null;
        foreach (var modelo in ModelosDisponibles())
        {
            try
            {
                var resultado = await ExtraerConModeloAsync(modelo, prompt, cancellationToken);
                if (ultimoError is not null)
                {
                    _logger.LogInformation("Gemini respondió con el modelo de respaldo {Modelo}.", modelo);
                }
                return resultado;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                ultimoError = ex;
                _logger.LogWarning(
                    "Gemini con el modelo {Modelo} falló: {Mensaje}. Se intentará el siguiente modelo.",
                    modelo,
                    ex.Message);
            }
        }

        throw ultimoError ?? new InvalidOperationException("Ningún modelo de Gemini respondió.");
    }

    private IEnumerable<string> ModelosDisponibles()
    {
        yield return Modelo;
        foreach (var respaldo in ModelosRespaldo)
        {
            if (!string.Equals(respaldo, Modelo, StringComparison.OrdinalIgnoreCase))
                yield return respaldo;
        }
    }

    private async Task<ResultadoExtraccion> ExtraerConModeloAsync(
        string modelo,
        string prompt,
        CancellationToken cancellationToken)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelo}:generateContent?key={ApiKey}";
        var body = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new { responseMimeType = "application/json", temperature = 0 }
        };

        using var response = await _httpClient.PostAsJsonAsync(url, body, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var detalle = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Gemini respondió {Status}: {Detalle}", (int)response.StatusCode, detalle);
            throw new HttpRequestException($"Gemini respondió {(int)response.StatusCode}.");
        }

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        var textoModelo = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(textoModelo))
            throw new InvalidOperationException("Gemini devolvió una respuesta vacía.");

        var extraido = JsonSerializer.Deserialize<GeminiPayload>(textoModelo, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("No se pudo interpretar el JSON de Gemini.");

        var fechaVisita = DateTime.TryParse(extraido.FechaVisita, out var fecha) ? fecha : (DateTime?)null;
        var estado = Enum.TryParse<EstadoSolicitud>(extraido.Estado, true, out var parsed)
            ? parsed
            : EstadoSolicitud.FaltaInformacion;

        return new ResultadoExtraccion
        {
            Motor = "gemini",
            Cliente = extraido.Cliente ?? string.Empty,
            Contacto = extraido.Contacto ?? string.Empty,
            Municipio = ReglasComerciales.NormalizarMunicipio(extraido.Municipio),
            Direccion = extraido.Direccion ?? string.Empty,
            EmailContacto = extraido.EmailContacto ?? string.Empty,
            Referencia = extraido.Referencia ?? string.Empty,
            FechaVisita = fechaVisita,
            FranjaHoraria = extraido.FranjaHoraria ?? string.Empty,
            FinDeSemana = ReglasComerciales.EsFinDeSemana(fechaVisita),
            FueraDeHorario = ReglasComerciales.EsFueraDeHorario(extraido.FranjaHoraria),
            Estado = estado,
            CamposFaltantes = extraido.CamposFaltantes ?? string.Empty,
            Observaciones = extraido.Observaciones ?? string.Empty,
            Items = extraido.Items.Select(i => new ItemExtraido
            {
                CodigoTarifa = i.CodigoTarifa ?? string.Empty,
                Concepto = i.Concepto ?? string.Empty,
                Cantidad = i.Cantidad,
                Observacion = i.Observacion ?? string.Empty
            }).ToList()
        };
    }

    private sealed class GeminiPayload
    {
        public string? Cliente { get; set; }
        public string? Contacto { get; set; }
        public string? Municipio { get; set; }
        public string? Direccion { get; set; }
        public string? EmailContacto { get; set; }
        public string? Referencia { get; set; }
        public string? FechaVisita { get; set; }
        public string? FranjaHoraria { get; set; }
        public string? Estado { get; set; }
        public string? CamposFaltantes { get; set; }
        public string? Observaciones { get; set; }
        public List<GeminiItem> Items { get; set; } = [];
    }

    private sealed class GeminiItem
    {
        [JsonPropertyName("codigoTarifa")]
        public string? CodigoTarifa { get; set; }
        public string? Concepto { get; set; }
        public decimal Cantidad { get; set; }
        public string? Observacion { get; set; }
    }
}
