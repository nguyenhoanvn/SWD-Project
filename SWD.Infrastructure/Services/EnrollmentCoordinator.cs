using Microsoft.EntityFrameworkCore;
using SWD.Application.DTOs;
using SWD.Application.Interfaces;
using SWD.Data;
using SWD.Domain.Models;
using SWD.Interfaces;

namespace SWD.Services
{
    public class EnrollmentCoordinator : IEnrollmentCoordinator
    {
        private readonly IAcademicService     _academic;
        private readonly IFinancialService    _financial;
        private readonly IAuditLogService     _audit;
        private readonly INotificationService _notification;
        private readonly IClassRepository _classRepository;
        private readonly IRegistrationRepository _registrationRepository;

        public EnrollmentCoordinator(
            IAcademicService     academic,
            IFinancialService    financial,
            IAuditLogService     audit,
            INotificationService notification,
            IClassRepository classRepository,
            IRegistrationRepository registrationRepository
)
        {
            _academic     = academic;
            _financial    = financial;
            _audit        = audit;
            _notification = notification;
            _classRepository = classRepository;
            _registrationRepository = registrationRepository;
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
            var cls = _classRepository.Read(classId);
            _registrationRepository.Create(studentId, classId);

            await _audit.LogActivity(studentId, "", $"Tạo đơn đăng ký lớp {cls.ClassName}");

            // 1.12 chuyển sang UC18
            return new EnrollmentResult(true, "Đơn đăng ký đã được tạo. Vui lòng hoàn tất thanh toán.");
        }

        // UC18 msg 2.2 processTransactionResult
        public async Task<EnrollmentResult> ProcessTransactionResult(string registrationId, PaymentResult paymentResult)
        {
            // 2.3 logActivity
            await _audit.LogActivity("", paymentResult.TransactionRef,
                $"Giao dịch {paymentResult.TransactionRef}: {(paymentResult.IsSuccess ? "Thành công" : "Thất bại")}");

            var reg = _registrationRepository.Read(registrationId);

            if (reg == null)
                return new EnrollmentResult(false, "Không tìm thấy đơn đăng ký.");

            if (paymentResult.IsSuccess)
            {
                _registrationRepository.Update(registrationId, RegistrationStatus.Paid);

                // 2.4 triggerEnrollmentNotification
                await _notification.TriggerEnrollmentNotification(reg, true);

                return new EnrollmentResult(true,
                    $"Đăng ký thành công! Bạn đã ghi danh vào lớp {reg.Class.ClassName}.",
                    registrationId);
            }
            else
            {
                _registrationRepository.Update(registrationId, RegistrationStatus.Cancelled);

                await _notification.TriggerEnrollmentNotification(reg, false);

                return new EnrollmentResult(false,
                    $"Thanh toán thất bại: {paymentResult.Message}. Đơn đăng ký đã bị hủy.");
            }
        }
    }
}
