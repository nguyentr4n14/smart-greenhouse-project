namespace SmartGreenhouse.Domain.Entities;

public class ControlProfile
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string StrategyKey { get; set; } = string.Empty; // e.g., "HysteresisCooling", "MoistureTopUp"
    public string? ParametersJson { get; set; } // JSON string for strategy-specific parameters
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Device? Device { get; set; }
    public bool IsActive { get; set; }
}