using LittleberryApi.Data;
using LittleberryApi.Models;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly PfsaDbContext _context;
    private readonly IDataLayerService _dataLayer;

    public CatalogController(PfsaDbContext context, IDataLayerService dataLayer)
    {
        _context = context;
        _dataLayer = dataLayer;
    }

    /// <summary>
    /// Retrieves a list of the most recent 35 books added to the collection.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var list = await _context.Catalogs
            .Where(t => t.Status != "Deleted")
            .OrderByDescending(t => t.CreateDate)
            .Take(35)
            .ToListAsync();

        return Ok(list);
    }

    /// <summary>
    /// With the prefix of the subject as a parameter, this retrieves a list of active books within that subject
    /// </summary>
    /// <param name="prefix">The Prefix of the Subject eg. R for Reference</param>
    [HttpGet("{prefix}")]
    public async Task<IActionResult> GetByPrefix(string prefix)
    {
        var list = await _context.Catalogs
            .Where(t => t.Prefix == prefix && t.Status != "Deleted")
            .OrderBy(t => t.Title)
            .Take(500)
            .ToListAsync();

        return Ok(list);
    }

    /// <summary>
    /// Basic Search Function. All parameters must be included, but may be empty.
    /// </summary>
    [HttpGet("search")]
    [HttpGet("~/api/library/search")]
    public async Task<IActionResult> Search([FromQuery] string? prefix, [FromQuery] string? author, [FromQuery] string? title)
    {
        prefix ??= "";
        author ??= "";
        title ??= "";

        var list = await _context.CatalogSearchAsync(prefix, author, title, "Active");
        return Ok(list);
    }

    /// <summary>
    /// Get a specific book by prefix and book number
    /// </summary>
    [HttpGet("{prefix}/{booknumber:int}")]
    [Route("api/library/catalog/{prefix}/{booknumber}")]
    public async Task<IActionResult> GetBook(string prefix, int booknumber)
    {
        var books = await _context.GetBookAsync(prefix, booknumber);

        if (books.Count > 0)
        {
            return Ok(books[0]);
        }

        return Ok(-1);
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Book book, [FromQuery] string sid)
    {
        var account = await _context.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        // Get the last ID and prefix for the subject
        var sql = $"SELECT LastId, Prefix FROM dbo.Subjects WHERE id={book.SubjectId}";
        var ds = await _dataLayer.GetDataAsync(sql);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            var row = ds.Tables[0].Rows[0];
            book.BookNumber = (int)row["LastId"] + 1;
            book.Prefix = (string)row["Prefix"];

            // Update the last_id in subject_lookup
            sql = $"UPDATE subject_lookup SET last_id={book.BookNumber} WHERE id={book.SubjectId}";
            await _dataLayer.RunSqlAsync(sql);
        }

        book.CallNumber = book.Prefix + book.BookNumber.ToString();
        book.CreateBy = $"{account.LastName},{account.FirstName}";
        book.CreateDate = DateTime.Now;

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return Ok(book);
    }

    /// <summary>
    /// Update an existing book
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Book book, [FromQuery] string sid)
    {
        var account = await _context.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.BookId == book.BookId);

        if (existingBook == null)
        {
            return NotFound();
        }

        // Log status change in history
        if (existingBook.Status != book.Status)
        {
            var history = new History
            {
                BookId = existingBook.BookId,
                CreateBy = $"{account.LastName},{account.FirstName}",
                CreateDate = DateTime.Now,
                Status = book.Status,
                Detail = $"Changed from {existingBook.Status} to {book.Status}"
            };
            _context.Histories.Add(history);
        }

        // Preserve original values
        book.CallNumber = existingBook.CallNumber;
        book.CreateBy = existingBook.CreateBy;
        book.CreateDate = existingBook.CreateDate;
        book.ModifiedBy = $"{account.LastName},{account.FirstName}";
        book.ModifiedDate = DateTime.Now;

        _context.Entry(existingBook).State = EntityState.Detached;
        _context.Entry(book).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(book);
    }

    /// <summary>
    /// Delete a book (stub - not implemented)
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        // Not implemented in original
        return Ok();
    }
}
