using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Api.Contracts;
using SmartGreenhouse.Application.Control;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ControlController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ControlService _controlService;

    public ControlController(AppDbContext context, ControlService controlService)
    {
        _context = context;
        _controlService = controlService;
    }

    [HttpPost("profile")]
    public async Task<ActionResult<ControlProfile>> SetControlProfile([FromBody] SetControlProfileRequest request)
    {
        var device = await _context.Devices.FindAsync(request.DeviceId);
        if (device == null)
        {
            return NotFound($"Device with ID {request.DeviceId} not found");
        }

        var existingProfile = await _context.ControlProfiles
            .FirstOrDefaultAsync(p => p.DeviceId == request.DeviceId);

        string? parametersJson = null;
        if (request.Parameters.HasValue)
        {
            parametersJson = JsonSerializer.Serialize(request.Parameters.Value);
        }

        if (existingProfile != null)
        {
            existingProfile.StrategyKey = request.StrategyKey;
            existingProfile.ParametersJson = parametersJson;
            existingProfile.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            existingProfile = new ControlProfile
            {
                DeviceId = request.DeviceId,
                StrategyKey = request.StrategyKey,
                ParametersJson = parametersJson
            };
            _context.ControlProfiles.Add(existingProfile);
        }

        await _context.SaveChangesAsync();

        return Ok(existingProfile);
    }

    [HttpGet("profile/{deviceId}")]
    public async Task<ActionResult<ControlProfile>> GetControlProfile(int deviceId)
    {
        var profile = await _context.ControlProfiles
            .FirstOrDefaultAsync(p => p.DeviceId == deviceId);

        if (profile == null)
        {
            return NotFound($"No control profile found for device {deviceId}");
        }

        return Ok(profile);
    }

    [HttpPost("evaluate")]
    public async Task<ActionResult<IEnumerable<ActuatorCommand>>> EvaluateControl([FromBody] EvaluateControlRequest request)
    {
        var device = await _context.Devices.FindAsync(request.DeviceId);
        if (device == null)
        {
            return NotFound($"Device with ID {request.DeviceId} not found");
        }

        var commands = await _controlService.EvaluateControlAsync(request.DeviceId);
        return Ok(commands);
    }
}