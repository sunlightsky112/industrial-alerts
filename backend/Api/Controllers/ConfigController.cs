using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("config")]
[Authorize] // requires JWT
public class ConfigController : ControllerBase
{
    private readonly AppDbContext _db;
    public ConfigController(AppDbContext db) => _db = db;

    // GET /config
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cfg = await _db.Configs.FirstOrDefaultAsync(c => c.Id == 1);
        if (cfg is null) return NotFound();
        return Ok(cfg);
    }

    public record UpdateConfigDto(decimal TempMax, decimal HumidityMax);

    // PUT /config
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UpdateConfigDto dto)
    {
        if (dto.TempMax <= 0 || dto.HumidityMax <= 0)
            return BadRequest("Thresholds must be positive.");

        var cfg = await _db.Configs.FirstOrDefaultAsync(c => c.Id == 1);
        if (cfg is null) return NotFound();

        cfg.TempMax = dto.TempMax;
        cfg.HumidityMax = dto.HumidityMax;
        cfg.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(cfg);
    }
}
