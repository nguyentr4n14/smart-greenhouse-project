using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Application.Events.Observers;

public class AlertRuleObserver : IReadingObserver
{
    private readonly AppDbContext _context;
    private readonly ILogger<AlertRuleObserver> _logger;

    public AlertRuleObserver(AppDbContext context, ILogger<AlertRuleObserver> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task OnReadingAsync(IReadingEvent readingEvent)
    {
        // Query active rules for this device and sensor type
        var rules = await _context.AlertRules
            .Where(r => r.DeviceId == readingEvent.DeviceId
                        && r.SensorType == readingEvent.SensorType
                        && r.IsActive)
            .ToListAsync();

        foreach (var rule in rules)
        {
            if (EvaluateRule(rule, readingEvent.Value))
            {
                var notification = new AlertNotification
                {
                    AlertRuleId = rule.Id,
                    DeviceId = readingEvent.DeviceId,
                    SensorType = readingEvent.SensorType,
                    Value = readingEvent.Value,
                    Threshold = rule.Threshold,
                    Message = $"Alert: {readingEvent.SensorType} {rule.OperatorSymbol} {rule.Threshold} (actual: {readingEvent.Value})",
                    TriggeredAt = readingEvent.Timestamp
                };

                _context.AlertNotifications.Add(notification);

                _logger.LogWarning(
                    "Alert triggered: Device={DeviceId}, Rule={RuleId}, Message={Message}",
                    readingEvent.DeviceId,
                    rule.Id,
                    notification.Message);
            }
        }

        await _context.SaveChangesAsync();
    }

    private bool EvaluateRule(AlertRule rule, double value)
    {
        return rule.OperatorSymbol switch
        {
            ">" => value > rule.Threshold,
            "<" => value < rule.Threshold,
            ">=" => value >= rule.Threshold,
            "<=" => value <= rule.Threshold,
            "==" => Math.Abs(value - rule.Threshold) < 0.001,
            _ => false
        };
    }
}