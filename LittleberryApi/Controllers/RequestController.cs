using LittleberryApi.Data;
using LittleberryApi.Models;
using LittleberryApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Controllers;

public class QuickRequestInput
{
    public string? CallNumber { get; set; }
    public string? RequestByEmail { get; set; }
    public DateTime RequestDate { get; set; }
}

public class QuickRequestUpdateInput
{
    public int ReservationSubId { get; set; }
    public bool OnOff { get; set; }
    public string? ChangeType { get; set; }
    public DateTime ChangeDate { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class RequestController : ControllerBase
{
    private readonly PfsaDbContext _context;

    public RequestController(PfsaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all open requests from the system (admin only)
    /// </summary>
    [HttpGet]
    [Route("~/library/request")]
    [Route("~/api/request")]
    public async Task<IActionResult> Get([FromQuery] string sid)
    {
        var account = await _context.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        var requests = await _context.VwReservations
            .Where(p => p.ReshelveDate == null)
            .OrderByDescending(p => p.RequestDate)
            .ToListAsync();

        return Ok(requests);
    }

    /// <summary>
    /// Get a specific request by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] string sid)
    {
        var request = await _context.VwReservations
            .FirstOrDefaultAsync(p => p.ReservationSubId == id);

        if (request == null)
        {
            return NotFound();
        }

        return Ok(request);
    }

    /// <summary>
    /// Create a new book request
    /// </summary>
    [HttpPost]
    [Route("~/library/request")]
    [Route("~/api/request")]
    public async Task<IActionResult> Post([FromBody] QuickRequestInput obj, [FromQuery] string sid)
    {
        // Find the book by call number
        var books = await _context.Books
            .Where(p => p.CallNumber == obj.CallNumber)
            .ToListAsync();

        if (books.Count == 0)
        {
            return Ok("Sorry, we have no record of this book in our catalog");
        }

        foreach (var book in books)
        {
            var status = book.Status?.ToUpper() ?? "";

            if (status != "DELETED" && status != "UNAVAILABLE" && status != "LOST")
            {
                var requests = await _context.AddRequestAsync(book.BookId, obj.RequestByEmail ?? "", DateTime.Now, sid);

                if (requests.Count > 0)
                {
                    return Ok(requests[0]);
                }
            }
            else
            {
                return Ok($"Sorry, this book is currently {status.ToLower()}");
            }
        }

        return Ok("Unable to process request");
    }

    /// <summary>
    /// Update a request
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] QuickRequestUpdateInput obj, [FromQuery] string sid)
    {
        var account = await _context.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        if (obj.ChangeDate == DateTime.MinValue)
        {
            obj.ChangeDate = DateTime.Now;
        }

        var requests = await _context.UpdateRequestAsync(obj.ReservationSubId, obj.ChangeType ?? "", obj.OnOff, obj.ChangeDate, sid);

        if (requests.Count > 0)
        {
            return Ok(requests[0]);
        }

        return NotFound();
    }

    /// <summary>
    /// Delete a request (stub - not implemented)
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        // Not implemented in original
        return Ok();
    }
}
