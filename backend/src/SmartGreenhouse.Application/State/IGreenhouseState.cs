using SmartGreenhouse.Application.Control;

namespace SmartGreenhouse.Application.State
{
   
    public record TransitionResult
    {
        // Added 'required' to fix nullable warnings
        public required string NextStateName { get; init; }
        public IReadOnlyList<ActuatorCommand> Commands { get; init; } = Array.Empty<ActuatorCommand>();
        public string? Note { get; init; }
    }

    // The State Pattern interface
    public interface IGreenhouseState
    {
        string Name { get; }
        Task<TransitionResult> TickAsync(GreenhouseStateContext context);
    }
}
