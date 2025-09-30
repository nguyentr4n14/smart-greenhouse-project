namespace SmartGreenhouse.Application.Abstractions;

/// <summary>
/// Abstract factory for creating device integration components.
/// This follows the Abstract Factory pattern, allowing different implementations
/// for different device types (e.g., simulated, MQTT, USB).
/// </summary>
public interface IDeviceIntegrationFactory
{
    /// <summary>
    /// Creates a sensor reader for this device integration type.
    /// </summary>
    /// <returns>A sensor reader instance</returns>
    ISensorReader CreateSensorReader();

    /// <summary>
    /// Creates an actuator controller for this device integration type.
    /// </summary>
    /// <returns>An actuator controller instance</returns>
    IActuatorController CreateActuatorController();
}