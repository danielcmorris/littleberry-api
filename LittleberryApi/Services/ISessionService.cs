using LittleberryApi.Models;
using LittleberryApi.Models.DTOs;

namespace LittleberryApi.Services;

public interface ISessionService
{
    Task<Account?> GetUserBySessionIdAsync(string sessionId);
    Task<int> CheckUserAsync(string sessionId);
    Task<SecurityReturnObject> MatchSessionAsync(string token);
    Task<SecurityReturnObject> GetSessionIdAsync(string token, string loginType = "jwt");
    Task<Member?> GetUserDetailsFromTokenAsync(string token);
}
