using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using File = Geonorge.OpplastingsApi.Models.Entity.File;

namespace Geonorge.OpplastingsApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationConfiguration _config;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
        IOptions<NotificationConfiguration> options,
        ILogger<NotificationService> logger)
        {
            _config = options.Value;
            _logger = logger;
        }

        public void SendEmailStatusChangedToUploader(File file)
        {
            try
            {
                string subject = "Ny status fil";
                string body = $"Filen {file.FileName} har fått status {file.Status}";
                SendEmail(file.UploaderEmail, file.Dataset.ContactEmail, null, subject, body);
                _logger.LogInformation("SendEmailStatusChangedToUploader");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void SendEmailUploadedFileToContact(File file)
        {
            try
            {
                string subject = "Ny opplastet fil";
                string body = $"Filen {file.FileName} er lastet opp av {file.UploaderPerson}";
                SendEmail(file.UploaderEmail, file.Dataset.ContactEmail,file.Dataset.ContactEmailExtra, subject, body);
                _logger.LogInformation("SendEmailUploadedFileToContact");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

        }

        public void SendEmail(string fromEmail, string toEmail, string? toEmailExtra, string subject, string messageBody) 
        {
            try
            {
                MimeMessage message = new MimeMessage();
                MailboxAddress from = MailboxAddress.Parse(fromEmail);
                message.From.Add(from);

                MailboxAddress to = MailboxAddress.Parse(toEmail);
                message.To.Add(to);

                if (!string.IsNullOrEmpty(toEmailExtra)) 
                { 
                    MailboxAddress toExtra = MailboxAddress.Parse(toEmailExtra);
                    message.To.Add(toExtra);
                }

                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = messageBody;

                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new SmtpClient();
                client.Connect(_config.SmtpHost);

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();

            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }

    public interface INotificationService
    {
        void SendEmailUploadedFileToContact(File file);
        void SendEmailStatusChangedToUploader(File file);
    }
}
