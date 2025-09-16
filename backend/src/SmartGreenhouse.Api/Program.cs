using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Application.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using SmartGreenhouse.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Host=localhost;Port=5432;Database=greenhouse;Username=postgres;Password=1234";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));

builder.Services.AddScoped<ReadingService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
await app.RunAsync();
