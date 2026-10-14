namespace MantiaServiceAPI.Services.Extraccion;

public interface IExtraccionTexto
{
    Task<ResultadoExtraccion> ExtraerAsync(string texto, CancellationToken cancellationToken = default);
}

public interface IExtraccionService : IExtraccionTexto;
