
using SWD.Application.DTOs;

namespace SWD.Interfaces
{
    public interface IPaymentGatewayProxy
    {
        Task<GatewayResponse> AuthorizeSecureTransaction(PaymentRequest request);
    }
}
