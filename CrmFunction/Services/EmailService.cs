using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CrmFunction.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public Task SendNewCustomerNotificationAsync(string sellerEmail, string sellerName,
        string customerName, string customerEmail, string customerPhone,
        string customerAddress, string customerTitle)
    {
        var subject = $"Ny kund tilldelad: {customerName}";
        var body = $@"Hej {sellerName},

Du har blivit utsedd som ansvarig säljare för följande NYA kund:

Namn:     {customerName}
Titel:    {customerTitle}
E-post:   {customerEmail}
Telefon:  {customerPhone}
Adress:   {customerAddress}

Vänligen ta kontakt med kunden så snart som möjligt.

Med vänliga hälsningar,
CRM-systemet";

        return SendAsync(sellerEmail, subject, body);
    }

    public Task SendUpdatedCustomerNotificationAsync(string sellerEmail, string sellerName,
        string customerName, string customerEmail, string customerPhone,
        string customerAddress, string customerTitle)
    {
        var subject = $"Kund uppdaterad: {customerName}";
        var body = $@"Hej {sellerName},

Uppgifterna för din kund har uppdaterats:

Namn:     {customerName}
Titel:    {customerTitle}
E-post:   {customerEmail}
Telefon:  {customerPhone}
Adress:   {customerAddress}

Med vänliga hälsningar,
CRM-systemet";

        return SendAsync(sellerEmail, subject, body);
    }

    private async Task SendAsync(string to, string subject, string body)
    {
        var smtpHost = _config["Email:SmtpHost"];
        var smtpPort = _config["Email:SmtpPort"];
        var smtpUser = _config["Email:Username"];
        var smtpPass = _config["Email:Password"];
        var fromEmail = _config["Email:From"] ?? "crm@example.com";

        if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpUser))
        {
            _logger.LogInformation("=== EMAIL (console fallback) ===");
            _logger.LogInformation("Till: {To}", to);
            _logger.LogInformation("Ämne: {Subject}", subject);
            _logger.LogInformation("{Body}", body);
            _logger.LogInformation("================================");
            return;
        }

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(fromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(smtpHost, int.Parse(smtpPort ?? "587"), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(smtpUser, smtpPass);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);

        _logger.LogInformation("E-post skickad till {Email}, ämne: {Subject}", to, subject);
    }
}
