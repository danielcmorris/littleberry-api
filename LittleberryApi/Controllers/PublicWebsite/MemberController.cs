using LittleberryApi.Data;
using LittleberryApi.Models;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Controllers.PublicWebsite;

[ApiController]
public class MemberController : ControllerBase
{
    private readonly PfsaDbContext _context;
    private readonly IDataLayerService _dataLayer;

    public MemberController(PfsaDbContext context, IDataLayerService dataLayer)
    {
        _context = context;
        _dataLayer = dataLayer;
    }

    /// <summary>
    /// Get a member by ID
    /// </summary>
    [HttpGet]
    [Route("api/website/member/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var member = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == id);

        if (member == null)
        {
            return NotFound();
        }

        // Don't return password
        member.Password = "";
        return Ok(member);
    }

    /// <summary>
    /// Create a new member
    /// </summary>
    [HttpPost]
    [Route("api/website/member")]
    public async Task<IActionResult> Post([FromBody] Member member)
    {
        var account = new Account
        {
            FirstName = member.FirstName,
            LastName = member.LastName,
            Phone = member.Phone,
            Email = member.Email,
            Address1 = member.Address1,
            Address2 = member.Address2,
            City = member.City,
            State = member.State,
            Zip = member.PostalCode,
            PostalCode = member.PostalCode,
            AccountType = "Member",
            Status = "Active",
            CreateDate = DateTime.Now,
            CreateBy = $"{member.LastName}, {member.FirstName}",
            OfficeId = 1
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Get session ID
        var sql = $"strpLogin '{account.Email}';";
        var ds = await _dataLayer.GetDataAsync(sql);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            account.SessionId = ds.Tables[0].Rows[0]["SessionKey"]?.ToString();
        }

        return Ok(account);
    }
}
