using Microsoft.Extensions.Logging;
using SmartGreenhouse.Application.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace SmartGreenhouse.Application.Adapters.Notifications
{
    public class ConsoleNotificationAdapter : INotificationAdapter
    {
        private readonly ILogger<ConsoleNotificationAdapter> _logger;

        public ConsoleNotificationAdapter(ILogger<ConsoleNotificationAdapter> logger)
        {
            _logger = logger;
        }

        public Task NotifyAsync(int deviceId, string title, string message, CancellationToken ct)
        {
            _logger.LogWarning("--- NOTIFICATION (Device {DeviceId}) ---", deviceId);
            _logger.LogWarning("Title: {Title}", title);
            _logger.LogWarning("Message: {Message}", message);
            _logger.LogWarning("-------------------------------------");
            return Task.CompletedTask;
        }
    }
}
