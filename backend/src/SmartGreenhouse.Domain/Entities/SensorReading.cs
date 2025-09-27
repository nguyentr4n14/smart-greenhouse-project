using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Domain.Entities;

public class SensorReading
{
    public int Id { get; set; } // PK
    public int DeviceId { get; set; } // FK -> Device.Id

    [System.Text.Json.Serialization.JsonIgnore]
    public Device? Device { get; set; } // navigation property

    public SensorTypeEnum SensorType { get; set; } // Temperature|Humidity|Light|SoilMoisture
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;       // °C|%|lux|%
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}