using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWD.Service;
using SWD.Service.Entities;
using SWD.Service.Wrappers;

namespace SWD.Web.Pages.Enrollment
{
    public class EnrollmentView
    {
        public string RegistrationId { get; set; } = "";
        public string ClassId { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string ScheduleInfo { get; set; } = "";
        public DateTime RegistrationDate { get; set; }
        public int Status { get; set; }
        public int PaymentStatus { get; set; }
        public string TransactionRef { get; set; } = "";
    }
    public class HistoryModel : PageModel
    {
        private readonly RegistrationWrapper _registrationWrapper = new();
        private readonly ClassWrapper _classWrapper = new();
        private readonly ScheduleWrapper _scheduleWrapper = new();
        private readonly CECMSContext _db = CECMSContext.Instance;

        private static readonly string[] DayNames = ["", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

        public string StudentId { get; private set; } = "";
        public List<Student> AllStudents { get; private set; } = [];
        public List<EnrollmentView> Enrollments { get; private set; } = [];
        public List<Notification> Notifications { get; private set; } = [];

        public void OnGet(string? studentId)
        {
            AllStudents = _db.Students;
            StudentId = studentId ?? _db.Students.FirstOrDefault()?.StudentId ?? "";

            var regs = _registrationWrapper.ReadByStudent(StudentId);
            Enrollments = regs.Select(r =>
            {
                var cls = _classWrapper.Read(r.ClassId);
                var schedule = cls != null ? _scheduleWrapper.ReadByScheduleId(cls.ScheduleId) : null;
                var payment = _db.Payments.FirstOrDefault(p => p.RegistrationId == r.RegistrationId);

                // Also find payment by checking transaction logs matching this registration's class
                if (payment == null)
                {
                    // Find most recent payment for this student+class combo by checking all payments
                    var allPayments = _db.Payments.Where(p => p.Status != 0).ToList();
                    var log = _db.TransactionLogs
                        .Where(l => l.UserId == StudentId && l.Details.Contains(r.ClassId))
                        .OrderByDescending(l => l.Timestamp)
                        .FirstOrDefault();
                    if (log != null)
                        payment = _db.Payments.FirstOrDefault(p => p.TransactionReference == log.TransactionReference);
                }

                return new EnrollmentView
                {
                    RegistrationId = r.RegistrationId,
                    ClassId = r.ClassId,
                    ClassName = cls?.ClassName ?? r.ClassId,
                    ScheduleInfo = schedule != null
                        ? $"{DayNames[schedule.DayOfWeek]} {schedule.StartTime:HH:mm}–{schedule.EndTime:HH:mm}"
                        : "TBD",
                    RegistrationDate = r.RegistrationDate,
                    Status = r.Status,
                    PaymentStatus = payment?.Status ?? 0,
                    TransactionRef = payment?.TransactionReference ?? ""
                };
            }).OrderByDescending(e => e.RegistrationDate).ToList();

            Notifications = _db.Notifications
                .Where(n => n.ReceiverId == StudentId)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }
    }
}
