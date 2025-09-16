using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Infrastructure.Data;

namespace SmartGreenhouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _db;
    public DevicesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _db.Devices.AsNoTracking().ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Device device)
    {
        _db.Devices.Add(device);
        await _db.SaveChangesAsync();
        return Ok(device);
    }
}