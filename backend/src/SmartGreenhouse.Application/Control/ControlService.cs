using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Application.Control;

public class ControlService
{
    private readonly AppDbContext _context;
    private readonly ControlStrategySelector _strategySelector;

    public ControlService(AppDbContext context, ControlStrategySelector strategySelector)
    {
        _context = context;
        _strategySelector = strategySelector;
    }

    public async Task<IEnumerable<ActuatorCommand>> EvaluateControlAsync(int deviceId)
    {
        // Load latest readings for the device
        var latestReadings = await _context.SensorReadings
            .Where(r => r.DeviceId == deviceId)
            .GroupBy(r => r.SensorType)
            .Select(g => g.OrderByDescending(r => r.Timestamp).First())
            .ToListAsync();

        if (latestReadings.Count == 0)
        {
            return Enumerable.Empty<ActuatorCommand>();
        }

        // Convert Dictionary<SensorTypeEnum, double> to Dictionary<string, double>
        var readingsDictionary = latestReadings.ToDictionary(
            r => r.SensorType.ToString(),
            r => r.Value
        );

        // Select strategy and parameters
        var strategy = await _strategySelector.SelectStrategyAsync(deviceId);
        var parameters = await _strategySelector.GetParametersAsync(deviceId);

        // Create context and evaluate
        var context = new ControlContext
        {
            DeviceId = deviceId,
            LatestReadings = readingsDictionary,
            Parameters = parameters
        };

        return await strategy.EvaluateAsync(context);
    }
}