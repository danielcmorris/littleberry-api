using System.Data;
using System.IdentityModel.Tokens.Jwt;
using LittleberryApi.Data;
using LittleberryApi.Models;
using LittleberryApi.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LittleberryApi.Services;

public class SessionService : ISessionService
{
    private readonly PfsaDbContext _context;
    private readonly IConfiguration _configuration;

    public SessionService(PfsaDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<Account?> GetUserBySessionIdAsync(string sessionId)
    {
        return await _context.GetUserBySessionIdAsync(sessionId);
    }

    public async Task<int> CheckUserAsync(string sessionId)
    {
        var connectionString = _configuration.GetConnectionString("server18");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(
            "SELECT ISNULL(AccountId,0) FROM dbo.fnSecurity_UserBySessionId(@SessionId);",
            connection);
        command.Parameters.AddWithValue("@SessionId", sessionId);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<SecurityReturnObject> MatchSessionAsync(string token)
    {
        var responseObject = new SecurityReturnObject();
        var member = await GetUserDetailsFromTokenAsync(token);

        if (member == null)
        {
            responseObject.Authorized = false;
            return responseObject;
        }

        var connectionString = _configuration.GetConnectionString("server18");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var sql = $"cmdMatchToken @Email, @FirstName, @LastName, @SessionId, @Token";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", member.Email ?? "");
        command.Parameters.AddWithValue("@FirstName", member.FirstName?.Replace("'", "''") ?? "");
        command.Parameters.AddWithValue("@LastName", member.LastName?.Replace("'", "''") ?? "");
        command.Parameters.AddWithValue("@SessionId", member.SessionID ?? "");
        command.Parameters.AddWithValue("@Token", token);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            responseObject.SessionID = reader["SessionID"]?.ToString();
            responseObject.AccountID = reader["AccountID"] != DBNull.Value ? (int)reader["AccountID"] : 0;
            responseObject.UserLevel = reader["LevelName"]?.ToString() ?? "NONE";
        }

        responseObject.Authorized = true;
        return responseObject;
    }

    public async Task<SecurityReturnObject> GetSessionIdAsync(string token, string loginType = "jwt")
    {
        var responseObject = new SecurityReturnObject();
        string email = "";

        if (loginType == "jwt")
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var payload = jwtToken.Payload;

            email = payload.ContainsKey("email") ? payload["email"]?.ToString() ?? "" : "";
        }

        var connectionString = _configuration.GetConnectionString("server18");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var escapedToken = token.Replace("'", "''");
        await using var command = new SqlCommand($"exec [UserSession] '{email}','{escapedToken}','';", connection);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            responseObject.SessionID = reader["SessionID"]?.ToString();
            responseObject.AccountID = reader["AccountID"] != DBNull.Value ? (int)reader["AccountID"] : 0;
            responseObject.UserLevel = reader["LevelName"]?.ToString() ?? "none";
        }

        responseObject.Authorized = true;
        return responseObject;
    }

    public async Task<Member?> GetUserDetailsFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var payload = jwtToken.Payload;

            return new Member
            {
                FirstName = payload.ContainsKey("given_name") ? payload["given_name"]?.ToString() : null,
                LastName = payload.ContainsKey("family_name") ? payload["family_name"]?.ToString() : null,
                Phone = payload.ContainsKey("picture") ? payload["picture"]?.ToString() : null,
                Email = payload.ContainsKey("email") ? payload["email"]?.ToString() : null,
                SessionID = payload.ContainsKey("https://mypfsa.org/session") ? payload["https://mypfsa.org/session"]?.ToString() : null
            };
        }
        catch
        {
            return null;
        }
    }
}
