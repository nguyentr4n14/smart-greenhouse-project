
using SmartGreenhouse.Application.Control;
using SmartGreenhouse.Application.State;

namespace SmartGreenhouse.Application.State.States
{
    public class IrrigatingState : IGreenhouseState
    {
        public string Name => "Irrigating";

        private const decimal MOISTURE_IRRIGATE_OFF_THRESHOLD = 50.0m;
        private const decimal TEMP_ALARM_THRESHOLD = 40.0m;

        public Task<TransitionResult> TickAsync(GreenhouseStateContext context)
        {
            if (context.LatestReadings.Temperature > TEMP_ALARM_THRESHOLD)
            {
                return Task.FromResult(new TransitionResult
                {
                    NextStateName = "Alarm",
                    Note = "Alarm triggered during irrigation. Stopping all actions.",
                    Commands = new List<ActuatorCommand> { new("Pump", "Off") }
                });
            }

            var moisture = context.LatestReadings.SoilMoisture;
            if (moisture >= MOISTURE_IRRIGATE_OFF_THRESHOLD)
            {
                return Task.FromResult(new TransitionResult
                {
                    NextStateName = "Idle",
                    Note = $"Moisture {moisture}% is above target. Returning to Idle.",
                    Commands = new List<ActuatorCommand> { new("Pump", "Off") }
                });
            }

            return Task.FromResult(new TransitionResult
            {
                NextStateName = Name,
                Note = $"Actively irrigating. Current moisture: {moisture}%.",
                Commands = new List<ActuatorCommand> { new("Pump", "On") }
            });
        }
    }
}
