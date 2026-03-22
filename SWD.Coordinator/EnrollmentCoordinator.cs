using SWD.Monitor.BusinessLogics;
using SWD.Monitor.Proxies;
using SWD.Service.DTOs;
using SWD.Service.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Coordinator
{
    public record EnrollmentRequestResult(
        bool Success,
        string Message,
        string? RegistrationId = null,
        string? TransactionReference = null
    );

    public record PaymentTransactionResult(
        bool Success,
        string Message,
        string? TransactionReference = null
    );

    public class EnrollmentCoordinator
    {
        private readonly EnrollmentManager _enrollmentManager = new();
        private readonly PaymentProxy _paymentProxy = new();
        private readonly NotificationProxy _notificationProxy = new();
        private readonly TransactionLogWrapper _transactionLogWrapper = new();
        private readonly RegistrationWrapper _registrationWrapper = new();

        public EnrollmentRequestResult SendEnrollmentRequest(string studentId, string classId)
        {
            // msg 1.2: Validate Enrollment Request → EnrollmentManager
            var validationResult = _enrollmentManager.EnrollRequest(studentId, classId);

            if (!validationResult.Valid)
            {
                // msg 1.9A / 1.11A / 1.13A: Invalid Prerequisite / Schedule / Capacity
                // msg 1.9A.1 / 1.11A.1 / 1.13A.1: Display Error to StudentInterface
                return new EnrollmentRequestResult(false, validationResult.Message);
            }

            // msg 1.13: Enrollment Conditions Evaluated — OK
            // msg 1.14: Display Payment Form
            return new EnrollmentRequestResult(true, "Enrollment validated. Please proceed to payment.", null);
        }

        public PaymentTransactionResult RequestPaymentTransaction(
            string studentId,
            string classId,
            string paymentMethod,
            bool simulateSuccess)
        {
            var transactionRef = $"TXN-{Guid.NewGuid():N}"[..12].ToUpper();
            decimal amount = 5_000_000m; // 5,000,000 VND demo tuition

            var paymentInfo = new PaymentInfo(
                RegistrationId: "",   // will update after creation
                Amount: amount,
                PaymentMethod: paymentMethod,
                TransactionReference: transactionRef
            );

            // msg 2.2: Initiate Payment → FinancialService
            var paymentResult = _paymentProxy.InitiatePayment(paymentInfo, simulateSuccess);

            if (!paymentResult.Success)
            {
                // msg 2.5A.3 / 2.6A.4: Log failure
                _transactionLogWrapper.LogActivity(new LogData(
                    UserId: studentId,
                    TransactionReference: transactionRef,
                    Details: $"Payment FAILED for class {classId}. Reason: {paymentResult.Message}"
                ));

                // msg 2.5A.4 / 2.6A.5: Display Error to StudentInterface
                return new PaymentTransactionResult(false, paymentResult.Message, transactionRef);
            }

            // msg 2.8: Log Activity (success)
            _transactionLogWrapper.LogActivity(new LogData(
                UserId: studentId,
                TransactionReference: transactionRef,
                Details: $"Payment PAID for class {classId}."
            ));

            // msg 2.9: Create → Registration
            var registrationId = _registrationWrapper.Create(studentId, classId);

            // msg 2.10: Enrolling Class Result confirmed

            // msg 2.11: Trigger Enrollment Notification
            var notifyResult = _notificationProxy.TriggerEnrollmentNotification(
                receiverId: studentId,
                content: $"You have successfully enrolled in class {classId}. Registration ID: {registrationId}. Transaction: {transactionRef}."
            );

            // msg 2.16 / 2.17: Display Enrollment Result
            return new PaymentTransactionResult(
                true,
                $"Enrollment complete! Registration ID: {registrationId}. Notification sent: {notifyResult.Sent}",
                transactionRef
            );
        }
    }
}
