// Pages/Courses/Enroll.cshtml.cs
// UC02: Enroll in Course
// Gọi EnrollmentCoordinator → kiểm tra prereq/schedule/capacity → tạo đơn → chuyển UC18

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWD.Data;
using SWD.Services;
using SWD.Interfaces;

namespace SWD.Pages.Courses
{
    public class EnrollModel : PageModel
    {
        private readonly IEnrollmentCoordinator _coordinator;
        private readonly AppDbContext           _db;

        public EnrollModel(IEnrollmentCoordinator coordinator, AppDbContext db)
        {
            _coordinator = coordinator;
            _db          = db;
        }

        [BindProperty(SupportsGet = true)] public string ClassId   { get; set; } = "";
        [BindProperty(SupportsGet = true)] public string StudentId { get; set; } = "";

        public SWD.Models.Class?   SelectedClass   { get; set; }
        public SWD.Models.Student? CurrentStudent  { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            SelectedClass  = await _db.Classes.Include(c => c.Course).Include(c => c.Schedule)
                                               .FirstOrDefaultAsync(c => c.ClassId == ClassId);
            CurrentStudent = await _db.Students.FirstOrDefaultAsync(s => s.StudentId == StudentId);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            // Gọi EnrollmentCoordinator.requestEnrollment (msg 1.1)
            var result = await _coordinator.RequestEnrollment(StudentId, ClassId);

            if (!result.IsSuccess)
            {
                ErrorMessage   = result.Message;
                SelectedClass  = await _db.Classes.Include(c => c.Course).Include(c => c.Schedule)
                                                   .FirstOrDefaultAsync(c => c.ClassId == ClassId);
                CurrentStudent = await _db.Students.FirstOrDefaultAsync(s => s.StudentId == StudentId);
                return Page();
            }

            return RedirectToPage("/Payment/Process", new
            {
                registrationId = result.RegistrationId,
                studentId      = StudentId
            });
        }
    }
}
