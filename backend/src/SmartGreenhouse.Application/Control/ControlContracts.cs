namespace SmartGreenhouse.Application.Control;

public interface IControlStrategy
{
    Task<IEnumerable<ActuatorCommand>> EvaluateAsync(ControlContext context);
}

public class ControlContext
{
    public int DeviceId { get; init; }
    public Dictionary<string, double> LatestReadings { get; init; } = new();
    public Dictionary<string, object>? Parameters { get; init; }
}

public class ActuatorCommand
{
    public string ActuatorName { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty; // "On", "Off", "Set:50%"
}