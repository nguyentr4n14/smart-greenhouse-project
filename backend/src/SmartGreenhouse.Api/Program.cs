using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Application.Control;
using SmartGreenhouse.Application.DeviceIntegration;
using SmartGreenhouse.Application.Events;
using SmartGreenhouse.Application.Events.Observers;
using SmartGreenhouse.Application.Services;
using SmartGreenhouse.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Device integration
builder.Services.AddSingleton<IDeviceFactory, SimulatedDeviceFactory>();

// Observer pattern - scoped to match DbContext lifetime
builder.Services.AddScoped<IReadingPublisher>(serviceProvider =>
{
    var publisher = new ReadingPublisher();
    var logObserver = serviceProvider.GetRequiredService<LogObserver>();
    var alertObserver = serviceProvider.GetRequiredService<AlertRuleObserver>();

    publisher.Subscribe(logObserver);
    publisher.Subscribe(alertObserver);

    return publisher;
});

builder.Services.AddScoped<LogObserver>();
builder.Services.AddScoped<AlertRuleObserver>();

// Strategy pattern
builder.Services.AddSingleton<HysteresisCoolingStrategy>();
builder.Services.AddSingleton<MoistureTopUpStrategy>();
builder.Services.AddScoped<ControlStrategySelector>();
builder.Services.AddScoped<ControlService>();

// Application services
builder.Services.AddScoped<CaptureReadingService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();