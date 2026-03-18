using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Domain.Models
{
    public class TransactionLog
    {
        [Key] public string LogId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = "";
        public string TransactionReference { get; set; } = ""; 
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Details { get; set; } = "";
    }
}
