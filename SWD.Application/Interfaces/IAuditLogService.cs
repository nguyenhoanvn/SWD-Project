namespace SWD.Interfaces
{
    public interface IAuditLogService
    {
        Task LogActivity(string userId, string transactionRef, string details);
    }
}
