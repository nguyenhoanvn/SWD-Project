using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.DTOs
{
    public record PaymentInfo(
        string RegistrationId,
        decimal Amount,
        string PaymentMethod,
        string TransactionReference
    );
}
