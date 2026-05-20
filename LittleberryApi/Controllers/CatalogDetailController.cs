using LittleberryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogDetailController : ControllerBase
{
    private readonly PfsaDbContext _context;

    public CatalogDetailController(PfsaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get book history by prefix and book number
    /// </summary>
    [HttpGet("{prefix}/{booknumber}")]
    [Route("~/library/catalog/{prefix}/{booknumber}/history")]
    public async Task<IActionResult> GetHistory(string prefix, int booknumber)
    {
        var history = await _context.GetBookHistoryAsync(prefix, booknumber);

        if (history.Count > 0)
        {
            return Ok(history);
        }

        return Ok("No History Found");
    }

    /// <summary>
    /// Get catalog by prefix
    /// </summary>
    [HttpGet("{prefix}")]
    public async Task<IActionResult> GetByPrefix(string prefix)
    {
        var list = await _context.Catalogs
            .Where(t => t.Prefix == prefix && t.Status != "Deleted")
            .OrderByDescending(t => t.Title)
            .Take(500)
            .ToListAsync();

        return Ok(list);
    }
}
