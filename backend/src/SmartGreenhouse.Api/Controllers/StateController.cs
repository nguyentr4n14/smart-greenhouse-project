using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Api.Contracts;
using SmartGreenhouse.Application.Adapters;
using SmartGreenhouse.Application.Services;
using SmartGreenhouse.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartGreenhouse.Api.Controllers
{
    [ApiController]
    [Route("api/state")]
    public class StateController : ControllerBase
    {
        private readonly StateService _stateService;
        private readonly AppDbContext _dbContext;
        private readonly AdapterRegistry _adapterRegistry;

        public StateController(
            StateService stateService,
            AppDbContext dbContext,
            AdapterRegistry adapterRegistry)
        {
            _stateService = stateService;
            _dbContext = dbContext;
            _adapterRegistry = adapterRegistry;
        }

        [HttpPost("tick")]
        public async Task<IActionResult> RunTick([FromBody] RunStateTickRequest request)
        {
            var result = await _stateService.TickAsync(request.DeviceId);
            return Ok(result);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentState([FromQuery] int deviceId)
        {
            var snapshot = await _dbContext.DeviceStates
                .Where(s => s.DeviceId == deviceId)
                .OrderByDescending(s => s.EnteredAt)
                .FirstOrDefaultAsync();

            if (snapshot == null)
            {
                return Ok(new { deviceId, stateName = "Idle", enteredAt = DateTime.MinValue, notes = "No state recorded" });
            }
            return Ok(snapshot);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetStateHistory([FromQuery] int deviceId)
        {
            var history = await _dbContext.DeviceStates
                .Where(s => s.DeviceId == deviceId)
                .OrderByDescending(s => s.EnteredAt)
                .Take(50)
                .ToListAsync();

            return Ok(history);
        }

        [HttpPost("adapters")]
        public IActionResult SetAdapterSettings([FromBody] AdapterSettingsRequest request)
        {
            _adapterRegistry.ActuatorMode = request.ActuatorMode;
            _adapterRegistry.NotificationMode = request.NotificationMode;
            _adapterRegistry.WebhookUrl = request.WebhookUrl;

            return Ok(new
            {
                _adapterRegistry.ActuatorMode,
                _adapterRegistry.NotificationMode,
                _adapterRegistry.WebhookUrl
            });
        }

        [HttpGet("adapters")]
        public IActionResult GetAdapterSettings()
        {
            return Ok(new
            {
                _adapterRegistry.ActuatorMode,
                _adapterRegistry.NotificationMode,
                _adapterRegistry.WebhookUrl
            });
        }
    }
}
