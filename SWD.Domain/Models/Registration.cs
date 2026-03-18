
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Domain.Models
{
    public enum RegistrationStatus { Pending = 0, Paid = 1, Cancelled = 2 }

    public class Registration
    {
        [Key] public string RegistrationId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Student")] public string StudentId { get; set; } = "";
        [ForeignKey("Class")] public string ClassId { get; set; } = "";
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

        // Navigation
        public Student Student { get; set; } = null!;
        public Class Class { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
