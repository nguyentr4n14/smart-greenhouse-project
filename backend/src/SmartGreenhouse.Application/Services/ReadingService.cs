using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Domain.Enums;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Application.Services;

public class ReadingService
{
    private readonly AppDbContext _db;
    public ReadingService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<SensorReading>> QueryAsync(
        int? deviceId = null, 
        SensorTypeEnum? sensorType = null, 
        int take = 200)
    {
        var q = _db.Readings
            .Include(r => r.Device) // optional, if you want Device info in results
            .AsNoTracking()
            .OrderByDescending(r => r.Timestamp)
            .AsQueryable();

        if (deviceId.HasValue) 
            q = q.Where(r => r.DeviceId == deviceId.Value);
        
        if (sensorType.HasValue) 
            q = q.Where(r => r.SensorType == sensorType.Value);

        return await q.Take(take).ToListAsync();
    }

    public async Task<SensorReading> AddAsync(SensorReading reading)
    {
        _db.Readings.Add(reading);
        await _db.SaveChangesAsync();
        return reading;
    }
}