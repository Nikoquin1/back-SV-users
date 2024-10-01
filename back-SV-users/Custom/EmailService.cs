using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendPasswordChangedConfirmation(string toEmail)
    {
        var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
        {
            Port = int.Parse(_configuration["Smtp:Port"]),
            Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
            EnableSsl = bool.Parse(_configuration["Smtp:EnableSsl"])
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Smtp:Username"]),
            Subject = "Password Changed",
            Body = "Your password has been successfully changed.",
            IsBodyHtml = true,
        };
        mailMessage.To.Add(toEmail);

        smtpClient.Send(mailMessage);
    }
    public async Task SendRecoveryCode(string toEmail, string code)
    {
        var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
        {
            Port = int.Parse(_configuration["Smtp:Port"]),
            Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
            EnableSsl = bool.Parse(_configuration["Smtp:EnableSsl"])
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Smtp:Username"]),
            Subject = "Password Recovery Code",
            Body = $"Your password recovery code is: {code}",
            IsBodyHtml = true,
        };
        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage);
    }

}
