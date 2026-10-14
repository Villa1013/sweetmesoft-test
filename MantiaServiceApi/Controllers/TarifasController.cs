using MantiaServiceAPI.Data;
using MantiaServiceAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MantiaServiceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarifasController : ControllerBase
{
    private readonly AppDbContext _db;

    public TarifasController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<TarifaDto>>> Listar([FromQuery] string? version, CancellationToken ct)
    {
        var query = _db.Tarifas.AsNoTracking().Where(t => t.Activa);
        if (!string.IsNullOrWhiteSpace(version))
            query = query.Where(t => t.Version == version);

        var tarifas = await query
            .OrderBy(t => t.Version)
            .ThenBy(t => t.Codigo)
            .Select(t => new TarifaDto(t.Version, t.Codigo, t.Concepto, t.Unidad, t.PrecioUnitarioCOP, t.Tipo))
            .ToListAsync(ct);

        return Ok(tarifas);
    }
}
