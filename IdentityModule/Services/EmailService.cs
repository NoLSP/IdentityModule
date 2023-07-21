using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System;

namespace IdentityModule.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
            using var emailMessage = new MimeMessage();
 
            emailMessage.From.Add(new MailboxAddress("IdentityModule", "borovinskiyartem@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
             
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 25, false);
                await client.AuthenticateAsync("borovinskiyartem@yandex.ru", "artem2000");
                await client.SendAsync(emailMessage);
 
                await client.DisconnectAsync(true);
            }
            }
            catch(Exception e)
            {
                var here = "";
            }
        }
    }
}