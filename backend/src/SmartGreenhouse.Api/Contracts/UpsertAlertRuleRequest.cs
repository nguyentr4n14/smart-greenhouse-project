namespace SmartGreenhouse.Api.Contracts;

public record UpsertAlertRuleRequest(
    int DeviceId,
    string SensorType,
    string OperatorSymbol,
    double Threshold,
    bool IsActive = true
);