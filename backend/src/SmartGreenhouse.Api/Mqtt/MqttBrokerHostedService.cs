using MQTTnet;
using MQTTnet.Server;
using System.Text;
using SmartGreenhouse.Application.Mqtt;

namespace SmartGreenhouse.Api.Mqtt;

public class MqttBrokerHostedService : IHostedService
{
    private MqttServer? _server;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MqttBrokerHostedService> _logger;

    public MqttBrokerHostedService(
        IServiceProvider serviceProvider,
        ILogger<MqttBrokerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883)
            .Build();

        _server = new MqttFactory().CreateMqttServer(options);

        _server.InterceptingPublishAsync += async e =>
        {
            var topic = e.ApplicationMessage.Topic ?? "";
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            _logger.LogInformation("MQTT message received - Topic: {Topic}, Payload: {Payload}", topic, payload);

            // Create a scope to resolve scoped services
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEsp32MessageHandler>();
            
            await handler.HandleAsync(topic, payload, ct);
        };

        await _server.StartAsync();
        _logger.LogInformation("MQTT broker started on port 1883");
    }

    public async Task StopAsync(CancellationToken ct)
    {
        if (_server != null)
        {
            await _server.StopAsync();
            _logger.LogInformation("MQTT broker stopped");
        }
    }
}
