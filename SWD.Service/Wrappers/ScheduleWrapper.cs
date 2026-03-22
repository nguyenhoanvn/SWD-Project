using SWD.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Wrappers
{
    public class ScheduleWrapper
    {
        private readonly CECMSContext _db = CECMSContext.Instance;

        /// <summary>Read(in studentId, in classId, out schedule)</summary>
        public Schedule? Read(string studentId, string classId)
        {
            var reg = _db.Registrations.FirstOrDefault(r => r.StudentId == studentId && r.ClassId == classId && r.Status == 1);
            if (reg == null) return null;
            var cls = _db.Classes.FirstOrDefault(c => c.ClassId == classId);
            if (cls == null) return null;
            return _db.Schedules.FirstOrDefault(s => s.ScheduleId == cls.ScheduleId);
        }

        public Schedule? ReadByScheduleId(string scheduleId)
            => _db.Schedules.FirstOrDefault(s => s.ScheduleId == scheduleId);

        /// Returns all schedules the student is currently enrolled in
        public List<Schedule> ReadStudentSchedules(string studentId)
        {
            var classIds = _db.Registrations
                .Where(r => r.StudentId == studentId && r.Status == 1)
                .Select(r => r.ClassId)
                .ToList();
            var scheduleIds = _db.Classes
                .Where(c => classIds.Contains(c.ClassId))
                .Select(c => c.ScheduleId)
                .ToList();
            return _db.Schedules.Where(s => scheduleIds.Contains(s.ScheduleId)).ToList();
        }
    }
}
