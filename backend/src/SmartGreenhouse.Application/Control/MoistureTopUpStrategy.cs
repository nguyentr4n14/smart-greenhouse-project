namespace SmartGreenhouse.Application.Control;

public class MoistureTopUpStrategy : IControlStrategy
{
    public Task<IEnumerable<ActuatorCommand>> EvaluateAsync(ControlContext context)
    {
        var commands = new List<ActuatorCommand>();

        if (!context.LatestReadings.TryGetValue("SoilMoisture", out var moisture))
        {
            return Task.FromResult<IEnumerable<ActuatorCommand>>(commands);
        }

        // Default threshold
        double threshold = 30.0;

        // Override with custom parameters if provided
        if (context.Parameters != null &&
            context.Parameters.TryGetValue("threshold", out var thresholdParam))
        {
            threshold = Convert.ToDouble(thresholdParam);
        }

        if (moisture < threshold)
        {
            commands.Add(new ActuatorCommand
            {
                ActuatorName = "Pump",
                Action = "On"
            });
        }
        else
        {
            commands.Add(new ActuatorCommand
            {
                ActuatorName = "Pump",
                Action = "Off"
            });
        }

        return Task.FromResult<IEnumerable<ActuatorCommand>>(commands);
    }
}