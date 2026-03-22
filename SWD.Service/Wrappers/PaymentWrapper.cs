using SWD.Service.DTOs;
using SWD.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Wrappers
{
    public class PaymentWrapper
    {
        private readonly CECMSContext _db = CECMSContext.Instance;

        /// <summary>persistFinancialData(in paymentInfo, out saveResult)</summary>
        public SaveResult PersistFinancialData(PaymentInfo paymentInfo)
        {
            try
            {
                _db.Payments.Add(new Payment
                {
                    PaymentId = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                    RegistrationId = paymentInfo.RegistrationId,
                    Amount = paymentInfo.Amount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = paymentInfo.PaymentMethod,
                    Status = 0, // PENDING initially
                    TransactionReference = paymentInfo.TransactionReference
                });
                return new SaveResult(true, "Payment record saved.");
            }
            catch (Exception ex)
            {
                return new SaveResult(false, ex.Message);
            }
        }

        public void UpdateStatus(string transactionReference, int status)
        {
            var p = _db.Payments.FirstOrDefault(x => x.TransactionReference == transactionReference);
            if (p != null) p.Status = status;
        }

        public List<Payment> ReadAll() => _db.Payments;
    }
}
