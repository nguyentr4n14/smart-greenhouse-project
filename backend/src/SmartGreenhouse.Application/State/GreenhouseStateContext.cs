namespace SmartGreenhouse.Application.State;

public class GreenhouseStateContext
{
    public int DeviceId { get; set; }
    public Dictionary<string, double> LatestReadings { get; set; } = new();
    public Dictionary<string, object>? Parameters { get; set; }
    public string CurrentStateName { get; set; } = "Idle";
}