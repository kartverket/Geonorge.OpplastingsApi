using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
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
            //todo send email
            _logger.LogInformation("SendEmailStatusChangedToUploader");
        }

        public void SendEmailUploadedFileToContact(File file)
        {
            //todo send email
            _logger.LogInformation("SendEmailUploadedFileToContact");
        }
    }

    public interface INotificationService
    {
        void SendEmailUploadedFileToContact(File file);
        void SendEmailStatusChangedToUploader(File file);
    }
}
