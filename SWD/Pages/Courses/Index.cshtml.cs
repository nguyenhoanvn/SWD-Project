

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWD.Data;
using SWD.Models;

namespace SWD.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public IndexModel(AppDbContext db) => _db = db;

        public List<Class>   AvailableClasses { get; set; } = new();
        public List<Student> Students         { get; set; } = new();  

        [BindProperty(SupportsGet = true)]
        public string? SelectedStudentId { get; set; }

        public async Task OnGetAsync()
        {
            Students = await _db.Students.ToListAsync();

            AvailableClasses = await _db.Classes
                .Include(c => c.Course)
                .Include(c => c.Schedule)
                .Include(c => c.Registrations)
                .OrderBy(c => c.Course.CourseName)
                .ToListAsync();
        }
    }
}
