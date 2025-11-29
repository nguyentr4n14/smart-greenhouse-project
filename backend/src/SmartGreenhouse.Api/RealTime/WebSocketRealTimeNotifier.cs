using SmartGreenhouse.Application.Contracts;
using SmartGreenhouse.Application.RealTime;

namespace SmartGreenhouse.Api.RealTime;

public class WebSocketRealTimeNotifier : IRealTimeNotifier
{
    private readonly LiveReadingHub _hub;

    public WebSocketRealTimeNotifier(LiveReadingHub hub)
    {
        _hub = hub;
    }

    public Task BroadcastReadingAsync(ReadingDto dto, CancellationToken ct = default)
    {
        return _hub.BroadcastAsync(dto, ct);
    }
}
