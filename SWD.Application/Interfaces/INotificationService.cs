
using SWD.Domain.Models;

namespace SWD.Interfaces
{
    public interface INotificationService
    {
        Task<bool> TriggerEnrollmentNotification(Registration reg, bool success);
    }
}
