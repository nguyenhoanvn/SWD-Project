using SWD.Interfaces;
using SWD.Models;

namespace SWD.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationProxy _proxy;
        public NotificationService(INotificationProxy proxy) => _proxy = proxy;
        public async Task TriggerEnrollmentNotification(Registration reg, bool success)
        {
            string subject = success
                ? $"[Cambridge English Center] Xác nhận ghi danh lớp {reg.Class?.ClassName}"
                : $"[Cambridge English Center] Đăng ký lớp {reg.Class?.ClassName} không thành công";

            string body = success
                ? $"Chào {reg.Student?.StudentName},\n\nBạn đã ghi danh thành công vào lớp {reg.Class?.ClassName}.\nMã đăng ký: {reg.RegistrationId}\n\nTrân trọng."
                : $"Chào {reg.Student?.StudentName},\n\nĐăng ký lớp {reg.Class?.ClassName} không thành công do thanh toán thất bại.\n\nTrân trọng.";

            // 2.4.1 sendOutboundNotice
            await _proxy.SendOutboundNotice(reg.Student?.Email ?? "", subject, body);
        }
    }
}
