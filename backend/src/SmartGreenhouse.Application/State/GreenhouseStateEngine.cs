using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SmartGreenhouse.Application.Control;
using SmartGreenhouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartGreenhouse.Application.State
{
    public class GreenhouseStateEngine
    {
        private readonly AppDbContext _dbContext;
        private readonly ControlService _controlService; // Assuming you have this
        private readonly IServiceProvider _serviceProvider;

        public GreenhouseStateEngine(
            AppDbContext dbContext,
            ControlService controlService,
            IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _controlService = controlService;
            _serviceProvider = serviceProvider;
        }

        public async Task<(string CurrentStateName, TransitionResult Result)> TickAsync(int deviceId, GreenhouseStateContext context)
        {
            var lastSnapshot = await _dbContext.DeviceStates
                .Where(s => s.DeviceId == deviceId)
                .OrderByDescending(s => s.EnteredAt)
                .FirstOrDefaultAsync();

            string currentStateName = lastSnapshot?.StateName ?? "Idle";

            var currentState = GetState(currentStateName);

            var result = await currentState.TickAsync(context);

            return (currentStateName, result);
        }

        private IGreenhouseState GetState(string stateName)
        {
            // Use DI to resolve the state classes
            return stateName switch
            {
                "Cooling" => _serviceProvider.GetRequiredService<State.States.CoolingState>(),
                "Irrigating" => _serviceProvider.GetRequiredService<State.States.IrrigatingState>(),
                "Alarm" => _serviceProvider.GetRequiredService<State.States.AlarmState>(),
                _ => _serviceProvider.GetRequiredService<State.States.IdleState>()
            };
        }
    }
}
