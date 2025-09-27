using SmartGreenhouse.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartGreenhouse.Api.Contracts;

public record CaptureReadingRequest(
    [Range(1, int.MaxValue, ErrorMessage = "DeviceId must be a positive integer")]
    int DeviceId,
    
    SensorTypeEnum SensorType
);