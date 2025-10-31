using SmartGreenhouse.Application.Abstractions;

namespace SmartGreenhouse.Application.Events;

public class ReadingPublisher : IReadingPublisher
{
    private readonly List<IReadingObserver> _observers = new();

    public void Subscribe(IReadingObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public async Task PublishAsync(IReadingEvent readingEvent)
    {
        foreach (var observer in _observers)
        {
            await observer.OnReadingAsync(readingEvent);
        }
    }
}