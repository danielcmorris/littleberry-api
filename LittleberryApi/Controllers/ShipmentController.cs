using LittleberryApi.Data;
using LittleberryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentController : ControllerBase
{
    private readonly PfsaDbContext _context;

    public ShipmentController(PfsaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all shipments
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetShipments()
    {
        var shipments = await _context.Shipments.ToListAsync();
        return Ok(shipments);
    }

    /// <summary>
    /// Get a shipment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetShipment(int id)
    {
        var shipment = await _context.Shipments.FindAsync(id);

        if (shipment == null)
        {
            return NotFound();
        }

        return Ok(shipment);
    }

    /// <summary>
    /// Update a shipment
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutShipment(int id, [FromBody] Shipment shipment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != shipment.ShipmentId)
        {
            return BadRequest();
        }

        _context.Entry(shipment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ShipmentExistsAsync(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Create a new shipment
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostShipment([FromBody] Shipment shipment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetShipment), new { id = shipment.ShipmentId }, shipment);
    }

    /// <summary>
    /// Delete a shipment
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShipment(int id)
    {
        var shipment = await _context.Shipments.FindAsync(id);

        if (shipment == null)
        {
            return NotFound();
        }

        _context.Shipments.Remove(shipment);
        await _context.SaveChangesAsync();

        return Ok(shipment);
    }

    private async Task<bool> ShipmentExistsAsync(int id)
    {
        return await _context.Shipments.AnyAsync(e => e.ShipmentId == id);
    }
}
