using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Domain.Models
{
    public class Schedule
    {
        [Key] public string ScheduleId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Student")] public string StudentId { get; set; } = "";
        [ForeignKey("Class")] public string ClassId { get; set; } = "";
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Student Student { get; set; } = null!;
        public Class Class { get; set; } = null!;

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
}
