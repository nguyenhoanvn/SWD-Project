using SWD.Services;

namespace SWD.Interfaces
{
    public interface IPaymentGatewayProxy
    {
        Task<GatewayResponse> AuthorizeSecureTransaction(PaymentRequest request);
    }
}
