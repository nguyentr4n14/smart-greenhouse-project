namespace SmartGreenhouse.Domain.Entities;

public class SensorReading
{
    public int Id { get; set; } // PK
    public int DeviceId { get; set; } // FK -> Device.Id

    [JsonIgnore]
    public Device? Device { get; set; } // navigation property

    public string SensorType { get; set; } = string.Empty; // temp|humidity|light|soilMoisture
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;       // °C|%|lux|%
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}