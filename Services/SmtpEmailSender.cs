using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using szpont.Services;

namespace szpont.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;

        public SmtpEmailSender(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient();

            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_settings.User, _settings.Password);

            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("Szpont App", _settings.User));
            message.To.Add(MimeKit.MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new MimeKit.BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}