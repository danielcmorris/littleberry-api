using LittleberryApi.Models.DTOs;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MailToController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public MailToController(IEmailService emailService, IConfiguration configuration)
    {
        _emailService = emailService;
        _configuration = configuration;
    }

    /// <summary>
    /// Send email with API key validation
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SimpleMail mail)
    {
        var validApiKey = _configuration["AppSettings:ValidApiKey"];

        if (mail.Key != validApiKey)
        {
            return Unauthorized();
        }

        var success = await _emailService.SendSimpleMailAsync(mail);

        if (success)
        {
            return Ok(new { status = "sent" });
        }

        return StatusCode(500, new { status = "failed" });
    }
}
