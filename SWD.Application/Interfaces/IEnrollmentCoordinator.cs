using SWD.Application.DTOs;

namespace SWD.Interfaces
{
    public interface IEnrollmentCoordinator
    {
        Task<EnrollmentResult> SendEnrollmentRequest(string studentId, string classId);
        Task<PaymentResult> SendPaymentData(string studentId, string courseId);
        Task<EnrollmentResult> ProcessTransactionResult(string studentId, PaymentResult paymentResult);
    }
}
