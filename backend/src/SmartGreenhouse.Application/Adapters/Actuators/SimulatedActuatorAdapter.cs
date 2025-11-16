using Microsoft.Extensions.Logging;
using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Application.Control;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartGreenhouse.Application.Adapters.Actuators
{
    public class SimulatedActuatorAdapter : IActuatorAdapter
    {
        private readonly ILogger<SimulatedActuatorAdapter> _logger;

        public SimulatedActuatorAdapter(ILogger<SimulatedActuatorAdapter> logger)
        {
            _logger = logger;
        }

        public Task ApplyAsync(int deviceId, IReadOnlyList<ActuatorCommand> commands, CancellationToken ct)
        {
      
            var commandString = string.Join(", ", commands.Select(c => $"{c.ActuatorName}: {c.Action}"));

            _logger.LogWarning("[SIMULATION] Device {DeviceId}: Applying commands -> {Commands}",
                deviceId, commandString);

            return Task.CompletedTask;
        }
    }
}
