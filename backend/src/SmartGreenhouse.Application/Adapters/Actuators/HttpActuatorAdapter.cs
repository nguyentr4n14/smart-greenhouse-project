using Microsoft.Extensions.Logging;
using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Application.Control;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace SmartGreenhouse.Application.Adapters.Actuators
{
    public class HttpActuatorAdapter : IActuatorAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpActuatorAdapter> _logger;

        public HttpActuatorAdapter(HttpClient httpClient, ILogger<HttpActuatorAdapter> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task ApplyAsync(int deviceId, IReadOnlyList<ActuatorCommand> commands, CancellationToken ct)
        {
            var endpoint = $"/devices/{deviceId}/actuators";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, commands, ct);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Successfully sent commands to {Endpoint} for Device {DeviceId}",
                    endpoint, deviceId);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to apply actuator commands via HTTP for Device {DeviceId}",
                    deviceId);
            }
        }
    }
}
