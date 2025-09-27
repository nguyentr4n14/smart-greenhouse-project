using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Application.DeviceIntegration;
using SmartGreenhouse.Application.Services;
using SmartGreenhouse.Infrastructure.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add JSON configuration for enum string serialization
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Host=localhost;Port=5432;Database=greenhouse;Username=postgres;Password=1234";
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(connectionString));

// Register device integration factories
builder.Services.AddSingleton<SimulatedDeviceFactory>();

// Register device factory resolver
builder.Services.AddSingleton<IDeviceFactoryResolver, DeviceFactoryResolver>();

// Register application services
builder.Services.AddScoped<CaptureReadingService>();
builder.Services.AddScoped<ReadingService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
await app.RunAsync();
