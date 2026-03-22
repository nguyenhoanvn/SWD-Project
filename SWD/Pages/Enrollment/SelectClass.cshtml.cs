using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWD.Coordinator;
using SWD.Service;
using SWD.Service.Entities;
using SWD.Service.Wrappers;

namespace SWD.Web.Pages.Enrollment
{
    public class ClassViewModel
    {
        public string ClassId { get; set; } = "";
        public string ClassName { get; set; } = "";
        public double ConditionScore { get; set; }
        public int Capacity { get; set; }
        public int SeatsLeft { get; set; }
        public string DayName { get; set; } = "";
        public string TimeRange { get; set; } = "";
    }

    public class RegistrationViewModel
    {
        public string ClassName { get; set; } = "";
        public string DayName { get; set; } = "";
        public string TimeRange { get; set; } = "";
    }
    public class SelectClassModel : PageModel
    {
        private readonly EnrollmentCoordinator _coordinator = new();
        private readonly ClassWrapper _classWrapper = new();
        private readonly RegistrationWrapper _registrationWrapper = new();
        private readonly StudentWrapper _studentWrapper = new();
        private readonly ScheduleWrapper _scheduleWrapper = new();
        private readonly ScoreWrapper _scoreWrapper = new();
        private readonly CECMSContext _db = CECMSContext.Instance;

        [BindProperty(SupportsGet = true)]
        public string StudentId { get; set; } = "";

        public Student? Student { get; private set; }
        public List<ClassViewModel> ClassList { get; private set; } = [];
        public List<RegistrationViewModel> CurrentRegistrations { get; private set; } = [];
        public string ValidationMessage { get; private set; } = "";
        public bool ValidationSuccess { get; private set; }
        public string ValidatedClassId { get; private set; } = "";
        public string YourScore { get; private set; } = "";

        private static readonly string[] DayNames = ["", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

        public void OnGet()
        {
            LoadData();
        }

        // UC02 msg 1.1: selectClass → StudentInterface → Send Enrollment Request to Coordinator
        public IActionResult OnPost(string studentId, string classId, string action)
        {
            StudentId = studentId;
            LoadData();

            if (string.IsNullOrEmpty(classId))
            {
                ValidationMessage = "Please select a class first.";
                return Page();
            }

            if (action == "validate")
            {
                // Show the student's score for the selected class
                var score = _scoreWrapper.Read(studentId, classId);
                YourScore = score != null ? score.ScoreValue.ToString("F1") : "No score on record";

                // msg 1.1 → Coordinator.sendEnrollmentRequest
                var result = _coordinator.SendEnrollmentRequest(studentId, classId);

                ValidationSuccess = result.Success;
                ValidationMessage = result.Message;
                if (result.Success) ValidatedClassId = classId;
            }

            return Page();
        }

        private void LoadData()
        {
            Student = _studentWrapper.Read(StudentId);

            // Build ClassViewModel list with seat counts and schedule info
            ClassList = _classWrapper.ReadAll().Select(c =>
            {
                var schedule = _scheduleWrapper.ReadByScheduleId(c.ScheduleId);
                int enrolled = _registrationWrapper.CountActive(c.ClassId);
                return new ClassViewModel
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName,
                    ConditionScore = c.ConditionScore,
                    Capacity = c.Capacity,
                    SeatsLeft = c.Capacity - enrolled,
                    DayName = schedule != null ? DayNames[schedule.DayOfWeek] : "TBD",
                    TimeRange = schedule != null
                        ? $"{schedule.StartTime:HH:mm}–{schedule.EndTime:HH:mm}"
                        : "TBD"
                };
            }).ToList();

            // Current enrollments for schedule display
            var myRegs = _registrationWrapper.ReadByStudent(StudentId)
                .Where(r => r.Status == 1).ToList();

            CurrentRegistrations = myRegs.Select(r =>
            {
                var cls = _classWrapper.Read(r.ClassId);
                if (cls == null) return null;
                var schedule = _scheduleWrapper.ReadByScheduleId(cls.ScheduleId);
                return new RegistrationViewModel
                {
                    ClassName = cls.ClassName,
                    DayName = schedule != null ? DayNames[schedule.DayOfWeek] : "TBD",
                    TimeRange = schedule != null
                        ? $"{schedule.StartTime:HH:mm}–{schedule.EndTime:HH:mm}"
                        : "TBD"
                };
            }).Where(x => x != null).Cast<RegistrationViewModel>().ToList();
        }
    }
}
