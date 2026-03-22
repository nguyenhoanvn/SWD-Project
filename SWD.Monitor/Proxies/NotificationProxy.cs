
using SWD.Service;
using SWD.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Monitor.Proxies
{
    public record NotificationResult(bool Sent, string Message);
    public record NotificationProcessResult(bool Sent, string Message);

    public class NotificationProxy
    {
        private readonly CECMSContext _db = CECMSContext.Instance;

        public NotificationProcessResult TriggerEnrollmentNotification(
        string receiverId,
        string content)
        {
            // msg 2.12: Send Outbound Notice via proxy
            var result = SendOutboundNotice(receiverId, content);

            // msg 2.16: Sending Result → EnrollmentCoordinator
            return new NotificationProcessResult(result.Sent, result.Message);
        }

        public NotificationResult SendOutboundNotice(
            string receiverId,
            string messageContent)
        {
            // msg 2.12: Gateway Request → Notification System (simulated)
            // msg 2.13: Notification System returns result (always success in demo)
            bool sent = true;
            string gatewayMessage = "Notification delivered by system.";

            if (sent)
            {
                // msg 2.14: Record Notification in entity
                _db.Notifications.Add(new Notification
                {
                    NotificationId = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                    ReceiverId = receiverId,
                    MessageContent = messageContent,
                    CreatedDate = DateTime.UtcNow
                });
            }

            // msg 2.15: Save Result (Notification → NotificationProxy)
            return new NotificationResult(sent, gatewayMessage);
        }
    }
}
