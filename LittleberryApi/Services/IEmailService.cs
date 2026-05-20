using LittleberryApi.Models.DTOs;

namespace LittleberryApi.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string from, string subject, string body, string? cc = null, string? bcc = null);
    Task<bool> SendSimpleMailAsync(SimpleMail mail);
}
