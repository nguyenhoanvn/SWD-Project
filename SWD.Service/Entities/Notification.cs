using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Entities
{
    public class Notification
    {
        public string NotificationId { get; set; } = Guid.NewGuid().ToString();
        public string ReceiverId { get; set; } = string.Empty;
        public string MessageContent { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
