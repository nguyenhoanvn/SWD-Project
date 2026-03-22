using SWD.Domain.Models;
using SWD.Interfaces;

namespace SWD.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationProxy _proxy;
        public NotificationService(INotificationProxy proxy) => _proxy = proxy;
        public async Task<bool> TriggerEnrollmentNotification(Student student, bool success)
        {
            string subject = success
                ? $"[Cambridge English Center] Xác nhận ghi danh {student.StudentName}"
                : $"[Cambridge English Center] Đăng ký {student.StudentName} không thành công";

            string body = success
                ? $"Chào {student.StudentName},\n\nBạn đã ghi danh thành công.\n\nTrân trọng."
                : $"Chào {student.StudentName},\n\nĐăng ký không thành công do thanh toán thất bại.\n\nTrân trọng.";

            // 2.4.1 sendOutboundNotice
            await _proxy.SendOutboundNotice(student.Email ?? "", subject, body);
            return success;
        }
    }
}
