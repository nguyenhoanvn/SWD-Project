using SWD.Data;
using SWD.Interfaces;
using SWD.Models;

namespace SWD.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IPaymentGatewayProxy _gateway;
        private readonly AppDbContext         _db;

        public FinancialService(IPaymentGatewayProxy gateway, AppDbContext db)
        {
            _gateway = gateway;
            _db      = db;
        }

        // UC18 msg 1.2: initiatePayment
        public async Task<PaymentResult> InitiatePayment(PaymentRequest request)
        {
            // 1.3 authorizeSecureTransaction
            var gatewayResp = await _gateway.AuthorizeSecureTransaction(request);

            // 2.1 statusUpdate — lưu Payment
            var payment = new Payment
            {
                RegistrationId       = request.RegistrationId,
                Amount               = request.Amount,
                PaymentDate          = DateTime.Now,
                PaymentMethod        = request.PaymentMethod,
                Status               = gatewayResp.Authorized ? PaymentStatus.Success : PaymentStatus.Failed,
                TransactionReference = gatewayResp.TransactionRef
            };
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            return new PaymentResult(
                gatewayResp.Authorized,
                gatewayResp.Authorized ? "Giao dịch thành công" : "Cổng thanh toán từ chối giao dịch",
                gatewayResp.TransactionRef
            );
        }
    }
}
