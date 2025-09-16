using Microsoft.AspNetCore.Mvc;
using SmartGreenhouse.Application.Services;
using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingsController : ControllerBase
{
    private readonly ReadingService _service;
    public ReadingsController(ReadingService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? deviceId, [FromQuery] string? sensorType)
        => Ok(await _service.QueryAsync(deviceId, sensorType));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SensorReading reading)
        => Ok(await _service.AddAsync(reading));
}