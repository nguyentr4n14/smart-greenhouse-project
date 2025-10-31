namespace SmartGreenhouse.Domain.Entities;

public class AlertNotification
{
    public int Id { get; set; }
    public int AlertRuleId { get; set; }
    public int DeviceId { get; set; }
    public string SensorType { get; set; } = string.Empty;
    public double Value { get; set; }
    public double Threshold { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public AlertRule? AlertRule { get; set; }
    public Device? Device { get; set; }
}