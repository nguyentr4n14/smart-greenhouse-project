using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlertsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlertNotification>>> GetAlerts(
        [FromQuery] int? deviceId,
        [FromQuery] int take = 100)
    {
        var query = _context.AlertNotifications.AsQueryable();

        if (deviceId.HasValue)
        {
            query = query.Where(a => a.DeviceId == deviceId.Value);
        }

        var alerts = await query
            .OrderByDescending(a => a.TriggeredAt)
            .Take(take)
            .ToListAsync();

        return Ok(alerts);
    }
}