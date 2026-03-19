using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Application.DTOs
{
    public record PaymentRequest(string StudentId, string ClassId, string PaymentMethod, decimal Amount);
}
