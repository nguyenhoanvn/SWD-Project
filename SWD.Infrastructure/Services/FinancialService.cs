using Microsoft.EntityFrameworkCore;
using SWD.Application.DTOs;
using SWD.Data;
using SWD.Domain.Models;
using SWD.Interfaces;

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
        public async Task<PaymentResult> InitiatePayment(PaymentRequest paymentData)
        {
            // 1.3 authorizeSecureTransaction
            var gatewayResp = await _gateway.AuthorizeSecureTransaction(paymentData);

            // 2.1 statusUpdate — lưu Payment
            var payment = new Payment
            {
                StudentId       = paymentData.StudentId,
                Amount               = paymentData.Amount,
                PaymentDate          = DateTime.Now,
                PaymentMethod        = paymentData.PaymentMethod,
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

        public async Task<bool> UpdateStatus(string paymentId, PaymentStatus transactionStatus)
        {
            var payment = await _db.Payments
        .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment != null)
            {
                payment.Status = transactionStatus;
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
