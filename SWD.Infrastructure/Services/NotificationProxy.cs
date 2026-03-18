using Microsoft.Extensions.Logging;
using SWD.Interfaces;

namespace SWD.Services
{
    // «proxy» — giả lập Email/SMS gateway
    // Thực tế: gọi SendGrid / SMTP / Twilio
    public class NotificationProxy : INotificationProxy
    {
        private readonly ILogger<NotificationProxy> _logger;
        public NotificationProxy(ILogger<NotificationProxy> logger) => _logger = logger;

        // msg 2.4.1 sendOutboundNotice
        public Task SendOutboundNotice(string to, string subject, string body)
        {
            _logger.LogInformation("[EMAIL] To: {To} | Subject: {Subject}", to, subject);
            return Task.CompletedTask;
        }
    }
}
