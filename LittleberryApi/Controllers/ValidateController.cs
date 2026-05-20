using System.Text;
using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LittleberryApi.Controllers;

public class NewSession
{
    public int Id { get; set; }
    public string? SessionID { get; set; }
    public DateTime CreateDate { get; set; }
}

[ApiController]
public class ValidateController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly IDataLayerService _dataLayer;

    public ValidateController(ISessionService sessionService, IDataLayerService dataLayer)
    {
        _sessionService = sessionService;
        _dataLayer = dataLayer;
    }

    /// <summary>
    /// Create a new session
    /// </summary>
    [HttpGet]
    [Route("api/auth/new-session")]
    public async Task<IActionResult> GetNewSession([FromQuery] string key = "")
    {
        if (key != "Pe0BHFk2x2yNUnTcy6TFFpIQkwuN3i4WmI3i6xVgT8eo5njzLsNVxZtbLxk6x5lbfc5MV7h32enhNBRU5UwvYzPUrow9ZBjFL1J")
        {
            return Unauthorized();
        }

        var sessionId = RandomString(20);
        var session = new NewSession
        {
            SessionID = sessionId,
            CreateDate = DateTime.Now
        };

        var sql = $"INSERT INTO Sessions(SessionKey,SessionDate) VALUES ('{sessionId}','{session.CreateDate:yyyy-MM-dd HH:mm:ss}');SELECT @@IDENTITY;";
        var id = await _dataLayer.RunSqlAsync(sql);
        session.Id = id;

        return Ok(session);
    }

    /// <summary>
    /// Match session with token
    /// </summary>
    [HttpPost]
    [Route("api/auth/match-session")]
    public async Task<IActionResult> MatchSession([FromBody] JObject obj)
    {
        var token = obj["token"]?.ToString() ?? "";
        var result = await _sessionService.MatchSessionAsync(token);
        return Ok(result);
    }

    private static string RandomString(int size, bool lowerCase = true)
    {
        var builder = new StringBuilder(size);
        var random = new Random();
        char offset = lowerCase ? 'a' : 'A';
        const int lettersOffset = 26;

        for (var i = 0; i < size; i++)
        {
            var c = (char)random.Next(offset, offset + lettersOffset);
            builder.Append(c);
        }

        return lowerCase ? builder.ToString().ToLower() : builder.ToString();
    }
}
