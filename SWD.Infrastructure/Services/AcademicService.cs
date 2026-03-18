using Microsoft.EntityFrameworkCore;
using SWD.Data;
using SWD.Domain.Models;
using SWD.Interfaces;

namespace SWD.Services
{
    public class AcademicService : IAcademicService
    {
        private readonly AppDbContext _db;
        public AcademicService(AppDbContext db) => _db = db;

        // msg 1.3–1.6: kiểm tra điều kiện tiên quyết
        public async Task<bool> ValidatePrerequisite(string studentId, string classId)
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

        // msg 1.7–1.8: kiểm tra trùng lịch
        public async Task<bool> ValidateSchedule(string studentId, string classId)
        {
            var target = await _db.Classes.Include(c => c.Schedule)
                                          .FirstOrDefaultAsync(c => c.ClassId == classId);
            if (target == null) return false;

            var enrolledClasses = await _db.Registrations
                .Where(r => r.StudentId == studentId &&
                            (r.Status == RegistrationStatus.Pending || r.Status == RegistrationStatus.Paid))
                .Include(r => r.Class).ThenInclude(c => c.Schedule)
                .Select(r => r.Class)
                .ToListAsync();

            foreach (var ec in enrolledClasses)
            {
                if (ec.Schedule.DayOfWeek == target.Schedule.DayOfWeek &&
                    ec.Schedule.StartTime  < target.Schedule.EndTime   &&
                    target.Schedule.StartTime < ec.Schedule.EndTime)
                    return false;
            }
            return true;
        }

        // msg 1.9–1.10: kiểm tra sức chứa
        public async Task<bool> ValidateCapacity(string classId)
        {
            var cls = await _db.Classes.FirstOrDefaultAsync(c => c.ClassId == classId);
            if (cls == null) return false;

            int enrolled = await _db.Registrations
                .CountAsync(r => r.ClassId == classId &&
                                 (r.Status == RegistrationStatus.Pending || r.Status == RegistrationStatus.Paid));

            return enrolled < cls.Capacity;
        }
    }
}
