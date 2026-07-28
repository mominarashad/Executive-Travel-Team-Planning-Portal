using System.Net.Mail;
using Microsoft.Extensions.Options;
using TravelManagement.API.Features.Email.Interfaces;

namespace TravelManagement.API.Features.Email.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort);
        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress, _settings.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        await client.SendMailAsync(message);
    }
}
