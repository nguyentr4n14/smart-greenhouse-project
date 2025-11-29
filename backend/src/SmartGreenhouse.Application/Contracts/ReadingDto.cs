using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Application.Contracts;

public record ReadingDto(
    int Id,
    int DeviceId,
    string DeviceName,
    SensorTypeEnum SensorType,
    double Value,
    string Unit,
    DateTime Timestamp
);
