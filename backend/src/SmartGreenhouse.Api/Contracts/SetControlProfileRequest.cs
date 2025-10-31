using System.Text.Json;

namespace SmartGreenhouse.Api.Contracts;

public record SetControlProfileRequest(
    int DeviceId,
    string StrategyKey,
    JsonElement? Parameters = null
);