using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWD.Coordinator;
using SWD.Service.Wrappers;

namespace SWD.Web.Pages.Enrollment
{
    public class PaymentModel : PageModel
    {
        private readonly EnrollmentCoordinator _coordinator = new();
        private readonly ClassWrapper _classWrapper = new();
        private readonly ScheduleWrapper _scheduleWrapper = new();

        [BindProperty(SupportsGet = true)]
        public string StudentId { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string ClassId { get; set; } = "";

        public string ClassName { get; private set; } = "";
        public string ScheduleInfo { get; private set; } = "";
        public string ResultMessage { get; private set; } = "";
        public bool PaymentSuccess { get; private set; }
        public string TransactionReference { get; private set; } = "";

        private static readonly string[] DayNames = ["", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

        public void OnGet()
        {
            LoadClassInfo();
        }

        // UC18 msg 2.1: Student enters payment info → StudentInterface sends to EnrollmentCoordinator
        public IActionResult OnPost(
            string studentId,
            string classId,
            string paymentMethod,
            bool simulateSuccess)
        {
            StudentId = studentId;
            ClassId = classId;
            LoadClassInfo();

            // msg 2.1: Send Payment Data → EnrollmentCoordinator.requestPaymentTransaction
            var result = _coordinator.RequestPaymentTransaction(
                studentId,
                classId,
                paymentMethod,
                simulateSuccess
            );

            PaymentSuccess = result.Success;
            ResultMessage = result.Message;
            TransactionReference = result.TransactionReference ?? "";

            return Page();
        }

        private void LoadClassInfo()
        {
            var cls = _classWrapper.Read(ClassId);
            if (cls == null) return;
            ClassName = cls.ClassName;
            var schedule = _scheduleWrapper.ReadByScheduleId(cls.ScheduleId);
            if (schedule != null)
            {
                ScheduleInfo = $"{DayNames[schedule.DayOfWeek]} {schedule.StartTime:HH:mm}–{schedule.EndTime:HH:mm}";
            }
        }
    }
}
