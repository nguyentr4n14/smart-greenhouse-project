using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Application.DeviceIntegration;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Domain.Enums;
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

    public async Task<SensorReading> CaptureAsync(int deviceId, SensorTypeEnum sensorType)
    {
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null)
        {
            throw new InvalidOperationException($"Device with ID {deviceId} not found");
        }

        var sensorReader = _deviceFactory.CreateDevice(device.DeviceType.ToString());
        var sharedSensorType = ConvertToSharedSensorType(sensorType);
        var value = await sensorReader.ReadSensorAsync(sharedSensorType);

        var reading = new SensorReading
        {
            DeviceId = deviceId,
            SensorType = sensorType,
            Value = value,
            Unit = GetUnitForSensorType(sensorType), // ✅ Add unit
            Timestamp = DateTime.UtcNow
        };

        _context.SensorReadings.Add(reading);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Captured reading: Device={DeviceId}, Sensor={SensorType}, Value={Value} {Unit}",
            deviceId,
            sensorType,
            value,
            reading.Unit);

        await _publisher.PublishAsync(new ReadingEvent(
            reading.DeviceId,
            reading.SensorType.ToString(),
            reading.Value,
            reading.Timestamp
        ));

        return reading;
    }

    private SensorType ConvertToSharedSensorType(SensorTypeEnum sensorTypeEnum)
    {
        return sensorTypeEnum switch
        {
            SensorTypeEnum.Temperature => SensorType.Temperature,
            SensorTypeEnum.Humidity => SensorType.Humidity,
            SensorTypeEnum.Light => SensorType.Light,
            SensorTypeEnum.SoilMoisture => SensorType.SoilMoisture,
            _ => SensorType.Temperature
        };
    }

    // ✅ Add helper method to get unit
    private string GetUnitForSensorType(SensorTypeEnum sensorType)
    {
        return sensorType switch
        {
            SensorTypeEnum.Temperature => "°C",
            SensorTypeEnum.Humidity => "%",
            SensorTypeEnum.Light => "lux",
            SensorTypeEnum.SoilMoisture => "%",
            _ => ""
        };
    }
}