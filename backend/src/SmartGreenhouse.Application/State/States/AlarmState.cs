using SmartGreenhouse.Application.Control;

namespace SmartGreenhouse.Application.State.States;

public class AlarmState : IGreenhouseState
{
    public string StateName => "Alarm";

    public Task<StateTransitionResult> TickAsync(GreenhouseStateContext context, CancellationToken ct = default)
    {
        var result = new StateTransitionResult
        {
            NextStateName = "Alarm",
            Commands = new List<ActuatorCommand>
            {
                // Safe mode: turn everything off
                new ActuatorCommand { ActuatorName = "Fan", Action = "Off" },
                new ActuatorCommand { ActuatorName = "Pump", Action = "Off" }
            },
            Note = "System in alarm state - critical conditions"
        };

        // Check if conditions have stabilized
        bool tempOk = !context.LatestReadings.TryGetValue("Temperature", out var temp) || (temp <= 35.0 && temp >= 15.0);
        bool moistureOk = !context.LatestReadings.TryGetValue("SoilMoisture", out var moisture) || moisture >= 10.0;

        if (tempOk && moistureOk)
        {
            result.NextStateName = "Idle";
            result.Note = "Conditions stabilized, returning to idle";
        }

        return Task.FromResult(result);
    }
}