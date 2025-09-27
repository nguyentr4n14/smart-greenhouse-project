using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Application.Abstractions;

/// <summary>
/// Interface for reading sensor values from devices.
/// </summary>
public interface ISensorReader
{
    /// <summary>
    /// Reads a sensor value from the specified device.
    /// </summary>
    /// <param name="deviceId">The device identifier</param>
    /// <param name="sensorType">The type of sensor to read</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The raw sensor value</returns>
    Task<double> ReadAsync(int deviceId, SensorTypeEnum sensorType, CancellationToken ct = default);

    /// <summary>
    /// Gets the unit for the specified sensor type.
    /// </summary>
    /// <param name="sensorType">The sensor type</param>
    /// <returns>The unit string (e.g., "°C", "%", "lux")</returns>
    string UnitFor(SensorTypeEnum sensorType);
}