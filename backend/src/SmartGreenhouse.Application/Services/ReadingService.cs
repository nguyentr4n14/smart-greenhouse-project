using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Domain.Enums;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Application.Services;

public class ReadingService
{
    private readonly AppDbContext _context;

    public ReadingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SensorReading>> GetReadingsAsync(
        int? deviceId = null,
        SensorTypeEnum? sensorType = null,
        int take = 100)
    {
        var query = _context.SensorReadings.AsQueryable();

        if (deviceId.HasValue)
            query = query.Where(r => r.DeviceId == deviceId.Value);

        if (sensorType.HasValue)
            query = query.Where(r => r.SensorType == sensorType.Value);

        return await query
            .OrderByDescending(r => r.Timestamp)
            .Take(take)
            .ToListAsync();
    }

    public async Task<SensorReading?> GetLatestReadingAsync(int deviceId, SensorTypeEnum sensorType)
    {
        return await _context.SensorReadings
            .Where(r => r.DeviceId == deviceId && r.SensorType == sensorType)
            .OrderByDescending(r => r.Timestamp)
            .FirstOrDefaultAsync();
    }
}