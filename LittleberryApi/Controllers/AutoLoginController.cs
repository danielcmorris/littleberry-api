using LittleberryApi.Models;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutoLoginController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public AutoLoginController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>
    /// Auto login with account info
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Account account)
    {
        if (string.IsNullOrEmpty(account.SessionId))
        {
            return BadRequest("SessionId is required");
        }

        var user = await _sessionService.GetUserBySessionIdAsync(account.SessionId);

        if (user != null)
        {
            return Ok(user);
        }

        return Unauthorized();
    }
}
