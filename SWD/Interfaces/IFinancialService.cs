using SWD.Services;

namespace SWD.Interfaces
{
    public interface IFinancialService
    {
        Task<PaymentResult> InitiatePayment(PaymentRequest request);
    }
}
