
using SWD.Application.DTOs;
using SWD.Domain.Models;

namespace SWD.Interfaces
{
    public interface IFinancialService
    {
        Task<PaymentResult> InitiatePayment(PaymentRequest paymentData);
        Task<bool> UpdateStatus(string paymentId, PaymentStatus transactionStatus);
    }
}
