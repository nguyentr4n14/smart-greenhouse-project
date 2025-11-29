using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using SmartGreenhouse.Api.Mqtt;
using SmartGreenhouse.Api.RealTime;
using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Application.Adapters;
using SmartGreenhouse.Application.Adapters.Actuators;
using SmartGreenhouse.Application.Adapters.Notifications;
using SmartGreenhouse.Application.Control;
using SmartGreenhouse.Application.DeviceIntegration;
using SmartGreenhouse.Application.Events;
using SmartGreenhouse.Application.Events.Observers;
using SmartGreenhouse.Application.Mqtt;
using SmartGreenhouse.Application.RealTime;
using SmartGreenhouse.Application.Services;
using SmartGreenhouse.Application.State;
using SmartGreenhouse.Application.State.States;
using SmartGreenhouse.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database - Use pooled factory for better performance and singleton/scoped compatibility
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Also register scoped DbContext for controllers using the factory
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

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
builder.Services.AddScoped<ReadingService>();

// ========== NEW FOR ASSIGNMENT 4 ==========

// HTTP clients for adapters
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<HttpActuatorAdapter>(client =>
{
    // Default external IoT hub or device controller URL
    client.BaseAddress = new Uri("http://localhost:5055/");
});
builder.Services.AddHttpClient("WebhookClient");

// Actuator Adapters
builder.Services.AddSingleton<SimulatedActuatorAdapter>();
builder.Services.AddSingleton<HttpActuatorAdapter>();

// Notification Adapters
builder.Services.AddSingleton<ConsoleNotificationAdapter>();
builder.Services.AddSingleton<WebhookNotificationAdapter>();

// Adapter Registry
builder.Services.AddSingleton<AdapterRegistry>();

// State Pattern - Register concrete state classes (with full namespace)
builder.Services.AddScoped<IdleState>();
builder.Services.AddScoped<CoolingState>();
builder.Services.AddScoped<IrrigatingState>();
builder.Services.AddScoped<AlarmState>();

// State Engine & Service
builder.Services.AddScoped<GreenhouseStateEngine>();
builder.Services.AddScoped<StateService>();

// ========== END ASSIGNMENT 4 ==========

// ========== ASSIGNMENT 5 - Real-Time IoT Integration ==========

// WebSocket real-time broadcasting
builder.Services.AddSingleton<LiveReadingHub>();
builder.Services.AddSingleton<IRealTimeNotifier, WebSocketRealTimeNotifier>();

// MQTT message handling
builder.Services.AddScoped<IEsp32MessageHandler, Esp32MessageHandler>();

// MQTT broker hosted service
builder.Services.AddHostedService<MqttBrokerHostedService>();

// ========== END ASSIGNMENT 5 ==========

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

// WebSocket endpoint for live readings
app.UseWebSockets();

app.Map("/ws/live-readings", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    var hub = context.RequestServices.GetRequiredService<LiveReadingHub>();
    var socket = await context.WebSockets.AcceptWebSocketAsync();

    hub.Register(socket);

    var buffer = new byte[4096];
    while (socket.State == System.Net.WebSockets.WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
        if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
            break;
    }

    hub.Unregister(socket);
    await socket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
});

app.MapControllers();

app.Run();