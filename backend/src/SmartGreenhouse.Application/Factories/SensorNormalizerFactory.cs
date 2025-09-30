using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Application.Factories;

/// <summary>
/// Factory for creating sensor normalizers based on sensor type.
/// </summary>
public static class SensorNormalizerFactory
{
    /// <summary>
    /// Creates a normalizer for the specified sensor type.
    /// </summary>
    /// <param name="sensorType">The sensor type</param>
    /// <returns>A normalizer instance</returns>
    public static ISensorNormalizer Create(SensorTypeEnum sensorType)
    {
        return sensorType switch
        {
            SensorTypeEnum.Temperature => new CelsiusNormalizer(),
            SensorTypeEnum.Humidity => new PercentageNormalizer(),
            SensorTypeEnum.Light => new LuxNormalizer(),
            SensorTypeEnum.SoilMoisture => new PercentageNormalizer(),
            _ => throw new ArgumentException($"Unknown sensor type: {sensorType}", nameof(sensorType))
        };
    }
}

/// <summary>
/// Normalizer for temperature values (Celsius).
/// </summary>
internal sealed class CelsiusNormalizer : ISensorNormalizer
{
    public string CanonicalUnit => "°C";

    public double Normalize(double raw)
    {
        // Temperature can be negative, just pass through
        // Could add reasonable bounds checking if needed (e.g., -50°C to 100°C)
        return Math.Round(raw, 2);
    }
}

/// <summary>
/// Normalizer for percentage values (0-100%).
/// </summary>
internal sealed class PercentageNormalizer : ISensorNormalizer
{
    public string CanonicalUnit => "%";

    public double Normalize(double raw)
    {
        // Clamp percentage values between 0 and 100
        var clamped = Math.Max(0, Math.Min(100, raw));
        return Math.Round(clamped, 2);
    }
}

/// <summary>
/// Normalizer for light values (lux).
/// </summary>
internal sealed class LuxNormalizer : ISensorNormalizer
{
    public string CanonicalUnit => "lux";

    public double Normalize(double raw)
    {
        // Light values should be non-negative
        var normalized = Math.Max(0, raw);
        return Math.Round(normalized, 2);
    }
}