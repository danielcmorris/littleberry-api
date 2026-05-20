using LittleberryApi.Models.DTOs;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers.PublicWebsite;

[ApiController]
[Route("api/[controller]")]
public class FestaRequestController : ControllerBase
{
    private readonly IEmailService _emailService;

    public FestaRequestController(IEmailService emailService)
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
    /// Submit a festa request
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] FestaRequest request)
    {
        var body = $@"
<h2>Festa Request</h2>
<table border='1' cellpadding='5'>
    <tr><td><strong>Name:</strong></td><td>{request.FirstName} {request.LastName}</td></tr>
    <tr><td><strong>Email:</strong></td><td>{request.Email}</td></tr>
    <tr><td><strong>Phone:</strong></td><td>{request.Phone}</td></tr>
    <tr><td><strong>Comments:</strong></td><td>{request.Comments}</td></tr>
</table>";

        await _emailService.SendEmailAsync(
            "festarequests@kofc.org",
            "noreply@mypfsa.org",
            "Festa Request Submission",
            body);

        return Ok(new { status = "sent" });
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
