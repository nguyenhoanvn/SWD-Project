using SWD.Service.DTOs;
using SWD.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Wrappers
{
    public class TransactionLogWrapper
    {
        private readonly CECMSContext _db = CECMSContext.Instance;

        /// <summary>logActivity(in logData, out saveResult)</summary>
        public SaveResult LogActivity(LogData logData)
        {
            _db.TransactionLogs.Add(new TransactionLog
            {
                LogId = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                UserId = logData.UserId,
                TransactionReference = logData.TransactionReference,
                Timestamp = DateTime.UtcNow,
                Details = logData.Details
            });
            return new SaveResult(true, "Activity logged.");
        }

        public List<TransactionLog> ReadAll() => _db.TransactionLogs;
    }
}
