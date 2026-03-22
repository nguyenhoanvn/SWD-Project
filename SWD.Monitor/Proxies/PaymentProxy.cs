using SWD.Service.DTOs;
using SWD.Service.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Monitor.Proxies
{
    public record PaymentOperationResult(bool Success, string Message, string TransactionReference);
    public enum GatewayTransactionStatus { Success, Failure }

    public record GatewayResponse(
        GatewayTransactionStatus Status,
        string TransactionReference,
        string Message
    );
    public class PaymentProxy
    {
        private readonly PaymentWrapper _paymentWrapper = new();

        public PaymentOperationResult InitiatePayment(
            PaymentInfo paymentData,
            bool simulateSuccess)
        {
            // msg 2.3: Record Payment Request — persist PENDING
            var saveResult = _paymentWrapper.PersistFinancialData(paymentData);
            if (!saveResult.Success)
                return new PaymentOperationResult(false, "Failed to persist payment.", paymentData.TransactionReference);

            // msg 2.4: Save Result confirmed

            // msg 2.5: Gateway Request via proxy
            var gatewayResponse = AuthorizeSecureTransaction(paymentData, simulateSuccess);

            if (gatewayResponse.Status == GatewayTransactionStatus.Success)
            {
                // msg 2.7: Update Transaction Status to "Paid"
                _paymentWrapper.UpdateStatus(paymentData.TransactionReference, 1);

                // msg 2.8: Save Result confirmed
                return new PaymentOperationResult(true, "Payment successful.", gatewayResponse.TransactionReference);
            }
            else
            {
                // msg 2.6A.1: Update Transaction Status to "Fail"
                _paymentWrapper.UpdateStatus(paymentData.TransactionReference, -1);

                // msg 2.6A.2: Save Result confirmed
                return new PaymentOperationResult(false, gatewayResponse.Message, gatewayResponse.TransactionReference);
            }
        }

        public GatewayResponse AuthorizeSecureTransaction(
           PaymentInfo paymentData,
           bool simulateSuccess)
        {
            // msg 2.5 — Gateway Request → Payment Gateway
            if (simulateSuccess)
            {
                // msg 2.6 — Transaction Success Status
                return new GatewayResponse(
                    GatewayTransactionStatus.Success,
                    paymentData.TransactionReference,
                    "Payment authorised by gateway."
                );
            }
            else
            {
                // msg 2.6A — Transaction Failure Status
                return new GatewayResponse(
                    GatewayTransactionStatus.Failure,
                    paymentData.TransactionReference,
                    "Payment declined by gateway."
                );
            }
        }
    }
}
