using LittleberryApi.Models.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LittleberryApi.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string from, string subject, string body, string? cc = null, string? bcc = null)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            if (!string.IsNullOrEmpty(cc))
            {
                message.Cc.Add(MailboxAddress.Parse(cc));
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                message.Bcc.Add(MailboxAddress.Parse(bcc));
            }

            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();

            var server = _configuration["AWS:SES:Server"];
            var port = _configuration.GetValue<int>("AWS:SES:Port");
            var user = _configuration["AWS:SES:User"];
            var password = _configuration["AWS:SES:Password"];

            await client.ConnectAsync(server, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(user, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return false;
        }
    }

    public async Task<bool> SendSimpleMailAsync(SimpleMail mail)
    {
        return await SendEmailAsync(
            mail.To ?? "",
            mail.From ?? "noreply@mypfsa.org",
            mail.Subject ?? "",
            mail.Message ?? "",
            mail.Cc,
            mail.Bcc);
    }
}
