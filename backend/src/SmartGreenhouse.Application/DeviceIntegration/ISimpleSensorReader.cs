using SmartGreenhouse.Shared;

namespace SmartGreenhouse.Application.DeviceIntegration;

public interface ISimpleSensorReader
{
    Task<double> ReadSensorAsync(SensorType sensorType);
}