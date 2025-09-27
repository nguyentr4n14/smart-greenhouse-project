using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Api.Contracts;

public record ReadingDto(
    int Id,
    int DeviceId,
    SensorTypeEnum SensorType,
    double Value,
    string Unit,
    DateTime Timestamp
);