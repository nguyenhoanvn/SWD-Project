

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD.Models
{

    public class Student
    {
        [Key] public string StudentId { get; set; } = Guid.NewGuid().ToString();
        [Required] public string StudentName { get; set; } = "";
        [Required] public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<Score> Scores { get; set; } = new List<Score>();
    }

    public class Course
    {
        [Key] public string CourseId { get; set; } = Guid.NewGuid().ToString();
        [Required] public string CourseName { get; set; } = "";
        public double ScoreCondition { get; set; } = 0;   
        public decimal Fee { get; set; } = 0;
        public string Description { get; set; } = "";

        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }

    public class Schedule
    {
        [Key] public string ScheduleId { get; set; } = Guid.NewGuid().ToString();
        public int DayOfWeek { get; set; }     
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string DayName => DayOfWeek switch
        {
            2 => "Thứ 2",
            3 => "Thứ 3",
            4 => "Thứ 4",
            5 => "Thứ 5",
            6 => "Thứ 6",
            7 => "Thứ 7",
            _ => "Chủ nhật"
        };
    }

    public class Class
    {
        [Key] public string ClassId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Course")] public string CourseId { get; set; } = "";
        [ForeignKey("Schedule")] public string ScheduleId { get; set; } = "";
        public string ClassName { get; set; } = "";
        public int Capacity { get; set; } = 30;

        public Course Course { get; set; } = null!;
        public Schedule Schedule { get; set; } = null!;
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }


    public class Score
    {
        [Key] public string ScoreId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Student")] public string StudentId { get; set; } = "";
        public double ScoreValue { get; set; } = 0;
        public string CourseName { get; set; } = "";   

        public Student Student { get; set; } = null!;
    }

    public enum RegistrationStatus { Pending = 0, Paid = 1, Cancelled = 2 }

    public class Registration
    {
        [Key] public string RegistrationId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Student")] public string StudentId { get; set; } = "";
        [ForeignKey("Class")] public string ClassId { get; set; } = "";
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

        // Navigation
        public Student Student { get; set; } = null!;
        public Class Class { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public enum PaymentStatus { Pending = 0, Success = 1, Failed = 2 }

    public class Payment
    {
        [Key] public string PaymentId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Registration")] public string RegistrationId { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = "";        
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string TransactionReference { get; set; } = "";   

        public Registration Registration { get; set; } = null!;
    }

    public class TransactionLog
    {
        [Key] public string LogId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = "";
        public string TransactionReference { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Details { get; set; } = "";
    }
}