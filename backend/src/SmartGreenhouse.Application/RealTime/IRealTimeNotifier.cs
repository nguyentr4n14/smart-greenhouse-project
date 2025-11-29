using SmartGreenhouse.Application.Contracts;

namespace SmartGreenhouse.Application.RealTime;

public interface IRealTimeNotifier
{
    Task BroadcastReadingAsync(ReadingDto dto, CancellationToken ct = default);
}
