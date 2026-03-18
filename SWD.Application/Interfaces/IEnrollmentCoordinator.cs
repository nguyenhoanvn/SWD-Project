using SWD.Application.DTOs;

namespace SWD.Interfaces
{
    public interface IEnrollmentCoordinator
    {
        Task<EnrollmentResult> RequestEnrollment(string studentId, string classId);
        Task<EnrollmentResult> ProcessTransactionResult(string registrationId, PaymentResult paymentResult);
    }
}
