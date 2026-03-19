using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Domain.Models
{
    public enum PaymentStatus { Pending = 0, Success = 1, Failed = 2 }

    public class Payment
    {
        [Key] public string PaymentId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Student")] public string StudentId { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = "";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string TransactionReference { get; set; } = "";

        public Student Student { get; set; } = null!;
    }
}
