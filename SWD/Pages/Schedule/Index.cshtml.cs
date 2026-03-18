using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWD.Data;
using Microsoft.AspNetCore.Mvc;
using SWD.Domain.Models;
namespace SWD.Pages.Schedule
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public IndexModel(AppDbContext db) => _db = db;

        [BindProperty(SupportsGet = true)]
        public string? SelectedStudentId { get; set; }

        public List<Student> Students     { get; set; } = new();
        public List<Class>   AllClasses   { get; set; } = new();
        public Student?      SelectedStudent { get; set; }

        public List<string>  EnrolledClassIds { get; set; } = new();

        public List<int> Days { get; } = new() { 2, 3, 4, 5, 6, 7, 1 };

        public async Task OnGetAsync()
        {
            Students = await _db.Students.ToListAsync();

            AllClasses = await _db.Classes
                .Include(c => c.Course)
                .Include(c => c.Schedule)
                .Include(c => c.Registrations)
                .ToListAsync();

            if (SelectedStudentId != null)
            {
                SelectedStudent = await _db.Students
                    .FirstOrDefaultAsync(s => s.StudentId == SelectedStudentId);

                EnrolledClassIds = await _db.Registrations
                    .Where(r => r.StudentId == SelectedStudentId &&
                                (r.Status == RegistrationStatus.Paid ||
                                 r.Status == RegistrationStatus.Pending))
                    .Select(r => r.ClassId)
                    .ToListAsync();
            }
        }
    }
}
