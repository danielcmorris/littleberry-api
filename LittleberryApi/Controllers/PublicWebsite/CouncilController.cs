using LittleberryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Controllers.PublicWebsite;

[ApiController]
public class CouncilController : ControllerBase
{
    private readonly PfsaDbContext _context;

    public CouncilController(PfsaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get council by ID
    /// </summary>
    [HttpGet]
    [Route("api/website/council/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var council = await _context.Councils
            .FirstOrDefaultAsync(c => c.CouncilID == id);

        if (council == null)
        {
            return NotFound();
        }

        return Ok(council);
    }
}
