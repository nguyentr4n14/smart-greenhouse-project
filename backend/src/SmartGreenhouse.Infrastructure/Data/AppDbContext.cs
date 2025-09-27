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
        // Device configuration
        modelBuilder.Entity<Device>().HasKey(d => d.Id);
        modelBuilder.Entity<Device>().Property(d => d.DeviceName).HasMaxLength(120);
        modelBuilder.Entity<Device>().HasIndex(d => d.DeviceName);
        modelBuilder.Entity<Device>().HasIndex(d => d.DeviceType); // DeviceType index

        // SensorReading configuration
        modelBuilder.Entity<SensorReading>().HasKey(r => r.Id);
        
        // Configure relationship: one Device → many Readings
        modelBuilder.Entity<SensorReading>()
            .HasOne(r => r.Device)
            .WithMany(d => d.Readings)
            .HasForeignKey(r => r.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index includes enum column (stored as int)
        modelBuilder.Entity<SensorReading>()
            .HasIndex(r => new { r.DeviceId, r.SensorType, r.Timestamp });
    }
}