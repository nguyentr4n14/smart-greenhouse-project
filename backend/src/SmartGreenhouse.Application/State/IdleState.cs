

namespace SmartGreenhouse.Application.State
{
    public class IdleState : IGreenhouseState
    {
        public string Name => "Idle";

        // Hardcoded thresholds for this example
        private const decimal TEMP_COOL_THRESHOLD = 28.0m;
        private const decimal MOISTURE_IRRIGATE_THRESHOLD = 30.0m;
        private const decimal TEMP_ALARM_THRESHOLD = 40.0m;

        public Task<TransitionResult> TickAsync(GreenhouseStateContext context)
        {
            var temp = context.LatestReadings.Temperature;
            var moisture = context.LatestReadings.SoilMoisture;

            if (temp > TEMP_ALARM_THRESHOLD)
            {
                return Task.FromResult(new TransitionResult
                {
                    NextStateName = "Alarm",
                    Note = $"Temperature {temp}°C exceeded alarm threshold {TEMP_ALARM_THRESHOLD}°C."
                });
            }

            if (temp > TEMP_COOL_THRESHOLD)
            {
                return Task.FromResult(new TransitionResult
                {
                    NextStateName = "Cooling",
                    Note = $"Temperature {temp}°C exceeded cooling threshold {TEMP_COOL_THRESHOLD}°C."
                });
            }

            if (moisture < MOISTURE_IRRIGATE_THRESHOLD)
            {
                return Task.FromResult(new TransitionResult
                {
                    NextStateName = "Irrigating",
                    Note = $"Soil moisture {moisture}% below irrigation threshold {MOISTURE_IRRIGATE_THRESHOLD}%."
                });
            }

            return Task.FromResult(new TransitionResult
            {
                NextStateName = Name,
                Note = "All readings are nominal."
            });
        }
    }
}
