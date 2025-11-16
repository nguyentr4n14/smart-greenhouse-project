using SmartGreenhouse.Application.Control;
using SmartGreenhouse.Application.State;

namespace SmartGreenhouse.Application.State.States
{
    public class AlarmState : IGreenhouseState
    {
        public string Name => "Alarm";

        private const decimal TEMP_ALARM_CLEAR_THRESHOLD = 35.0m;

        public async Task<TransitionResult> TickAsync(GreenhouseStateContext context)
        {
            // 1. Send notification
            await context.NotificationAdapter.NotifyAsync(
                context.DeviceId,
                "GREENHOUSE ALARM",
                $"Device {context.DeviceId} in critical state. Temp: {context.LatestReadings.Temperature}°C."
            );

            // 2. Check if alarm condition is clear
            if (context.LatestReadings.Temperature <= TEMP_ALARM_CLEAR_THRESHOLD)
            {
                return new TransitionResult
                {
                    NextStateName = "Idle",
                    Note = $"Alarm cleared. Temperature {context.LatestReadings.Temperature}°C is safe.",
                    Commands = new List<ActuatorCommand> { new("Fan", "Off"), new("Pump", "Off") }
                };
            }

            // 3. Stay in Alarm, command safe-state
            return new TransitionResult
            {
                NextStateName = Name,
                Note = $"ALARM ACTIVE. Temp: {context.LatestReadings.Temperature}°C. Safe mode enabled.",
                Commands = new List<ActuatorCommand> { new("Fan", "Off"), new("Pump", "Off") }
            };
        }
    }
}
