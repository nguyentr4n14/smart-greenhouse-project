using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Api.Contracts;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertRulesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlertRulesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlertRule>>> GetAlertRules([FromQuery] int? deviceId)
    {
        var query = _context.AlertRules.AsQueryable();

        if (deviceId.HasValue)
        {
            query = query.Where(r => r.DeviceId == deviceId.Value);
        }

        var rules = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return Ok(rules);
    }

    [HttpPost]
    public async Task<ActionResult<AlertRule>> CreateAlertRule([FromBody] UpsertAlertRuleRequest request)
    {
        var device = await _context.Devices.FindAsync(request.DeviceId);
        if (device == null)
        {
            return NotFound($"Device with ID {request.DeviceId} not found");
        }

        var rule = new AlertRule
        {
            DeviceId = request.DeviceId,
            SensorType = request.SensorType,
            OperatorSymbol = request.OperatorSymbol,
            Threshold = request.Threshold,
            IsActive = request.IsActive
        };

        _context.AlertRules.Add(rule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAlertRules), new { deviceId = rule.DeviceId }, rule);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlertRule(int id)
    {
        var rule = await _context.AlertRules.FindAsync(id);
        if (rule == null)
        {
            return NotFound();
        }

        _context.AlertRules.Remove(rule);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}