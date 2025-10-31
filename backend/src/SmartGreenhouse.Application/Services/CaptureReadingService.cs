using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Application.DeviceIntegration;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;
using SmartGreenhouse.Shared;

namespace SmartGreenhouse.Application.Services;

public class CaptureReadingService
{
    private readonly AppDbContext _context;
    private readonly IDeviceFactory _deviceFactory;
    private readonly IReadingPublisher _publisher;
    private readonly ILogger<CaptureReadingService> _logger;

    public CaptureReadingService(
        AppDbContext context,
        IDeviceFactory deviceFactory,
        IReadingPublisher publisher,
        ILogger<CaptureReadingService> logger)
    {
        _context = context;
        _deviceFactory = deviceFactory;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<SensorReading> CaptureAsync(int deviceId, SensorType sensorType)
    {
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null)
        {
            throw new InvalidOperationException($"Device with ID {deviceId} not found");
        }

        var sensorReader = _deviceFactory.CreateDevice(device.DeviceType);
        var value = await sensorReader.ReadSensorAsync(sensorType);

        var reading = new SensorReading
        {
            DeviceId = deviceId,
            SensorType = sensorType.ToString(),
            Value = value,
            Timestamp = DateTime.UtcNow
        };

        _context.SensorReadings.Add(reading);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Captured reading: Device={DeviceId}, Sensor={SensorType}, Value={Value}",
            deviceId,
            sensorType,
            value);

        // Publish event to observers
        await _publisher.PublishAsync(new ReadingEvent(
            reading.DeviceId,
            reading.SensorType,
            reading.Value,
            reading.Timestamp
        ));

        return reading;
    }
}