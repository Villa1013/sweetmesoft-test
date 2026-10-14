using MantiaServiceAPI.DTOs;
using MantiaServiceAPI.Services.Cotizacion;
using Microsoft.AspNetCore.Mvc;

namespace MantiaServiceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CotizacionesController : ControllerBase
{
    private readonly CotizacionService _service;

    public CotizacionesController(CotizacionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<CotizacionListadoDto>>> Listar([FromQuery] int? solicitudId, CancellationToken ct)
        => Ok(await _service.ListarAsync(solicitudId, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CotizacionDetalleDto>> Obtener(int id, CancellationToken ct)
    {
        var cotizacion = await _service.ObtenerAsync(id, ct);
        return cotizacion is null ? NotFound() : Ok(cotizacion);
    }

    [HttpPost("solicitudes/{solicitudId:int}")]
    public async Task<ActionResult<CotizacionDetalleDto>> Generar(int solicitudId, CancellationToken ct)
    {
        var (cotizacion, errores) = await _service.GenerarAsync(solicitudId, ct);
        if (errores is not null)
            return cotizacion is null && errores.Contains("No existe la solicitud.")
                ? NotFound(new ErrorCalculoDto(errores))
                : UnprocessableEntity(new ErrorCalculoDto(errores));

        return CreatedAtAction(nameof(Obtener), new { id = cotizacion!.Id }, cotizacion);
    }

    [HttpPost("{id:int}/marcar-enviada")]
    public async Task<ActionResult<CotizacionDetalleDto>> MarcarEnviada(int id, CancellationToken ct)
    {
        var cotizacion = await _service.MarcarEnviadaAsync(id, ct);
        return cotizacion is null ? NotFound() : Ok(cotizacion);
    }
}
