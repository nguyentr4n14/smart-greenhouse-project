using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Application.Control;

public class ControlStrategySelector
{
    private readonly AppDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public ControlStrategySelector(AppDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public async Task<IControlStrategy> SelectStrategyAsync(int deviceId)
    {
        var profile = await _context.ControlProfiles
            .FirstOrDefaultAsync(p => p.DeviceId == deviceId);

        if (profile == null)
        {
            // Default strategy
            return _serviceProvider.GetRequiredService<HysteresisCoolingStrategy>();
        }

        return profile.StrategyKey switch
        {
            "HysteresisCooling" => _serviceProvider.GetRequiredService<HysteresisCoolingStrategy>(),
            "MoistureTopUp" => _serviceProvider.GetRequiredService<MoistureTopUpStrategy>(),
            _ => _serviceProvider.GetRequiredService<HysteresisCoolingStrategy>()
        };
    }

    public async Task<Dictionary<string, object>?> GetParametersAsync(int deviceId)
    {
        var profile = await _context.ControlProfiles
            .FirstOrDefaultAsync(p => p.DeviceId == deviceId);

        if (profile?.ParametersJson == null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(profile.ParametersJson);
        }
        catch
        {
            return null;
        }
    }
}