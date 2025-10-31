using System.Text.Json;

namespace SmartGreenhouse.Application.Control;

public class HysteresisCoolingStrategy : IControlStrategy
{
    public Task<IEnumerable<ActuatorCommand>> EvaluateAsync(ControlContext context)
    {
        var commands = new List<ActuatorCommand>();

        if (!context.LatestReadings.TryGetValue("Temperature", out var temperature))
        {
            return Task.FromResult<IEnumerable<ActuatorCommand>>(commands);
        }

        // Default thresholds
        double onAbove = 26.0;
        double offBelow = 24.0;

        // Override with custom parameters if provided
        if (context.Parameters != null)
        {
            if (context.Parameters.TryGetValue("onAbove", out var onAboveParam))
            {
                onAbove = ConvertToDouble(onAboveParam);
            }
            if (context.Parameters.TryGetValue("offBelow", out var offBelowParam))
            {
                offBelow = ConvertToDouble(offBelowParam);
            }
        }

        if (temperature >= onAbove)
        {
            commands.Add(new ActuatorCommand
            {
                ActuatorName = "Fan",
                Action = "On"
            });
        }
        else if (temperature <= offBelow)
        {
            commands.Add(new ActuatorCommand
            {
                ActuatorName = "Fan",
                Action = "Off"
            });
        }
        // No action in between (hysteresis zone)

        return Task.FromResult<IEnumerable<ActuatorCommand>>(commands);
    }

    private double ConvertToDouble(object value)
    {
        if (value is JsonElement jsonElement)
        {
            return jsonElement.GetDouble();
        }
        return Convert.ToDouble(value);
    }
}