using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Domain.Models
{
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
}
