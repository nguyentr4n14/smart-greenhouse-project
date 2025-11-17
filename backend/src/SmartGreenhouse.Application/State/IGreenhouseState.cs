using SmartGreenhouse.Application.Control;

namespace SmartGreenhouse.Application.State;

public interface IGreenhouseState
{
    string StateName { get; }
    Task<StateTransitionResult> TickAsync(GreenhouseStateContext context, CancellationToken ct = default);
}

public class StateTransitionResult
{
    public string NextStateName { get; set; } = string.Empty;
    public List<ActuatorCommand> Commands { get; set; } = new();
    public string? Note { get; set; }

    // Add a helper property to convert to IReadOnlyList
    public IReadOnlyList<ActuatorCommand> CommandsReadOnly => Commands.AsReadOnly();
}