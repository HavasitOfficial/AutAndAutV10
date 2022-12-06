using AutAndAutV10.Models;
using AutAndAutV10.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace AutAndAutV10.Services
{
    public class SendEmailService : ISendEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ILogService _logService;
        private readonly IConfiguration _configuration;

        public SendEmailService(EmailConfiguration emailConfiguration, ILogService logService, IConfiguration configuration)
        {
            _emailConfiguration = emailConfiguration;
            _logService = logService;
            _configuration = configuration;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(string.Empty, _emailConfiguration.From));
            emailMessage.To.Add(message.EmailTo);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.MessageTemplate };
            return emailMessage;

        }
        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                var AuthMechanisms = _configuration.GetValue<string>("AutAndAutOptions:AuthMechanisms");
                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, false);
                client.AuthenticationMechanisms.Remove(AuthMechanisms);
                client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.AppPassword);
                client.Send(mailMessage);
            }
            catch(Exception ex)
            {
                _logService.Error(ex, ex.Message);
            }
        }
    }
}
