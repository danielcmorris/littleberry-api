using LittleberryApi.Data;
using LittleberryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectController : ControllerBase
{
    private readonly PfsaDbContext _context;

    public SubjectController(PfsaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all subjects
    /// </summary>
    [HttpGet]
    [Route("~/library/subjects")]
    [Route("~/api/subject")]
    public async Task<IActionResult> Get()
    {
        var subjects = await _context.Subjects
            .OrderBy(s => s.Name)
            .ToListAsync();

        return Ok(subjects);
    }

    /// <summary>
    /// Get subjects by prefix
    /// </summary>
    [HttpGet("{prefix}")]
    [Route("~/library/subjects/{prefix}")]
    public async Task<IActionResult> GetByPrefix(string prefix)
    {
        var subjects = await _context.Subjects
            .Where(p => p.Prefix == prefix)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return Ok(subjects);
    }

    /// <summary>
    /// Update a subject
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Subject subject)
    {
        _context.Entry(subject).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(subject);
    }

    /// <summary>
    /// Create a new subject
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Subject subject)
    {
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        return Ok(subject);
    }

    /// <summary>
    /// Delete a subject (stub - not implemented)
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        // Not implemented in original
        return Ok();
    }
}
