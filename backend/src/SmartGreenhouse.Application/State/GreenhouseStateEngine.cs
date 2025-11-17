using Microsoft.Extensions.DependencyInjection;

namespace SmartGreenhouse.Application.State;

public class GreenhouseStateEngine
{
    private readonly IServiceProvider _serviceProvider;

    public GreenhouseStateEngine(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<StateTransitionResult> TickAsync(
        GreenhouseStateContext context,
        CancellationToken ct = default)
    {
        var state = ResolveState(context.CurrentStateName);
        return await state.TickAsync(context, ct);
    }

    private IGreenhouseState ResolveState(string stateName)
    {
        return stateName switch
        {
            "Idle" => _serviceProvider.GetRequiredService<SmartGreenhouse.Application.State.States.IdleState>(),
            "Cooling" => _serviceProvider.GetRequiredService<SmartGreenhouse.Application.State.States.CoolingState>(),
            "Irrigating" => _serviceProvider.GetRequiredService<SmartGreenhouse.Application.State.States.IrrigatingState>(),
            "Alarm" => _serviceProvider.GetRequiredService<SmartGreenhouse.Application.State.States.AlarmState>(),
            _ => _serviceProvider.GetRequiredService<SmartGreenhouse.Application.State.States.IdleState>()
        };
    }
}