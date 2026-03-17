using SWD.Data;
using SWD.Interfaces;
using SWD.Models;

namespace SWD.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _db;
        public AuditLogService(AppDbContext db) => _db = db;

        // UC18 msg 2.3: logActivity
        public async Task LogActivity(string userId, string transactionRef, string details)
        {
            _db.TransactionLogs.Add(new TransactionLog
            {
                UserId               = userId,
                TransactionReference = transactionRef,
                Timestamp            = DateTime.Now,
                Details              = details
            });
            await _db.SaveChangesAsync();
        }
    }
}
