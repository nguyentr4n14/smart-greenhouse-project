using SmartGreenhouse.Application.Control;

namespace SmartGreenhouse.Application.State.States;

public class IrrigatingState : IGreenhouseState
{
    public string StateName => "Irrigating";

    public Task<StateTransitionResult> TickAsync(GreenhouseStateContext context, CancellationToken ct = default)
    {
        var result = new StateTransitionResult
        {
            NextStateName = "Irrigating",
            Commands = new List<ActuatorCommand>
            {
                new ActuatorCommand { ActuatorName = "Pump", Action = "On" }
            },
            Note = "Irrigation in progress"
        };

        // Check if soil moisture has recovered (≥ 35% for hysteresis)
        if (context.LatestReadings.TryGetValue("SoilMoisture", out var moisture) && moisture >= 35.0)
        {
            result.NextStateName = "Idle";
            result.Note = "Soil moisture recovered, returning to idle";
            result.Commands.Clear();
            result.Commands.Add(new ActuatorCommand
            {
                ActuatorName = "Pump",
                Action = "Off"
            });
        }
        // Check for critical low moisture
        else if (context.LatestReadings.TryGetValue("SoilMoisture", out var criticalMoisture) && criticalMoisture < 10.0)
        {
            result.NextStateName = "Alarm";
            result.Note = "Critical low soil moisture!";
        }

        return Task.FromResult(result);
    }
}