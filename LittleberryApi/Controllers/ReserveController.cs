using LittleberryApi.Data;
using LittleberryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReserveController : ControllerBase
{
    private readonly PfsaDbContext _context;

    public ReserveController(PfsaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all reservations
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string sid)
    {
        var account = await _context.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        var reservations = await _context.Reservations.ToListAsync();
        return Ok(reservations);
    }

    /// <summary>
    /// Get a reservation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] string sid)
    {
        var account = await _context.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(p => p.ReservationId == id);

        if (reservation == null)
        {
            return NotFound();
        }

        return Ok(reservation);
    }

    /// <summary>
    /// Create a new reservation
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Reservation obj, [FromQuery] string sid)
    {
        var account = await _context.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        _context.Reservations.Add(obj);
        await _context.SaveChangesAsync();

        return Ok(obj);
    }

    /// <summary>
    /// Update a reservation
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Reservation obj, [FromQuery] string sid)
    {
        var account = await _context.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        _context.Entry(obj).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(obj);
    }

    /// <summary>
    /// Delete a reservation
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] string sid)
    {
        var account = await _context.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        var reservation = await _context.Reservations
            .SingleOrDefaultAsync(row => row.ReservationId == id);

        if (reservation == null)
        {
            return Ok(-1); // Did not find record
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return Ok(id); // Found record and deleted it
    }
}
