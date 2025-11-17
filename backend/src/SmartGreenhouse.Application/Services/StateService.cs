using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartGreenhouse.Application.Adapters;
using SmartGreenhouse.Application.State;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Application.Services;

public class StateService
{
    private readonly AppDbContext _context;
    private readonly GreenhouseStateEngine _stateEngine;
    private readonly AdapterRegistry _adapterRegistry;
    private readonly ILogger<StateService> _logger;

    public StateService(
        AppDbContext context,
        GreenhouseStateEngine stateEngine,
        AdapterRegistry adapterRegistry,
        ILogger<StateService> logger)
    {
        _context = context;
        _stateEngine = stateEngine;
        _adapterRegistry = adapterRegistry;
        _logger = logger;
    }

    public async Task<StateTransitionResult> TickAsync(int deviceId, CancellationToken ct = default)
    {
        // 1. Get current state from latest snapshot (or default to Idle)
        var latestSnapshot = await _context.DeviceStateSnapshots
            .Where(s => s.DeviceId == deviceId)
            .OrderByDescending(s => s.EnteredAt)
            .FirstOrDefaultAsync(ct);

        var currentStateName = latestSnapshot?.StateName ?? "Idle";

        // 2. Get latest readings for this device
        var latestReadings = await _context.SensorReadings
            .Where(r => r.DeviceId == deviceId)
            .GroupBy(r => r.SensorType)
            .Select(g => g.OrderByDescending(r => r.Timestamp).First())
            .ToListAsync(ct);

        var readingsDict = latestReadings.ToDictionary(
            r => r.SensorType.ToString(),
            r => r.Value
        );

        // 3. Build context and run state machine
        var context = new GreenhouseStateContext
        {
            DeviceId = deviceId,
            LatestReadings = readingsDict,
            CurrentStateName = currentStateName
        };

        var result = await _stateEngine.TickAsync(context, ct);

        // 4. If state changed, persist new snapshot
        if (result.NextStateName != currentStateName)
        {
            var newSnapshot = new DeviceStateSnapshot
            {
                DeviceId = deviceId,
                StateName = result.NextStateName,
                EnteredAt = DateTime.UtcNow,
                Notes = result.Note
            };

            _context.DeviceStateSnapshots.Add(newSnapshot);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Device {DeviceId} transitioned from {OldState} to {NewState}: {Note}",
                deviceId, currentStateName, result.NextStateName, result.Note);
        }

        // 5. Apply actuator commands via adapter
        if (result.Commands.Any())
        {
            var actuatorAdapter = _adapterRegistry.GetActuator();
            // Use CommandsReadOnly property to convert to IReadOnlyList
            await actuatorAdapter.ApplyAsync(deviceId, result.CommandsReadOnly, ct);
        }

        return result;
    }

    public async Task<DeviceStateSnapshot?> GetCurrentStateAsync(int deviceId, CancellationToken ct = default)
    {
        return await _context.DeviceStateSnapshots
            .Where(s => s.DeviceId == deviceId)
            .OrderByDescending(s => s.EnteredAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<DeviceStateSnapshot>> GetStateHistoryAsync(
        int deviceId,
        int limit = 50,
        CancellationToken ct = default)
    {
        return await _context.DeviceStateSnapshots
            .Where(s => s.DeviceId == deviceId)
            .OrderByDescending(s => s.EnteredAt)
            .Take(limit)
            .ToListAsync(ct);
    }
}