using SWD.Interfaces;

namespace SWD.Services
{
    // «proxy» — giả lập MoMo / VNPay
    // Thực tế: gọi HTTP đến endpoint của cổng thanh toán
    public class PaymentGatewayProxy : IPaymentGatewayProxy
    {
        public Task<GatewayResponse> AuthorizeSecureTransaction(PaymentRequest request)
        {
            if (request.PaymentMethod == "TestFail")
            {
                string failRef = $"TXN_FAIL_{DateTime.Now:yyyyMMddHHmmss}";
                return Task.FromResult(new GatewayResponse(false, failRef, "99"));
            }

            bool authorized = request.Amount > 0 &&
                              new[] { "MoMo", "VNPay", "ZaloPay" }.Contains(request.PaymentMethod);

            string txnRef = $"TXN_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString()[..8].ToUpper()}";

            return Task.FromResult(new GatewayResponse(
                authorized,
                txnRef,
                authorized ? "00" : "99"
            ));
        }
    }
}
