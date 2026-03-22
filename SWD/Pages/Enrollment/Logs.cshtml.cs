using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWD.Service;
using SWD.Service.Entities;

namespace SWD.Web.Pages.Enrollment
{
    public class LogsModel : PageModel
    {
        private readonly CECMSContext _db = CECMSContext.Instance;

        public List<TransactionLog> Logs { get; private set; } = [];
        public List<Payment> Payments { get; private set; } = [];
        public List<Notification> Notifications { get; private set; } = [];

        public void OnGet()
        {
            Logs = _db.TransactionLogs.OrderByDescending(l => l.Timestamp).ToList();
            Payments = _db.Payments.OrderByDescending(p => p.PaymentDate).ToList();
            Notifications = _db.Notifications.OrderByDescending(n => n.CreatedDate).ToList();
        }
    }
}
