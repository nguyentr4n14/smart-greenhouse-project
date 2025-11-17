using SmartGreenhouse.Application.Control;

namespace SmartGreenhouse.Application.State.States;

public class CoolingState : IGreenhouseState
{
    public string StateName => "Cooling";

    public Task<StateTransitionResult> TickAsync(GreenhouseStateContext context, CancellationToken ct = default)
    {
        var result = new StateTransitionResult
        {
            NextStateName = "Cooling",
            Commands = new List<ActuatorCommand>
            {
                new ActuatorCommand { ActuatorName = "Fan", Action = "On" }
            },
            Note = "Cooling in progress"
        };

        // Check if temperature has normalized (≤ 24°C for hysteresis)
        if (context.LatestReadings.TryGetValue("Temperature", out var temp) && temp <= 24.0)
        {
            result.NextStateName = "Idle";
            result.Note = "Temperature normalized, returning to idle";
            result.Commands.Clear();
            result.Commands.Add(new ActuatorCommand
            {
                ActuatorName = "Fan",
                Action = "Off"
            });
        }
        // Check for critical temperature
        else if (context.LatestReadings.TryGetValue("Temperature", out var criticalTemp) && criticalTemp > 35.0)
        {
            result.NextStateName = "Alarm";
            result.Note = "Critical temperature during cooling!";
        }

        return Task.FromResult(result);
    }
}