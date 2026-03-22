using SWD.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service
{
    public class CECMSContext
    {
        // ── Tables ──────────────────────────────────────────────
        public List<Student> Students { get; } = [];
        public List<Score> Scores { get; } = [];
        public List<Schedule> Schedules { get; } = [];
        public List<Class> Classes { get; } = [];
        public List<Registration> Registrations { get; } = [];
        public List<Payment> Payments { get; } = [];
        public List<Notification> Notifications { get; } = [];
        public List<TransactionLog> TransactionLogs { get; } = [];
        public List<UserAccount> UserAccounts { get; } = [];

        // ── Singleton ────────────────────────────────────────────
        private static CECMSContext? _instance;
        public static CECMSContext Instance => _instance ??= new CECMSContext();

        private CECMSContext() => Seed();

        // ── Seed demo data ───────────────────────────────────────
        private void Seed()
        {
            // Schedules
            var sch1 = new Schedule { ScheduleId = "SCH001", DayOfWeek = 2, StartTime = DateTime.Today.AddHours(8), EndTime = DateTime.Today.AddHours(10) };
            var sch2 = new Schedule { ScheduleId = "SCH002", DayOfWeek = 3, StartTime = DateTime.Today.AddHours(13), EndTime = DateTime.Today.AddHours(15) };
            var sch3 = new Schedule { ScheduleId = "SCH003", DayOfWeek = 2, StartTime = DateTime.Today.AddHours(8), EndTime = DateTime.Today.AddHours(10) }; // conflicts with sch1
            var sch4 = new Schedule { ScheduleId = "SCH004", DayOfWeek = 5, StartTime = DateTime.Today.AddHours(9), EndTime = DateTime.Today.AddHours(11) };
            Schedules.AddRange([sch1, sch2, sch3, sch4]);

            // Classes
            var c1 = new Class { ClassId = "CLS001", ClassName = "Advanced English A1", Capacity = 20, ScheduleId = "SCH001", ConditionScore = 7.0 };
            var c2 = new Class { ClassId = "CLS002", ClassName = "Basic English B1", Capacity = 2, ScheduleId = "SCH002", ConditionScore = 0.0 }; // nearly full
            var c3 = new Class { ClassId = "CLS003", ClassName = "Intermediate B2", Capacity = 15, ScheduleId = "SCH003", ConditionScore = 5.0 };
            var c4 = new Class { ClassId = "CLS004", ClassName = "Business English C1", Capacity = 10, ScheduleId = "SCH004", ConditionScore = 8.0 };
            Classes.AddRange([c1, c2, c3, c4]);

            // Students
            var s1 = new Student { StudentId = "STU001", StudentName = "Alice Nguyen", Email = "alice@demo.com" };
            var s2 = new Student { StudentId = "STU002", StudentName = "Bob Tran", Email = "bob@demo.com" };
            Students.AddRange([s1, s2]);

            // Scores (placement test results)
            Scores.AddRange([
                new Score { ScoreId = "SC001", StudentId = "STU001", ClassId = "CLS001", ScoreValue = 8.5 },
            new Score { ScoreId = "SC002", StudentId = "STU001", ClassId = "CLS004", ScoreValue = 8.5 },
            new Score { ScoreId = "SC005", StudentId = "STU001", ClassId = "CLS003", ScoreValue = 10 },
            new Score { ScoreId = "SC003", StudentId = "STU002", ClassId = "CLS001", ScoreValue = 5.0 }, // below 7.0 → blocked
            new Score { ScoreId = "SC004", StudentId = "STU002", ClassId = "CLS002", ScoreValue = 6.0 },
        ]);

            // Pre-existing registration for conflict demo
            Registrations.Add(new Registration
            {
                RegistrationId = "REG000",
                StudentId = "STU001",
                ClassId = "CLS001",
                RegistrationDate = DateTime.UtcNow.AddDays(-2),
                Status = 1
            });

            // Fill CLS002 up to capacity - 1 (leaves 1 seat)
            Registrations.Add(new Registration { RegistrationId = "REG_FILL1", StudentId = "STU002", ClassId = "CLS002", Status = 1 });

            // User accounts
            UserAccounts.AddRange([
                new UserAccount { AccountId = "ACC001", Email = "alice@demo.com", Fullname = "Alice Nguyen", Role = "Student", Status = "ACTIVE", Password = "demo", PhoneNumber = "0901111111" },
            new UserAccount { AccountId = "ACC002", Email = "bob@demo.com",   Fullname = "Bob Tran",     Role = "Student", Status = "ACTIVE", Password = "demo", PhoneNumber = "0902222222" },
        ]);
        }
    }
}
