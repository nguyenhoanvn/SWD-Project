using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Entities
{
    public class Payment
    {
        public string PaymentId { get; set; } = Guid.NewGuid().ToString();
        public string RegistrationId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = string.Empty;
        public int Status { get; set; } = 0;    // 0=PENDING, 1=PAID, -1=FAILED
        public string TransactionReference { get; set; } = string.Empty;
    }
}
