using SmartGreenhouse.Application.Control;

namespace SmartGreenhouse.Application.State.States;

public class IdleState : IGreenhouseState
{
    public string StateName => "Idle";

    public Task<StateTransitionResult> TickAsync(GreenhouseStateContext context, CancellationToken ct = default)
    {
        var result = new StateTransitionResult
        {
            NextStateName = "Idle",
            Commands = new List<ActuatorCommand>(),
            Note = "All conditions normal"
        };

        // Check if temperature is too high (> 26°C)
        if (context.LatestReadings.TryGetValue("Temperature", out var temp) && temp > 26.0)
        {
            result.NextStateName = "Cooling";
            result.Note = "Temperature too high, starting cooling";
            result.Commands.Add(new ActuatorCommand
            {
                ActuatorName = "Fan",
                Action = "On"
            });
        }
        // Check if soil moisture is too low (< 30%)
        else if (context.LatestReadings.TryGetValue("SoilMoisture", out var moisture) && moisture < 30.0)
        {
            result.NextStateName = "Irrigating";
            result.Note = "Soil moisture low, starting irrigation";
            result.Commands.Add(new ActuatorCommand
            {
                ActuatorName = "Pump",
                Action = "On"
            });
        }
        // Check for critical conditions (temp > 35 or moisture < 10)
        else if ((context.LatestReadings.TryGetValue("Temperature", out var criticalTemp) && criticalTemp > 35.0) ||
                 (context.LatestReadings.TryGetValue("SoilMoisture", out var criticalMoisture) && criticalMoisture < 10.0))
        {
            result.NextStateName = "Alarm";
            result.Note = "Critical conditions detected!";
        }

        return Task.FromResult(result);
    }
}