using SWD.Service.DTOs;
using SWD.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Wrappers
{
    public class NotificationWrapper
    {
        private readonly CECMSContext _db = CECMSContext.Instance;

        public SaveResult Save(string receiverId, string content)
        {
            _db.Notifications.Add(new Notification
            {
                NotificationId = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                ReceiverId = receiverId,
                MessageContent = content,
                CreatedDate = DateTime.UtcNow
            });
            return new SaveResult(true, "Notification saved.");
        }

        public List<Notification> ReadByReceiver(string receiverId)
            => _db.Notifications.Where(n => n.ReceiverId == receiverId).ToList();
    }
}
