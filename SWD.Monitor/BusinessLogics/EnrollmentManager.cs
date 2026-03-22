using SWD.Service.DTOs;
using SWD.Service.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Monitor.BusinessLogics
{
    public class EnrollmentManager
    {
        private readonly StudentWrapper _studentWrapper = new();
        private readonly ClassWrapper _classWrapper = new();
        private readonly ScoreWrapper _scoreWrapper = new();
        private readonly ScheduleWrapper _scheduleWrapper = new();
        private readonly RegistrationWrapper _registrationWrapper = new();

        public IsValidResult EnrollRequest(string studentId, string classId)
        {
            // 1.3 Read Student Information
            var student = _studentWrapper.Read(studentId);
            if (student == null)
                return new IsValidResult(false, "Student not found.");

            // 1.7 Read Condition Score (prerequisite) from Class
            var cls = _classWrapper.Read(classId);
            if (cls == null)
                return new IsValidResult(false, "Class not found.");

            // 1.5–1.6 Read Student Score
            var score = _scoreWrapper.Read(studentId, classId);
            double studentScore = score?.ScoreValue ?? 0.0;

            // 1.8 Check prerequisite condition score — A1 alternative flow
            if (cls.ConditionScore > 0 && studentScore < cls.ConditionScore)
                return new IsValidResult(false,
                    $"Score prerequisite not met. Required: {cls.ConditionScore}, Your score: {studentScore}");

            // 1.9–1.10 Read Schedule — check conflicts — A2 alternative flow
            var targetSchedule = _scheduleWrapper.ReadByScheduleId(cls.ScheduleId);
            if (targetSchedule == null)
                return new IsValidResult(false, "Class schedule not found.");

            var studentSchedules = _scheduleWrapper.ReadStudentSchedules(studentId);
            foreach (var s in studentSchedules)
            {
                if (s.ScheduleId == cls.ScheduleId) continue; // same slot already enrolled (edge case)
                if (s.DayOfWeek == targetSchedule.DayOfWeek
                    && s.StartTime < targetSchedule.EndTime
                    && s.EndTime > targetSchedule.StartTime)
                {
                    return new IsValidResult(false, "Schedule conflict detected.");
                }
            }

            // 1.11–1.12 Read Capacity — A3 alternative flow
            var enrolled = _registrationWrapper.CountActive(classId);
            if (enrolled >= cls.Capacity)
                return new IsValidResult(false, "Class is full. No seats available.");

            // All validations passed — return StudentClass payload (msg 1.13)
            var studentClass = new StudentClass(student, cls);
            return new IsValidResult(true, "Enrollment conditions satisfied.", studentClass);
        }
    }
}
