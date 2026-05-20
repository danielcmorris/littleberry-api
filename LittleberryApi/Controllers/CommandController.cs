using LittleberryApi.Data;
using LittleberryApi.Models.DTOs;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommandController : ControllerBase
{
    private readonly PfsaDbContext _context;
    private readonly IDataLayerService _dataLayer;

    public CommandController(PfsaDbContext context, IDataLayerService dataLayer)
    {
        _context = context;
        _dataLayer = dataLayer;
    }

    /// <summary>
    /// Get command info
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Command Controller");
    }

    /// <summary>
    /// Execute a stored procedure
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CustomCommandObject cmd, [FromQuery] string sid)
    {
        var account = await _context.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        if (account.AccountType != "Admin")
        {
            return Unauthorized();
        }

        var sql = $"EXEC {cmd.ProcedureName} {cmd.Parameters}";
        var ds = await _dataLayer.GetDataAsync(sql);

        return Ok(ds);
    }
}
