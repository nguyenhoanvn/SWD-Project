
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Domain.Models
{
    public class Course
    {
        [Key] public string CourseId { get; set; } = Guid.NewGuid().ToString();
        [Required] public string CourseName { get; set; } = "";
        public double ScoreCondition { get; set; } = 0;
        public decimal Fee { get; set; } = 0;
        public string Description { get; set; } = "";

        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
