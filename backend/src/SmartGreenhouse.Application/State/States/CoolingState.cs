using SmartGreenhouse.Application.Control;
using SmartGreenhouse.Application.State; // <-- Make sure this is present
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartGreenhouse.Application.State.States
{
    public class CoolingState : IGreenhouseState
    {
        public string Name => "Cooling";

        private const decimal TEMP_COOL_OFF_THRESHOLD = 25.0m;
        private const decimal TEMP_ALARM_THRESHOLD = 40.0m;

        public Task<TransitionResult> TickAsync(GreenhouseStateContext context)
        {
            var temp = context.LatestReadings.Temperature;

            if (temp > TEMP_ALARM_THRESHOLD)
            {
                return Task.FromResult(new TransitionResult
                {
                    NextStateName = "Alarm",
                    Note = $"Temperature {temp}°C exceeded alarm threshold.",
                    Commands = new List<ActuatorCommand> { new("Fan", "Off") }
                });
            }

            if (temp <= TEMP_COOL_OFF_THRESHOLD)
            {
                return Task.FromResult(new TransitionResult
                {
                    NextStateName = "Idle",
                    Note = $"Temperature {temp}°C is below target. Returning to Idle.",
                    Commands = new List<ActuatorCommand> { new("Fan", "Off") }
                });
            }

            return Task.FromResult(new TransitionResult
            {
                NextStateName = Name,
                Note = $"Actively cooling. Current temp: {temp}°C.",
                Commands = new List<ActuatorCommand> { new("Fan", "On") }
            });
        }
    }
}
