namespace Geonorge.OpplastingsApi.Services
{
    public class NotificationConfiguration
    {
        public static string SectionName => "Notification";
        public string SmtpHost { get; set; }
        public string WebmasterEmail { get; set; }
    }
}
