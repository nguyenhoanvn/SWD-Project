using SWD.Models;

namespace SWD.Interfaces
{
    public interface INotificationService
    {
        Task TriggerEnrollmentNotification(Registration reg, bool success);
    }
}
