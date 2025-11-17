using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Api.Contracts;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _db;
    public DevicesController(AppDbContext db) => _db = db;

    /// <summary>
    /// Gets all devices in the system.
    /// </summary>
    /// <returns>List of devices</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var devices = await _db.Devices
            .AsNoTracking()
            .Select(d => new DeviceDto(d.Id, d.DeviceName, d.DeviceType, d.CreatedAt))
            .ToListAsync();

        return Ok(devices);
    }

    /// <summary>
    /// Creates a new device in the system.
    /// </summary>
    /// <param name="device">The device to create</param>
    /// <returns>The created device</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Device device)
    {
        // Ensure CreatedAt is set to current UTC time
        device.CreatedAt = DateTime.UtcNow;

        _db.Devices.Add(device);
        await _db.SaveChangesAsync();

        var dto = new DeviceDto(device.Id, device.DeviceName, device.DeviceType, device.CreatedAt);
        return Ok(dto);
    }
    /// <summary>
    /// Deletes a device by ID.
    /// </summary>
    /// <param name="id">The device ID to delete</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var device = await _db.Devices.FindAsync(id);
        if (device == null)
        {
            return NotFound($"Device with ID {id} not found");
        }

        _db.Devices.Remove(device);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}