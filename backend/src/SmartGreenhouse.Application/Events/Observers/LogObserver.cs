using SmartGreenhouse.Application.Abstractions;

namespace SmartGreenhouse.Application.Events.Observers;

public class LogObserver : IReadingObserver
{
    private readonly ILogger<LogObserver> _logger;

    public LogObserver(ILogger<LogObserver> logger)
    {
        _logger = logger;
    }

    public Task OnReadingAsync(IReadingEvent readingEvent)
    {
        _logger.LogInformation(
            "Reading captured: Device={DeviceId}, Sensor={SensorType}, Value={Value}, Time={Timestamp}",
            readingEvent.DeviceId,
            readingEvent.SensorType,
            readingEvent.Value,
            readingEvent.Timestamp);

        return Task.CompletedTask;
    }
}