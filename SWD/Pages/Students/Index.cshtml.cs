using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWD.Data;
using SWD.Domain.Models;

namespace SWD.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public IndexModel(AppDbContext db) => _db = db;

        public List<Student> Students { get; set; } = new();

        public async Task OnGetAsync()
        {
            Students = await _db.Students
                .Include(s => s.Scores)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Class)
                        .ThenInclude(c => c.Course)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Class)
                        .ThenInclude(c => c.Schedule)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Payments)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostFillClassAsync(string classId)
        {
            var cls = await _db.Classes.FirstOrDefaultAsync(c => c.ClassId == classId);
            if (cls == null) return RedirectToPage();

            int current = await _db.Registrations
                .CountAsync(r => r.ClassId == classId && r.Status != RegistrationStatus.Cancelled);

            for (int i = current; i < cls.Capacity; i++)
            {
                _db.Registrations.Add(new Registration
                {
                    StudentId = "S01",
                    ClassId = classId,
                    Status = RegistrationStatus.Paid
                });
            }
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
