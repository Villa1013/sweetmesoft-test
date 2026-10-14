using MantiaServiceAPI.DTOs;
using MantiaServiceAPI.Models;
using MantiaServiceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantiaServiceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitudesController : ControllerBase
{
    private readonly SolicitudService _service;

    public SolicitudesController(SolicitudService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<SolicitudListadoDto>>> Listar([FromQuery] EstadoSolicitud? estado, CancellationToken ct)
        => Ok(await _service.ListarAsync(estado, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SolicitudDetalleDto>> Obtener(int id, CancellationToken ct)
    {
        var solicitud = await _service.ObtenerAsync(id, ct);
        return solicitud is null ? NotFound() : Ok(solicitud);
    }

    [HttpPost("analizar")]
    public async Task<ActionResult<ResultadoExtraccionDto>> Analizar([FromBody] AnalizarTextoRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
            return BadRequest(new { error = "El texto es obligatorio." });

        return Ok(await _service.AnalizarAsync(request, ct));
    }

    [HttpPost]
    public async Task<ActionResult<SolicitudDetalleDto>> Registrar([FromBody] AnalizarTextoRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
            return BadRequest(new { error = "El texto es obligatorio." });

        var creada = await _service.RegistrarDesdeTextoAsync(request, ct);
        return CreatedAtAction(nameof(Obtener), new { id = creada.Id }, creada);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SolicitudDetalleDto>> Actualizar(int id, [FromBody] ActualizarSolicitudRequest request, CancellationToken ct)
    {
        var actualizada = await _service.ActualizarAsync(id, request, ct);
        return actualizada is null ? NotFound() : Ok(actualizada);
    }
}
