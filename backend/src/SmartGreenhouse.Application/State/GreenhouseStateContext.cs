using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Application.Control;
using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Application.State
{
    public class GreenhouseStateContext
    {
        
        public required int DeviceId { get; init; }
        public required LatestReadingsDto LatestReadings { get; init; }
        public ControlProfile? ActiveProfile { get; init; }

        public required IActuatorAdapter ActuatorAdapter { get; init; }
        public required INotificationAdapter NotificationAdapter { get; init; }
    }
}
