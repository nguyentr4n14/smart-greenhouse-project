namespace SmartGreenhouse.Application.Abstractions;

/// <summary>
/// Interface for controlling actuators (devices that can perform actions like turning on/off).
/// </summary>
public interface IActuatorController
{
    /// <summary>
    /// Sets the state of an actuator on the specified device.
    /// </summary>
    /// <param name="deviceId">The device identifier</param>
    /// <param name="actuatorName">The name of the actuator (e.g., "fan", "pump", "heater")</param>
    /// <param name="on">True to turn on, false to turn off</param>
    /// <param name="ct">Cancellation token</param>
    Task SetStateAsync(int deviceId, string actuatorName, bool on, CancellationToken ct = default);
}