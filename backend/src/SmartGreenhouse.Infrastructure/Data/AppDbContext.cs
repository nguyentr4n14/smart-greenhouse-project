using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
    public DbSet<AlertRule> AlertRules => Set<AlertRule>();
    public DbSet<AlertNotification> AlertNotifications => Set<AlertNotification>();
    public DbSet<ControlProfile> ControlProfiles => Set<ControlProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Device configuration
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DeviceName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DeviceType).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.DeviceName);
        });

        // SensorReading configuration
        modelBuilder.Entity<SensorReading>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SensorType).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.DeviceId, e.SensorType, e.Timestamp });
            entity.HasIndex(e => e.Timestamp);

            entity.HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AlertRule configuration
        modelBuilder.Entity<AlertRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SensorType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.OperatorSymbol).IsRequired().HasMaxLength(10);
            entity.HasIndex(e => new { e.DeviceId, e.SensorType, e.IsActive });

            entity.HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AlertNotification configuration
        modelBuilder.Entity<AlertNotification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SensorType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => new { e.DeviceId, e.TriggeredAt });
            entity.HasIndex(e => e.AlertRuleId);

            entity.HasOne(e => e.AlertRule)
                .WithMany()
                .HasForeignKey(e => e.AlertRuleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ControlProfile configuration
        modelBuilder.Entity<ControlProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StrategyKey).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.DeviceId).IsUnique();

            entity.HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}