using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using SmartGreenhouse.Application.Contracts;

namespace SmartGreenhouse.Api.RealTime;

public class LiveReadingHub
{
    private readonly List<WebSocket> _clients = new();
    private readonly object _lock = new();

    private static readonly JsonSerializerOptions CamelCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public void Register(WebSocket socket)
    {
        lock (_lock) _clients.Add(socket);
    }

    public void Unregister(WebSocket socket)
    {
        lock (_lock) _clients.Remove(socket);
    }

    public async Task BroadcastAsync(ReadingDto dto, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(dto, CamelCaseOptions);
        var bytes = Encoding.UTF8.GetBytes(json);

        List<WebSocket> snapshot;
        lock (_lock) snapshot = _clients.ToList();

        foreach (var socket in snapshot)
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
                }
                catch
                {
                    Unregister(socket);
                }
            }
            else
            {
                Unregister(socket);
            }
        }
    }
}
