using Microsoft.EntityFrameworkCore;
using SWD.Data;
using SWD.Interfaces;
using SWD.Models;

namespace SWD.Services
{
    public class EnrollmentCoordinator : IEnrollmentCoordinator
    {
        private readonly IAcademicService     _academic;
        private readonly IFinancialService    _financial;
        private readonly IAuditLogService     _audit;
        private readonly INotificationService _notification;
        private readonly AppDbContext         _db;

        public EnrollmentCoordinator(
            IAcademicService     academic,
            IFinancialService    financial,
            IAuditLogService     audit,
            INotificationService notification,
            AppDbContext         db)
        {
            _academic     = academic;
            _financial    = financial;
            _audit        = audit;
            _notification = notification;
            _db           = db;
        }

        // UC02 msg 1.1 → 1.11
        public async Task<EnrollmentResult> RequestEnrollment(string studentId, string classId)
        {
            // 1.2 validatePrerequisite
            var prereq = await _academic.ValidatePrerequisite(studentId, classId);
            if (!prereq)
                return new EnrollmentResult(false, "Bạn chưa đạt điều kiện tiên quyết để đăng ký khóa học này.");

            // 1.9 validateSchedule
            var schedule = await _academic.ValidateSchedule(studentId, classId);
            if (!schedule)
                return new EnrollmentResult(false, "Lịch học bị trùng với lớp bạn đã đăng ký.");

            // 1.10 validateCapacity
            var capacity = await _academic.ValidateCapacity(classId);
            if (!capacity)
                return new EnrollmentResult(false, "Lớp học đã đầy, không còn chỗ trống.");

            // 1.11 Valid → tạo Registration Pending
            var cls = await _db.Classes.Include(c => c.Course).FirstAsync(c => c.ClassId == classId);
            var reg = new Registration
            {
                StudentId        = studentId,
                ClassId          = classId,
                RegistrationDate = DateTime.Now,
                Status           = RegistrationStatus.Pending
            };
            _db.Registrations.Add(reg);
            await _db.SaveChangesAsync();

            await _audit.LogActivity(studentId, "", $"Tạo đơn đăng ký {reg.RegistrationId} lớp {cls.ClassName}");

            // 1.12 chuyển sang UC18
            return new EnrollmentResult(true, "Đơn đăng ký đã được tạo. Vui lòng hoàn tất thanh toán.", reg.RegistrationId);
        }

        // UC18 msg 2.2 processTransactionResult
        public async Task<EnrollmentResult> ProcessTransactionResult(string registrationId, PaymentResult paymentResult)
        {
            // 2.3 logActivity
            await _audit.LogActivity("", paymentResult.TransactionRef,
                $"Giao dịch {paymentResult.TransactionRef}: {(paymentResult.IsSuccess ? "Thành công" : "Thất bại")}");

            var reg = await _db.Registrations
                .Include(r => r.Class).ThenInclude(c => c.Course)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(r => r.RegistrationId == registrationId);

            if (reg == null)
                return new EnrollmentResult(false, "Không tìm thấy đơn đăng ký.");

            if (paymentResult.IsSuccess)
            {
                reg.Status = RegistrationStatus.Paid;
                await _db.SaveChangesAsync();

                // 2.4 triggerEnrollmentNotification
                await _notification.TriggerEnrollmentNotification(reg, true);

                return new EnrollmentResult(true,
                    $"Đăng ký thành công! Bạn đã ghi danh vào lớp {reg.Class.ClassName}.",
                    registrationId);
            }
            else
            {
                reg.Status = RegistrationStatus.Cancelled;
                await _db.SaveChangesAsync();

                await _notification.TriggerEnrollmentNotification(reg, false);

                return new EnrollmentResult(false,
                    $"Thanh toán thất bại: {paymentResult.Message}. Đơn đăng ký đã bị hủy.");
            }
        }
    }
}
