using Microsoft.Extensions.DependencyInjection;
using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Application.Adapters.Actuators;
using SmartGreenhouse.Application.Adapters.Notifications;


namespace SmartGreenhouse.Application.Adapters
{
    public class AdapterRegistry
    {
        private readonly IServiceProvider _serviceProvider;

        // In-memory store for settings
        public string ActuatorMode { get; set; } = "Simulated";
        public string NotificationMode { get; set; } = "Console";
        public string? WebhookUrl { get; set; }

        public AdapterRegistry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IActuatorAdapter GetActuator()
        {
            return ActuatorMode switch
            {
                "Http" => _serviceProvider.GetRequiredService<HttpActuatorAdapter>(),
                _ => _serviceProvider.GetRequiredService<SimulatedActuatorAdapter>()
            };
        }

        public INotificationAdapter GetNotification()
        {
            var mode = (NotificationMode == "Webhook" && !string.IsNullOrEmpty(WebhookUrl))
                ? "Webhook"
                : "Console";

            return mode switch
            {
                "Webhook" => _serviceProvider.GetRequiredService<WebhookNotificationAdapter>(),
                _ => _serviceProvider.GetRequiredService<ConsoleNotificationAdapter>()
            };
        }
    }
}
