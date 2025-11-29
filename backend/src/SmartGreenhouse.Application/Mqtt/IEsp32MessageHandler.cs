namespace SmartGreenhouse.Application.Mqtt;

public interface IEsp32MessageHandler
{
    Task HandleAsync(string topic, string payload, CancellationToken ct = default);
}
