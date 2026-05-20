using LittleberryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly PfsaDbContext _context;

    public AuthorController(PfsaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get list of authors with book counts
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int BookCount = 20)
    {
        var authors = await _context.GetAuthorListAsync(BookCount);
        return Ok(authors);
    }

    /// <summary>
    /// Get books by author name
    /// </summary>
    [HttpGet("{AuthorName}")]
    public async Task<IActionResult> GetByAuthor(string AuthorName)
    {
        var books = await _context.Books
            .Where(p => p.Author == AuthorName)
            .ToListAsync();

        return Ok(books);
    }
}
