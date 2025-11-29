using Microsoft.AspNetCore.Mvc;
using SmartGreenhouse.Api.Contracts;
using SmartGreenhouse.Application.Services;
using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingsController : ControllerBase
{
    private readonly ReadingService _readingService;
    private readonly CaptureReadingService _captureService;

    public ReadingsController(
        ReadingService readingService,
        CaptureReadingService captureService)
    {
        _readingService = readingService;
        _captureService = captureService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReadings(
        [FromQuery] int? deviceId = null,
        [FromQuery] SensorTypeEnum? sensorType = null,
        [FromQuery] int take = 100)
    {
        var readings = await _readingService.GetReadingsAsync(deviceId, sensorType, take);
        var dtos = readings.Select(r => new ReadingDto(
            r.Id,
            r.DeviceId,
            r.SensorType,
            r.Value,
            r.Unit,
            r.Timestamp
        )).ToList();
        return Ok(dtos);
    }

    [HttpPost("capture")]
    public async Task<IActionResult> CaptureReading([FromBody] CaptureReadingRequest request)
    {
        try
        {
            var reading = await _captureService.CaptureAsync(request.DeviceId, request.SensorType);
            return Ok(reading);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

public record CaptureReadingRequest(int DeviceId, SensorTypeEnum SensorType);