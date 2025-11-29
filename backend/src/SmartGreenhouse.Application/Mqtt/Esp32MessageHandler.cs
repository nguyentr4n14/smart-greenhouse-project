using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartGreenhouse.Application.Contracts;
using SmartGreenhouse.Application.RealTime;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Domain.Enums;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Application.Mqtt;

public class Esp32MessageHandler : IEsp32MessageHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly ILogger<Esp32MessageHandler> _logger;
    private readonly IRealTimeNotifier _realTimeNotifier;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Esp32MessageHandler(
        IDbContextFactory<AppDbContext> dbFactory,
        ILogger<Esp32MessageHandler> logger,
        IRealTimeNotifier realTimeNotifier)
    {
        _dbFactory = dbFactory;
        _logger = logger;
        _realTimeNotifier = realTimeNotifier;
    }

    public async Task HandleAsync(string topic, string payload, CancellationToken ct = default)
    {
        try
        {
            // Parse topic: greenhouse/{deviceName}/sensor/{sensorType}
            var parts = topic.Split('/');
            if (parts.Length != 4 || parts[0] != "greenhouse" || parts[2] != "sensor")
            {
                _logger.LogWarning("Invalid topic format: {Topic}", topic);
                return;
            }

            var deviceName = parts[1];
            var sensorTypeString = parts[3];

            // Parse sensor type
            if (!Enum.TryParse<SensorTypeEnum>(sensorTypeString, true, out var sensorType))
            {
                _logger.LogWarning("Unknown sensor type: {SensorType}", sensorTypeString);
                return;
            }

            // Deserialize payload
            var esp32Payload = JsonSerializer.Deserialize<Esp32Payload>(payload, JsonOptions);
            if (esp32Payload == null)
            {
                _logger.LogWarning("Failed to deserialize payload: {Payload}", payload);
                return;
            }

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            // Find or create device
            var device = await db.Devices
                .FirstOrDefaultAsync(d => d.DeviceName == deviceName, ct);

            if (device == null)
            {
                device = new Device
                {
                    DeviceName = deviceName,
                    DeviceType = DeviceTypeEnum.MqttEdge,
                    CreatedAt = DateTime.UtcNow
                };
                db.Devices.Add(device);
                await db.SaveChangesAsync(ct);
                _logger.LogInformation("Created new device: {DeviceName}", deviceName);
            }

            // Create sensor reading
            var reading = new SensorReading
            {
                DeviceId = device.Id,
                SensorType = sensorType,
                Value = esp32Payload.Value,
                Unit = esp32Payload.Unit,
                Timestamp = esp32Payload.Timestamp ?? DateTime.UtcNow
            };

            db.SensorReadings.Add(reading);
            await db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Saved reading: Device={DeviceName}, Sensor={SensorType}, Value={Value} {Unit}",
                deviceName, sensorType, reading.Value, reading.Unit);

            // Map to DTO and broadcast
            var dto = MapToDto(reading, device);
            await _realTimeNotifier.BroadcastReadingAsync(dto, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MQTT message. Topic: {Topic}, Payload: {Payload}", topic, payload);
        }
    }

    private ReadingDto MapToDto(SensorReading reading, Device device)
        => new(
            reading.Id,
            reading.DeviceId,
            device.DeviceName,
            reading.SensorType,
            reading.Value,
            reading.Unit,
            reading.Timestamp
        );

    private sealed class Esp32Payload
    {
        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; } = "";

        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }
    }
}
