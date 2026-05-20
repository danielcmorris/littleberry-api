using LittleberryApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookHistoryController : ControllerBase
{
    private readonly PfsaDbContext _context;

    public BookHistoryController(PfsaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get book history by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        // Note: Original code had hardcoded "none" and 1 - preserving that behavior
        var history = await _context.GetBookHistoryAsync("none", 1);

        if (history.Count > 0)
        {
            return Ok(history[0]);
        }

        return Ok("No History Found");
    }
}
