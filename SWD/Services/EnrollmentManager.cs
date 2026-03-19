using Microsoft.EntityFrameworkCore;
using SWD.Data;
using SWD.Interfaces;
using SWD.Models;

namespace SWD.Services
{
    // «business logic» EnrollmentManager
    // Chịu trách nhiệm kiểm tra toàn bộ điều kiện ghi danh
    // Thay thế AcademicService trong design mới
    public class EnrollmentManager : IEnrollmentManager
    {
        private readonly AppDbContext _db;
        public EnrollmentManager(AppDbContext db) => _db = db;

        public async Task<bool> EnrollRequest(string studentId, string classId)
        {
            if (!await ValidatePrerequisite(studentId, classId)) return false;
            if (!await ValidateSchedule(studentId, classId)) return false;
            if (!await ValidateCapacity(classId)) return false;

            return true;
        }


        private async Task<bool> ValidatePrerequisite(string studentId, string classId)
        {
            var cls = await _db.Classes.Include(c => c.Course)
                                       .FirstOrDefaultAsync(c => c.ClassId == classId);
            if (cls == null) return false;

            double required = cls.Course.ScoreCondition;
            if (required <= 0) return true;  

            var bestScore = await _db.Scores
                .Where(s => s.StudentId == studentId)
                .MaxAsync(s => (double?)s.ScoreValue) ?? 0;

            return bestScore >= required;
        }

        private async Task<bool> ValidateSchedule(string studentId, string classId)
        {
            var target = await _db.Classes.Include(c => c.Schedule)
                                          .FirstOrDefaultAsync(c => c.ClassId == classId);
            if (target == null) return false;

            var enrolledClasses = await _db.Registrations
                .Where(r => r.StudentId == studentId &&
                            (r.Status == RegistrationStatus.Pending ||
                             r.Status == RegistrationStatus.Paid))
                .Include(r => r.Class).ThenInclude(c => c.Schedule)
                .Select(r => r.Class)
                .ToListAsync();

            foreach (var ec in enrolledClasses)
            {
                if (ec.Schedule.DayOfWeek  == target.Schedule.DayOfWeek &&
                    ec.Schedule.StartTime   < target.Schedule.EndTime   &&
                    target.Schedule.StartTime < ec.Schedule.EndTime)
                    return false;
            }
            return true;
        }

        private async Task<bool> ValidateCapacity(string classId)
        {
            var cls = await _db.Classes.FirstOrDefaultAsync(c => c.ClassId == classId);
            if (cls == null) return false;

            int enrolled = await _db.Registrations
                .CountAsync(r => r.ClassId == classId &&
                                 (r.Status == RegistrationStatus.Pending ||
                                  r.Status == RegistrationStatus.Paid));

            return enrolled < cls.Capacity;
        }
    }
}
