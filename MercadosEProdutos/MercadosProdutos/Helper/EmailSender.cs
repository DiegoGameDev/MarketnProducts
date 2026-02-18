using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Helper;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential("diegosantanacontato2@gmail.com", "spjb xzor slnb eqmd"),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress("diegosantanacontato2@gmail.com"),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        message.To.Add(email);

        await smtp.SendMailAsync(message);
    }
}