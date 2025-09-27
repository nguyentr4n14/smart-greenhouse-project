namespace SmartGreenhouse.Domain.Enums;

/// <summary>
/// Enum representing different types of sensors in the smart greenhouse system.
/// </summary>
public enum SensorTypeEnum
{
    /// <summary>
    /// Temperature sensor (°C)
    /// </summary>
    Temperature = 0,
    
    /// <summary>
    /// Humidity sensor (%)
    /// </summary>
    Humidity = 1,
    
    /// <summary>
    /// Light sensor (lux)
    /// </summary>
    Light = 2,
    
    /// <summary>
    /// Soil moisture sensor (%)
    /// </summary>
    SoilMoisture = 3
}