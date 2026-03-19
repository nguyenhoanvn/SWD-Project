
using SWD.Domain.Models;

namespace SWD.Interfaces
{
    public interface INotificationService
    {
        Task<bool> TriggerEnrollmentNotification(Student student, bool success);
    }
}
