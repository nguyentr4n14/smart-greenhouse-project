using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Domain.Enums;
using SmartGreenhouse.Shared;

namespace SmartGreenhouse.Application.DeviceIntegration;

/// <summary>
/// Factory for creating simulated device components.
/// Used for testing and development without actual hardware.
/// </summary>
public sealed class SimulatedDeviceFactory : IDeviceIntegrationFactory, IDeviceFactory
{
    public ISensorReader CreateSensorReader() => new SimulatedSensorReader();

    public IActuatorController CreateActuatorController() => new SimulatedActuatorController();

    // Implementation for IDeviceFactory
    ISimpleSensorReader IDeviceFactory.CreateDevice(string deviceType) => new SimulatedSensorReader();
}

/// <summary>
/// Simulated sensor reader that generates plausible random values.
/// </summary>
internal sealed class SimulatedSensorReader : ISensorReader, ISimpleSensorReader
{
    private static readonly Random _rng = new();

    // Implementation for Assignment 2 interface (using SensorTypeEnum)
    public Task<double> ReadAsync(
        int deviceId,
        SensorTypeEnum sensorType,
        CancellationToken ct = default
    )
    {
        // Generate realistic random values for each sensor type
        var value = sensorType switch
        {
            SensorTypeEnum.Temperature => 18 + _rng.NextDouble() * 10,     // 18-28°C
            SensorTypeEnum.Humidity => 40 + _rng.NextDouble() * 40,       // 40-80%
            SensorTypeEnum.Light => 200 + _rng.NextDouble() * 800,        // 200-1000 lux
            SensorTypeEnum.SoilMoisture => 20 + _rng.NextDouble() * 60,   // 20-80%
            _ => 0,
        };

        // Add small delay to simulate sensor reading time
        return Task.Delay(50, ct).ContinueWith(_ => value, ct);
    }

    // Implementation for Assignment 3 interface (using SensorType)
    public Task<double> ReadSensorAsync(SensorType sensorType)
    {
        // Map SensorType to SensorTypeEnum and use existing logic
        var sensorTypeEnum = sensorType switch
        {
            SensorType.Temperature => SensorTypeEnum.Temperature,
            SensorType.Humidity => SensorTypeEnum.Humidity,
            SensorType.Light => SensorTypeEnum.Light,
            SensorType.SoilMoisture => SensorTypeEnum.SoilMoisture,
            _ => SensorTypeEnum.Temperature
        };

        return ReadAsync(0, sensorTypeEnum);
    }

    public string UnitFor(SensorTypeEnum sensorType) =>
        sensorType switch
        {
            SensorTypeEnum.Temperature => "°C",
            SensorTypeEnum.Humidity => "%",
            SensorTypeEnum.Light => "lux",
            SensorTypeEnum.SoilMoisture => "%",
            _ => "",
        };
}

/// <summary>
/// Simulated actuator controller that logs actions to console.
/// </summary>
internal sealed class SimulatedActuatorController : IActuatorController
{
    public Task SetStateAsync(
        int deviceId,
        string actuatorName,
        bool on,
        CancellationToken ct = default
    )
    {
        // Log the action (in production this would communicate with actual hardware)
        Console.WriteLine($"[SIM] Device {deviceId}: {actuatorName} => {(on ? "ON" : "OFF")}");

        // Add small delay to simulate actuator response time
        return Task.Delay(100, ct);
    }
}