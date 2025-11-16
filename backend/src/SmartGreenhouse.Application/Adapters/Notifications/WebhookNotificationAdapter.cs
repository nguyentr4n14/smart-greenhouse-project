using Microsoft.Extensions.Logging;
using SmartGreenhouse.Application.Abstractions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace SmartGreenhouse.Application.Adapters.Notifications
{
    public class WebhookNotificationAdapter : INotificationAdapter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AdapterRegistry _registry;
        private readonly ILogger<WebhookNotificationAdapter> _logger;

        public WebhookNotificationAdapter(IHttpClientFactory httpClientFactory, AdapterRegistry registry, ILogger<WebhookNotificationAdapter> logger)
        {
            _httpClientFactory = httpClientFactory;
            _registry = registry;
            _logger = logger;
        }

        public async Task NotifyAsync(int deviceId, string title, string message, CancellationToken ct)
        {
            var webhookUrl = _registry.WebhookUrl;
            if (string.IsNullOrEmpty(webhookUrl))
            {
                _logger.LogError("WebhookNotificationAdapter: WebhookUrl is not set.");
                return;
            }

            var client = _httpClientFactory.CreateClient("WebhookClient");
            var payload = new { deviceId, title, message };

            try
            {
                var response = await client.PostAsJsonAsync(webhookUrl, payload, ct);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to send webhook notification to {Url}", webhookUrl);
            }
        }
    }
}
