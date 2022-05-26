
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace NotificationService.Email;

public class EmailService : IEmailService
{
    
    public void Send(string to, string subject, string text, string? from = null)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("sonny.boehm17@ethereal.email"));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Plain) { Text = text };
        
        using var smtp = new SmtpClient();
        smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate("sonny.boehm17@ethereal.email", "tF7QwaRkauNkuDyJTq");
        smtp.Send(email);
        smtp.Disconnect(true);

    }


}