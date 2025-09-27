namespace SmartGreenhouse.Application.Abstractions;

/// <summary>
/// Interface for normalizing sensor values to canonical units.
/// </summary>
public interface ISensorNormalizer
{
    /// <summary>
    /// Gets the canonical unit for this sensor type.
    /// </summary>
    string CanonicalUnit { get; }

    /// <summary>
    /// Normalizes a raw sensor value to the canonical representation.
    /// </summary>
    /// <param name="raw">The raw sensor value</param>
    /// <returns>The normalized value</returns>
    double Normalize(double raw);
}