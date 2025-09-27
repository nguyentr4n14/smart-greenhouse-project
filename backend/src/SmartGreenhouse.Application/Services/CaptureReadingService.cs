using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Application.DeviceIntegration;
using SmartGreenhouse.Application.Factories;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Domain.Enums;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Application.Services;

/// <summary>
/// Service for capturing sensor readings from devices.
/// This orchestrates the workflow of reading from devices, normalizing values, and storing results.
/// </summary>
public class CaptureReadingService
{
    private readonly AppDbContext _db;
    private readonly IDeviceFactoryResolver _factoryResolver;

    public CaptureReadingService(AppDbContext db, IDeviceFactoryResolver factoryResolver)
    {
        _db = db;
        _factoryResolver = factoryResolver;
    }

    /// <summary>
    /// Captures a sensor reading from the specified device.
    /// </summary>
    /// <param name="deviceId">The device to read from</param>
    /// <param name="sensorType">The type of sensor to read</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The captured and stored sensor reading</returns>
    /// <exception cref="InvalidOperationException">Thrown when device is not found</exception>
    public async Task<SensorReading> CaptureAsync(
        int deviceId, 
        SensorTypeEnum sensorType, 
        CancellationToken ct = default)
    {
        // 1. Load device or throw exception
        var device = await _db.Devices.FirstOrDefaultAsync(d => d.Id == deviceId, ct);
        if (device == null)
        {
            throw new InvalidOperationException($"Device with ID {deviceId} not found.");
        }

        // 2. Resolve factory based on device type
        var factory = _factoryResolver.Resolve(device);
        var sensorReader = factory.CreateSensorReader();

        // 3. Read raw value from device
        var rawValue = await sensorReader.ReadAsync(deviceId, sensorType, ct);

        // 4. Normalize value using appropriate normalizer
        var normalizer = SensorNormalizerFactory.Create(sensorType);
        var normalizedValue = normalizer.Normalize(rawValue);

        // 5. Create and save sensor reading
        var reading = new SensorReading
        {
            DeviceId = deviceId,
            SensorType = sensorType,
            Value = normalizedValue,
            Unit = normalizer.CanonicalUnit,
            Timestamp = DateTime.UtcNow
        };

        _db.Readings.Add(reading);
        await _db.SaveChangesAsync(ct);

        return reading;
    }
}