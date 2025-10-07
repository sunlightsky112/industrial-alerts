using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Api.Controllers;

[ApiController]
[Route("alerts")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AlertsController(AppDbContext db) => _db = db;

    // GET /alerts?status=open|ack&from=...&to=...
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? status,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to)
    {
        var query = _db.Alerts.AsQueryable();

        // status filter: open|ack
        if (!string.IsNullOrWhiteSpace(status))
        {
            AlertStatus? desired = status.Trim().ToLowerInvariant() switch
            {
                "open" => AlertStatus.Open,
                "ack" => AlertStatus.Acknowledged,
                _ => null
            };

            if (desired is null)
                return BadRequest("Invalid status. Use 'open' or 'ack'.");

            query = query.Where(a => a.Status == desired);
        }

        // date range filter
        if (from.HasValue && to.HasValue && from > to)
            return BadRequest("'from' must be earlier than 'to'.");

        if (from.HasValue)
            query = query.Where(a => a.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(a => a.CreatedAt <= to.Value);

        var alerts = await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return Ok(alerts);
    }

    // POST /alerts/{id}/ack
    [HttpPost("{id}/ack")]
    public async Task<IActionResult> Acknowledge(Guid id)
    {
        var alert = await _db.Alerts.FirstOrDefaultAsync(a => a.Id == id);
        if (alert is null) return NotFound();

        if (alert.Status == AlertStatus.Acknowledged)
            return BadRequest("Alert already acknowledged.");

        alert.Status = AlertStatus.Acknowledged;
        await _db.SaveChangesAsync();

        return Ok(alert);
    }
}
