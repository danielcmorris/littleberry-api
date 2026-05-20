using System.Data;
using System.Security.Cryptography;
using System.Text;
using LittleberryApi.Data;
using LittleberryApi.Models;
using LittleberryApi.Models.DTOs;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    private readonly PfsaDbContext _context;
    private readonly ISessionService _sessionService;
    private readonly IDataLayerService _dataLayer;

    public AccountController(PfsaDbContext context, ISessionService sessionService, IDataLayerService dataLayer)
    {
        _context = context;
        _sessionService = sessionService;
        _dataLayer = dataLayer;
    }

    /// <summary>
    /// Get all accounts (requires authentication)
    /// </summary>
    [HttpGet]
    [Route("api/account")]
    [Route("library/account")]
    public async Task<IActionResult> Get([FromQuery] string sid)
    {
        var userId = await _sessionService.CheckUserAsync(sid);

        if (userId <= 0)
        {
            return Unauthorized();
        }

        var list = await _context.Accounts
            .OrderByDescending(t => t.LastName)
            .Take(1500)
            .ToListAsync();

        return Ok(list);
    }

    /// <summary>
    /// Search accounts by email
    /// </summary>
    [HttpGet]
    [Route("library/accounts/search/{searchtype}")]
    public async Task<IActionResult> Search(string searchtype, [FromQuery] string q, [FromQuery] string sid)
    {
        var account = await _sessionService.GetUserBySessionIdAsync(sid);

        if (account == null)
        {
            return Unauthorized();
        }

        if (account.AccountType != "Admin")
        {
            return Unauthorized();
        }

        var accounts = await _context.Accounts
            .Where(p => p.Email == q)
            .ToListAsync();

        if (accounts.Count > 0)
        {
            // Don't send passwords
            foreach (var a in accounts)
            {
                a.Password = "";
            }
            return Ok(accounts);
        }

        return Ok("No Accounts Found");
    }

    /// <summary>
    /// Get account by ID
    /// </summary>
    [HttpGet]
    [Route("api/account/{id}")]
    [Route("library/account/{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] string sid)
    {
        var securityAccount = await _context.GetUserBySessionIdAsync(sid);

        if (securityAccount == null)
        {
            return Unauthorized();
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(p => p.AccountId == id);

        if (account != null)
        {
            account.Password = ""; // Don't send password
            return Ok(account);
        }

        return Ok();
    }

    /// <summary>
    /// Login with credentials
    /// </summary>
    [HttpPost]
    [Route("api/account")]
    public async Task<IActionResult> Login([FromBody] LoginCredentials credentials)
    {
        var encryptedPassword = CalculateMD5Hash(credentials.Password ?? "");

        var sql = $"strpLogin '{credentials.Email}';";
        var ds = await _dataLayer.GetDataAsync(sql);

        if (ds.Tables.Count > 0)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var account = new Account
                {
                    AccountId = int.Parse(dr["AccountId"].ToString()!),
                    FirstName = dr["FirstName"]?.ToString(),
                    LastName = dr["LastName"]?.ToString(),
                    Phone = dr["Phone"]?.ToString(),
                    Email = dr["Email"]?.ToString(),
                    Address1 = dr["Address1"]?.ToString(),
                    Address2 = dr["Address2"]?.ToString(),
                    AddressTitle = dr["AddressTitle"]?.ToString(),
                    City = dr["City"]?.ToString(),
                    State = dr["State"]?.ToString(),
                    Zip = dr["Zip"]?.ToString(),
                    Country = dr["Country"]?.ToString(),
                    AccountType = dr["AccountType"]?.ToString(),
                    Status = dr["Status"]?.ToString(),
                    SessionId = dr["SessionKey"]?.ToString()
                };

                var storedPassword = dr["password"]?.ToString();

                // If password matches plain text, update to encrypted
                if (credentials.Password == storedPassword)
                {
                    await _dataLayer.RunSqlAsync($"UPDATE Account SET password='{encryptedPassword}' WHERE AccountId={dr["AccountId"]}");
                }

                if (credentials.Password == storedPassword || encryptedPassword == storedPassword)
                {
                    return Ok(account);
                }

                return Unauthorized();
            }
        }

        return Unauthorized();
    }

    /// <summary>
    /// Create a new account
    /// </summary>
    [HttpPost]
    [Route("api/account/create")]
    public async Task<IActionResult> Create([FromBody] Account account, [FromQuery] string? sid)
    {
        Account? userAccount = null;

        if (!string.IsNullOrEmpty(sid))
        {
            userAccount = await _context.GetUserBySessionIdAsync(sid);
        }

        if (userAccount != null)
        {
            account.CreateBy = $"{userAccount.LastName}, {userAccount.FirstName}";
        }
        else
        {
            account.AccountType = "Member";
            account.CreateBy = $"{account.LastName}, {account.FirstName}";
        }

        if (!string.IsNullOrEmpty(account.Password))
        {
            account.Password = CalculateMD5Hash(account.Password);
        }

        account.OfficeId = 1;
        account.Status = "Active";
        account.CreateDate = DateTime.Now;

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Get session ID
        var sql = $"strpLogin '{account.Email}';";
        var ds = await _dataLayer.GetDataAsync(sql);

        if (ds.Tables.Count > 0)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                account.SessionId = dr["SessionKey"]?.ToString();
            }
        }

        return Ok(account);
    }

    /// <summary>
    /// Update an existing account
    /// </summary>
    [HttpPut]
    [Route("api/account")]
    public async Task<IActionResult> Put([FromBody] Account account, [FromQuery] string sid)
    {
        var securityAccount = await _context.GetUserBySessionIdAsync(sid);

        if (securityAccount == null)
        {
            return Unauthorized();
        }

        var existingAccount = await _context.Accounts.FirstOrDefaultAsync(p => p.AccountId == account.AccountId);

        if (existingAccount == null)
        {
            return NotFound();
        }

        account.OfficeId = securityAccount.OfficeId; // Can't change office here
        account.Password = "NOW HANDLED BY AUTH0";

        // Only the user themselves or an Admin can update
        if (account.AccountId == securityAccount.AccountId || securityAccount.AccountType == "Admin")
        {
            _context.Entry(existingAccount).State = EntityState.Detached;
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(account);
        }

        return Unauthorized();
    }

    /// <summary>
    /// Delete an account (stub - not implemented)
    /// </summary>
    [HttpDelete]
    [Route("api/account/{id}")]
    public IActionResult Delete(int id)
    {
        // Not implemented in original
        return Ok();
    }

    private static string CalculateMD5Hash(string input)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);

        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }
}
