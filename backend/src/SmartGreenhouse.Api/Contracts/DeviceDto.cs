using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Api.Contracts;

public record DeviceDto(
    int Id,
    string DeviceName,
    DeviceTypeEnum DeviceType,
    DateTime CreatedAt
);