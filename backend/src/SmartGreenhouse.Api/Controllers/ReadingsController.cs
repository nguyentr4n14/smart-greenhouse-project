using Microsoft.AspNetCore.Mvc;
using SmartGreenhouse.Api.Contracts;
using SmartGreenhouse.Application.Services;
using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingsController : ControllerBase
{
    private readonly CaptureReadingService _captureService;
    private readonly ReadingService _readingService;

    public ReadingsController(CaptureReadingService captureService, ReadingService readingService)
    {
        _captureService = captureService;
        _readingService = readingService;
    }

    /// <summary>
    /// Gets sensor readings with optional filtering by device ID and sensor type.
    /// </summary>
    /// <param name="deviceId">Optional device ID to filter by</param>
    /// <param name="sensorType">Optional sensor type to filter by (Temperature, Humidity, Light, SoilMoisture)</param>
    /// <param name="take">Number of readings to return (default: 200, max: 1000)</param>
    /// <returns>List of sensor readings</returns>
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int? deviceId, 
        [FromQuery] SensorTypeEnum? sensorType,
        [FromQuery] int take = 200)
    {
        // Limit the number of readings to prevent excessive data transfer
        take = Math.Min(take, 1000);
        
        var readings = await _readingService.QueryAsync(deviceId, sensorType, take);
        var dtos = readings.Select(r => new ReadingDto(
            r.Id, 
            r.DeviceId, 
            r.SensorType, 
            r.Value, 
            r.Unit, 
            r.Timestamp
        ));
        
        return Ok(dtos);
    }

    /// <summary>
    /// Captures a new sensor reading from the specified device.
    /// This actively queries the device for a fresh measurement.
    /// </summary>
    /// <param name="request">The capture reading request</param>
    /// <returns>The captured sensor reading</returns>
    [HttpPost("capture")]
    public async Task<IActionResult> Capture([FromBody] CaptureReadingRequest request)
    {
        try
        {
            var reading = await _captureService.CaptureAsync(request.DeviceId, request.SensorType);
            var dto = new ReadingDto(
                reading.Id,
                reading.DeviceId,
                reading.SensorType,
                reading.Value,
                reading.Unit,
                reading.Timestamp
            );
            
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}