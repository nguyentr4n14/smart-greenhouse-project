using SmartGreenhouse.Application.Control;

namespace SmartGreenhouse.Application.Abstractions
{
    public interface IActuatorAdapter
    {
        Task ApplyAsync(int deviceId, IReadOnlyList<ActuatorCommand> commands, CancellationToken ct = default);
    }
}
