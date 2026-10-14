namespace MantiaServiceAPI.Services.Extraccion;

public class ExtraccionService : IExtraccionService
{
    private readonly GeminiExtraccionService _gemini;
    private readonly ReglasExtraccionService _reglas;
    private readonly ILogger<ExtraccionService> _logger;

    public ExtraccionService(
        GeminiExtraccionService gemini,
        ReglasExtraccionService reglas,
        ILogger<ExtraccionService> logger)
    {
        _gemini = gemini;
        _reglas = reglas;
        _logger = logger;
    }

    public async Task<ResultadoExtraccion> ExtraerAsync(string texto, CancellationToken cancellationToken = default)
    {
        if (_gemini.TieneApiKey)
        {
            try
            {
                var resultado = await _gemini.ExtraerAsync(texto, cancellationToken);
                _logger.LogInformation("Extracción resuelta con Gemini.");
                return resultado;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Gemini falló; se usa extracción por reglas.");
            }
        }
        else
        {
            _logger.LogDebug("Gemini no configurado (falta Gemini:ApiKey); se usa extracción por reglas.");
        }

        return await _reglas.ExtraerAsync(texto, cancellationToken);
    }
}
