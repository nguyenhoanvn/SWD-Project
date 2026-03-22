
using Microsoft.Extensions.DependencyInjection;
using SWD.Domain.Models;

namespace SWD.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            if (db.Students.Any()) return;

            // ── Schedules ──────────────────────────────────────
            var sch1 = new Schedule { ScheduleId = "SCH01", DayOfWeek = 2, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(10, 0, 0) };
            var sch2 = new Schedule { ScheduleId = "SCH02", DayOfWeek = 4, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(10, 0, 0) };
            var sch3 = new Schedule { ScheduleId = "SCH03", DayOfWeek = 3, StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(15, 0, 0) };
            var sch4 = new Schedule { ScheduleId = "SCH04", DayOfWeek = 6, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(11, 0, 0) };
            db.Schedules.AddRange(sch1, sch2, sch3, sch4);

            // ── Courses ────────────────────────────────────────
            var c1 = new Course { CourseId = "C01", CourseName = "IELTS Foundation", Description = "Khóa nền tảng IELTS cho người mới bắt đầu" };
            var c2 = new Course { CourseId = "C02", CourseName = "IELTS Intermediate", Description = "Yêu cầu IELTS Foundation ≥ 4.5" };
            var c3 = new Course { CourseId = "C03", CourseName = "IELTS Advanced", Description = "Yêu cầu IELTS Intermediate ≥ 6.0" };
            var c4 = new Course { CourseId = "C04", CourseName = "Business English", Description = "Tiếng Anh thương mại cho người đi làm" };
            db.Courses.AddRange(c1, c2, c3, c4);

            // ── Classes ────────────────────────────────────────
            var cl1 = new Class { ClassId = "CL01", CourseId = "C01", ScheduleId = "SCH01", ClassName = "Foundation A - Thứ 2", Capacity = 20, ScoreCondition = 0, Fee = 3_500_000 };
            var cl2 = new Class { ClassId = "CL02", CourseId = "C01", ScheduleId = "SCH04", ClassName = "Foundation B - Thứ 7", Capacity = 20, ScoreCondition = 4.5, Fee = 4_500_000 };
            var cl3 = new Class { ClassId = "CL03", CourseId = "C02", ScheduleId = "SCH02", ClassName = "Intermediate - Thứ 4", Capacity = 15, ScoreCondition = 6.0, Fee = 5_500_000 };
            var cl4 = new Class { ClassId = "CL04", CourseId = "C03", ScheduleId = "SCH03", ClassName = "Advanced - Thứ 3", Capacity = 10, ScoreCondition = 0, Fee = 4_000_000 };
            var cl5 = new Class { ClassId = "CL05", CourseId = "C04", ScheduleId = "SCH01", ClassName = "Business - Thứ 2", Capacity = 25, ScoreCondition = 0, Fee = 4_000_000 };
            db.Classes.AddRange(cl1, cl2, cl3, cl4, cl5);

            // ── Students ───────────────────────────────────────
            var s1 = new Student { StudentId = "S01", StudentName = "Nguyễn Văn An", Email = "an@student.com", PasswordHash = "demo" };
            var s2 = new Student { StudentId = "S02", StudentName = "Trần Thị Bình", Email = "binh@student.com", PasswordHash = "demo" };
            var s3 = new Student { StudentId = "S03", StudentName = "Lê Minh Cường", Email = "cuong@student.com", PasswordHash = "demo" };
            db.Students.AddRange(s1, s2, s3);

            // ── Scores (điểm đã có để demo điều kiện tiên quyết) ──
            // An: đã học Foundation → 5.0 (đủ cho Intermediate)
            // Bình: đã học Foundation → 3.5 (không đủ cho Intermediate)
            // Cường: đã học Intermediate → 6.5 (đủ cho Advanced)
            db.Scores.AddRange(
                new Score { ScoreId = "SC01", StudentId = "S01", CourseName = "IELTS Foundation", ScoreValue = 5.0 },
                new Score { ScoreId = "SC02", StudentId = "S02", CourseName = "IELTS Foundation", ScoreValue = 3.5 },
                new Score { ScoreId = "SC03", StudentId = "S03", CourseName = "IELTS Intermediate", ScoreValue = 6.5 }
            );

            // Cường đã đăng ký lớp Business – Thứ 2 trước → dùng để demo trùng lịch
            var existingReg = new Registration
            {
                RegistrationId = "REG_DEMO",
                StudentId = "S03",
                ClassId = "CL05",
                RegistrationDate = DateTime.Now.AddDays(-5),
                Status = RegistrationStatus.Paid
            };
            db.Registrations.Add(existingReg);
            db.Payments.Add(new Payment
            {
                PaymentId = "PAY_DEMO",
                StudentId = "STU_DEMO",
                Amount = 4_000_000,
                PaymentMethod = "VNPay",
                Status = PaymentStatus.Success,
                TransactionReference = "TXN_DEMO_001"
            });

            db.SaveChanges();
        }
    }
}
