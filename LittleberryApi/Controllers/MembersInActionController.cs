using LittleberryApi.Models.DTOs;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers;

[ApiController]
public class MembersInActionController : ControllerBase
{
    private readonly IEmailService _emailService;

    public MembersInActionController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    /// <summary>
    /// Send members in action form
    /// </summary>
    [HttpPost]
    [Route("api/members-in-action/send")]
    public async Task<IActionResult> Post(
        [FromBody] MembersInAction form,
        [FromQuery] string? email1,
        [FromQuery] string? email2)
    {
        var body = BuildEmailBody(form);

        // Send to main recipient
        await _emailService.SendEmailAsync(
            "fraternalprograms@kofc.org",
            "noreply@mypfsa.org",
            "Members In Action Form Submission",
            body);

        // Send to additional recipients if provided
        if (!string.IsNullOrEmpty(email1))
        {
            await _emailService.SendEmailAsync(email1, "noreply@mypfsa.org", "Members In Action Form Submission", body);
        }

        if (!string.IsNullOrEmpty(email2))
        {
            await _emailService.SendEmailAsync(email2, "noreply@mypfsa.org", "Members In Action Form Submission", body);
        }

        return Ok(new { status = "sent" });
    }

    private static string BuildEmailBody(MembersInAction form)
    {
        return $@"
<h2>Members In Action Form Submission</h2>
<table border='1' cellpadding='5'>
    <tr><td><strong>Council Number:</strong></td><td>{form.CouncilNumber}</td></tr>
    <tr><td><strong>Council City:</strong></td><td>{form.CouncilCity}</td></tr>
    <tr><td><strong>Contact Name:</strong></td><td>{form.ContactName}</td></tr>
    <tr><td><strong>Address:</strong></td><td>{form.Address1}</td></tr>
    <tr><td><strong>City:</strong></td><td>{form.City}</td></tr>
    <tr><td><strong>State:</strong></td><td>{form.State}</td></tr>
    <tr><td><strong>Zip Code:</strong></td><td>{form.ZipCode}</td></tr>
    <tr><td><strong>Home Phone:</strong></td><td>{form.HomePhone}</td></tr>
    <tr><td><strong>Cell Phone:</strong></td><td>{form.CellPhone}</td></tr>
    <tr><td><strong>Email:</strong></td><td>{form.Email}</td></tr>
    <tr><td><strong>Event Date:</strong></td><td>{form.EventDate}</td></tr>
    <tr><td><strong>Benefiting Organization:</strong></td><td>{form.BenefitingOrganization}</td></tr>
    <tr><td><strong>Objective:</strong></td><td>{form.Objective}</td></tr>
    <tr><td><strong>Implementation Plan:</strong></td><td>{form.ImplementationPlan}</td></tr>
    <tr><td><strong>Council Services Provided:</strong></td><td>{form.CouncilServicesProvided}</td></tr>
    <tr><td><strong>Marketing Tools:</strong></td><td>{form.MarketingTools}</td></tr>
    <tr><td><strong>Assistance Details:</strong></td><td>{form.AssistanceDetails}</td></tr>
    <tr><td><strong>Amount:</strong></td><td>{form.Amount}</td></tr>
    <tr><td><strong>Comments:</strong></td><td>{form.Comments}</td></tr>
</table>";
    }
}
