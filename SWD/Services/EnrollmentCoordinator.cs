using Microsoft.EntityFrameworkCore;
using SWD.Data;
using SWD.Interfaces;
using SWD.Models;

namespace SWD.Services
{
    // «coordinator» EnrollmentCoordinator
    public class EnrollmentCoordinator : IEnrollmentCoordinator
    {
        private readonly IEnrollmentManager   _manager;       
        private readonly IFinancialService    _financial;
        private readonly IAuditLogService     _audit;
        private readonly INotificationService _notification;
        private readonly AppDbContext         _db;

        public EnrollmentCoordinator(
            IEnrollmentManager   manager,
            IFinancialService    financial,
            IAuditLogService     audit,
            INotificationService notification,
            AppDbContext         db)
        {
            _manager      = manager;
            _financial    = financial;
            _audit        = audit;
            _notification = notification;
            _db           = db;
        }


        public async Task<EnrollmentResult> RequestEnrollment(string studentId, string classId)
        {
            var cls = await _db.Classes.Include(c => c.Course)
                                       .Include(c => c.Schedule)
                                       .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (!await CheckPrerequisite(studentId, classId))
                return new EnrollmentResult(false, "Bạn chưa đạt điều kiện tiên quyết để đăng ký khóa học này.");

            if (!await CheckSchedule(studentId, classId))
                return new EnrollmentResult(false, "Lịch học bị trùng với lớp bạn đã đăng ký.");

            if (!await CheckCapacity(classId))
                return new EnrollmentResult(false, "Lớp học đã đầy, không còn chỗ trống.");

            var valid = await _manager.EnrollRequest(studentId, classId);
            if (!valid)
                return new EnrollmentResult(false, "Không thể ghi danh, vui lòng thử lại.");

            var reg = new Registration
            {
                StudentId        = studentId,
                ClassId          = classId,
                RegistrationDate = DateTime.Now,
                Status           = RegistrationStatus.Pending
            };
            _db.Registrations.Add(reg);
            await _db.SaveChangesAsync();

            await _audit.LogActivity(studentId, "",
                $"Tạo đơn đăng ký {reg.RegistrationId} lớp {cls?.ClassName}");
            return new EnrollmentResult(true,
                "Đơn đăng ký đã được tạo. Vui lòng hoàn tất thanh toán.",
                reg.RegistrationId);
        }
        public async Task<EnrollmentResult> ProcessTransactionResult(string registrationId, PaymentResult paymentResult)
        {

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

        // ── Helper: tách riêng để lấy message lỗi cụ thể ──────

        private async Task<bool> CheckPrerequisite(string studentId, string classId)
        {
            var cls = await _db.Classes.Include(c => c.Course)
                                       .FirstOrDefaultAsync(c => c.ClassId == classId);
            if (cls == null) return false;
            if (cls.Course.ScoreCondition <= 0) return true;

            var best = await _db.Scores
                .Where(s => s.StudentId == studentId)
                .MaxAsync(s => (double?)s.ScoreValue) ?? 0;

            return best >= cls.Course.ScoreCondition;
        }

        private async Task<bool> CheckSchedule(string studentId, string classId)
        {
            var target = await _db.Classes.Include(c => c.Schedule)
                                          .FirstOrDefaultAsync(c => c.ClassId == classId);
            if (target == null) return false;

            var enrolled = await _db.Registrations
                .Where(r => r.StudentId == studentId &&
                            (r.Status == RegistrationStatus.Pending ||
                             r.Status == RegistrationStatus.Paid))
                .Include(r => r.Class).ThenInclude(c => c.Schedule)
                .Select(r => r.Class)
                .ToListAsync();

            return !enrolled.Any(ec =>
                ec.Schedule.DayOfWeek   == target.Schedule.DayOfWeek &&
                ec.Schedule.StartTime    < target.Schedule.EndTime   &&
                target.Schedule.StartTime < ec.Schedule.EndTime);
        }

        private async Task<bool> CheckCapacity(string classId)
        {
            var cls = await _db.Classes.FirstOrDefaultAsync(c => c.ClassId == classId);
            if (cls == null) return false;

            int count = await _db.Registrations
                .CountAsync(r => r.ClassId == classId &&
                                 (r.Status == RegistrationStatus.Pending ||
                                  r.Status == RegistrationStatus.Paid));

            return count < cls.Capacity;
        }
    }
}
