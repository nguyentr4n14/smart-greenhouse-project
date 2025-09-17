namespace SmartGreenhouse.Domain.Entities;

public class Device
{
    public int Id { get; set; } // PK
    public string DeviceName { get; set; } = string.Empty; // custom human-readable name
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<SensorReading> Readings { get; set; } = new List<SensorReading>();
}