using LittleberryApi.Models.DTOs;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers.PublicWebsite;

[ApiController]
[Route("api/[controller]")]
public class SimpleMailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public SimpleMailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    /// <summary>
    /// Get (stub - not implemented)
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }

    /// <summary>
    /// Get by ID (stub - not implemented)
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        return Ok();
    }

    /// <summary>
    /// Send a simple email
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SimpleMail msg)
    {
        var success = await _emailService.SendSimpleMailAsync(msg);

        if (success)
        {
            return Ok(new { status = "sent" });
        }

        return StatusCode(500, new { status = "failed" });
    }

    /// <summary>
    /// Update (stub - not implemented)
    /// </summary>
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] string value)
    {
        return Ok();
    }

    /// <summary>
    /// Delete (stub - not implemented)
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return Ok();
    }
}
