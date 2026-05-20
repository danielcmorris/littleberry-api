using LittleberryApi.Models.DTOs;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers.PublicWebsite;

[ApiController]
[Route("api/[controller]")]
public class QuoteRequestController : ControllerBase
{
    private readonly IEmailService _emailService;

    public QuoteRequestController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    /// <summary>
    /// Submit a quote request
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] QuoteRequest quote)
    {
        var body = BuildEmailBody(quote);

        await _emailService.SendEmailAsync(
            "collegeplanning@kofc.org",
            "noreply@mypfsa.org",
            "College Planning Quote Request",
            body);

        return Ok(new { status = "sent" });
    }

    private static string BuildEmailBody(QuoteRequest quote)
    {
        return $@"
<h2>College Planning Quote Request</h2>
<table border='1' cellpadding='5'>
    <tr><td><strong>First Name:</strong></td><td>{quote.FirstName}</td></tr>
    <tr><td><strong>Last Name:</strong></td><td>{quote.LastName}</td></tr>
    <tr><td><strong>Email:</strong></td><td>{quote.Email}</td></tr>
    <tr><td><strong>Phone:</strong></td><td>{quote.Phone}</td></tr>
    <tr><td><strong>Address:</strong></td><td>{quote.Address1}</td></tr>
    <tr><td><strong>City:</strong></td><td>{quote.City}</td></tr>
    <tr><td><strong>State:</strong></td><td>{quote.State}</td></tr>
    <tr><td><strong>Postal Code:</strong></td><td>{quote.PostalCode}</td></tr>
</table>";
    }
}
