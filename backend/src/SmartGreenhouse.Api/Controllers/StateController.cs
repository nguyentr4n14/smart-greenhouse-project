using Microsoft.AspNetCore.Mvc;
using SmartGreenhouse.Api.Contracts;
using SmartGreenhouse.Application.Services;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StateController : ControllerBase
{
    private readonly StateService _stateService;

    public StateController(StateService stateService)
    {
        _stateService = stateService;
    }

    [HttpPost("tick")]
    public async Task<IActionResult> RunTick([FromBody] RunStateTickRequest request)
    {
        var result = await _stateService.TickAsync(request.DeviceId);
        return Ok(new
        {
            deviceId = request.DeviceId,
            previousState = result.Note?.Contains("from") == true
                ? result.Note.Split("from")[1].Split("to")[0].Trim()
                : "Unknown",
            nextState = result.NextStateName,
            commands = result.Commands,
            note = result.Note
        });
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentState([FromQuery] int deviceId)
    {
        var snapshot = await _stateService.GetCurrentStateAsync(deviceId);
        if (snapshot == null)
        {
            return NotFound(new { message = $"No state found for device {deviceId}" });
        }
        return Ok(snapshot);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetStateHistory([FromQuery] int deviceId, [FromQuery] int limit = 50)
    {
        var history = await _stateService.GetStateHistoryAsync(deviceId, limit);
        return Ok(history);
    }
}