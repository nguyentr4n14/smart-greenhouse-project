using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Application.Adapters;
using SmartGreenhouse.Application.Control;
using SmartGreenhouse.Application.State;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartGreenhouse.Application.Services
{
    public class StateService
    {
        private readonly AppDbContext _dbContext;
        private readonly GreenhouseStateEngine _engine;
        private readonly ControlService _controlService; // Assuming you have this
        private readonly AdapterRegistry _adapterRegistry;

        public StateService(
            AppDbContext dbContext,
            GreenhouseStateEngine engine,
            ControlService controlService,
            AdapterRegistry adapterRegistry)
        {
            _dbContext = dbContext;
            _engine = engine;
            _controlService = controlService;
            _adapterRegistry = adapterRegistry;
        }

        public async Task<TransitionResult> TickAsync(int deviceId)
        {
            // 1. Build context
            // You must have a ControlService with GetLatestReadingsAsync
            var readings = await _controlService.EvaluateControlAsync(deviceId);
            var profile = await _dbContext.ControlProfiles
                .FirstOrDefaultAsync(p => p.DeviceId == deviceId && p.IsActive);

            var actuatorAdapter = _adapterRegistry.GetActuator();
            var notificationAdapter = _adapterRegistry.GetNotification();

            var context = new GreenhouseStateContext
            {
                DeviceId = deviceId,
                LatestReadings = (LatestReadingsDto)readings,
                ActiveProfile = profile,
                ActuatorAdapter = actuatorAdapter,
                NotificationAdapter = notificationAdapter
            };

            // 2. Run engine
            var (currentStateName, result) = await _engine.TickAsync(deviceId, context);

            // 3. Persist change
            if (result.NextStateName != currentStateName)
            {
                var snapshot = new DeviceStateSnapshot
                {
                    DeviceId = deviceId,
                    StateName = result.NextStateName,
                    EnteredAt = DateTime.UtcNow,
                    Notes = result.Note
                };
                _dbContext.DeviceStates.Add(snapshot);
                await _dbContext.SaveChangesAsync();
            }

            // 4. Apply commands
            if (result.Commands.Any())
            {
                await actuatorAdapter.ApplyAsync(deviceId, result.Commands);
            }

            return result;
        }
    }
}
