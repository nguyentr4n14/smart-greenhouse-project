namespace SmartGreenhouse.Application.DeviceIntegration;

/// <summary>
/// Simple factory interface for Assignment 3 compatibility
/// </summary>
public interface IDeviceFactory
{
    ISimpleSensorReader CreateDevice(string deviceType);
}