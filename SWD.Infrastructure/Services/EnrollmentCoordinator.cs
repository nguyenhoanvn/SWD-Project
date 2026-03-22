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
        private readonly IAcademicManager     _academic;
        private readonly IEnrollmentManager _enrollment;
        private readonly IFinancialService    _financial;
        private readonly IAuditLogService     _audit;
        private readonly INotificationService _notification;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IRegistrationRepository _registrationRepository;

        public EnrollmentCoordinator(
            IAcademicManager     academic,
            IEnrollmentManager enrollment,
            IFinancialService    financial,
            IAuditLogService     audit,
            INotificationService notification,
            IClassRepository classRepository,
            IRegistrationRepository registrationRepository,
            IStudentRepository studentRepository
)
        {
            _academic     = academic;
            _enrollment = enrollment;
            _financial    = financial;
            _audit        = audit;
            _notification = notification;
            _classRepository = classRepository;
            _registrationRepository = registrationRepository;
            _studentRepository = studentRepository;
        }

        // UC02 msg 1.1 → 1.11
        public async Task<EnrollmentResult> SendEnrollmentRequest(string studentId, string classId)
        {
            _enrollment.EnrollRequest(studentId, classId);

            // 1.11 Valid → tạo Registration Pending
            var cls = _classRepository.Read(classId);
            _registrationRepository.Create(studentId, classId);

            await _audit.LogActivity(studentId, "", $"Tạo đơn đăng ký lớp {cls.ClassName}");

            // 1.12 chuyển sang UC18
            return new EnrollmentResult(true, "Đơn đăng ký đã được tạo. Vui lòng hoàn tất thanh toán.");
        }

        public async Task<PaymentResult> SendPaymentData(string studentId, string classId)
        {
            var data = new PaymentRequest(studentId, classId, "Pending", 100);
            var res = await _financial.InitiatePayment(data);

            return res;
        }

        // UC18 msg 2.2 processTransactionResult
        public async Task<EnrollmentResult> ProcessTransactionResult(string studentId, PaymentResult paymentResult)
        {
            // 2.3 logActivity
            await _audit.LogActivity("", paymentResult.TransactionRef,
                $"Giao dịch {paymentResult.TransactionRef}: {(paymentResult.IsSuccess ? "Thành công" : "Thất bại")}");

            var student = _studentRepository.Read(studentId);

            if (student == null)
                return new EnrollmentResult(false, "Không tìm thấy học sinh.");

            if (paymentResult.IsSuccess)
            {
                // 2.4 triggerEnrollmentNotification
                await _notification.TriggerEnrollmentNotification(student, true);

                return new EnrollmentResult(true,
                    $"Đăng ký thành công!");
            }
            else
            {

                await _notification.TriggerEnrollmentNotification(student, false);

                return new EnrollmentResult(false,
                    $"Thanh toán thất bại: {paymentResult.Message}. Đơn đăng ký đã bị hủy.");
            }
        }
    }
}
