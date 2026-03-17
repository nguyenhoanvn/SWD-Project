namespace SWD.Services
{
    public record EnrollmentRequest(string StudentId, string ClassId);
    public record EnrollmentResult(bool IsSuccess, string Message, string? RegistrationId = null);
    public record PaymentRequest(string RegistrationId, string PaymentMethod, decimal Amount);
    public record PaymentResult(bool IsSuccess, string Message, string TransactionRef);
    public record GatewayResponse(bool Authorized, string TransactionRef, string RawStatus);
}
