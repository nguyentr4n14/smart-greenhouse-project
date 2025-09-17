using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<SensorReading> Readings => Set<SensorReading>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<SensorReading>()
            .HasKey(r => r.Id);

        // Configure relationship: one Device → many Readings
        modelBuilder.Entity<SensorReading>()
            .HasOne(r => r.Device)
            .WithMany(d => d.Readings)
            .HasForeignKey(r => r.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for query performance
        modelBuilder.Entity<SensorReading>()
            .HasIndex(r => new { r.DeviceId, r.SensorType, r.Timestamp });
    }
}