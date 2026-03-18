using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Domain.Models
{
    public class Score
    {
        [Key] public string ScoreId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Student")] public string StudentId { get; set; } = "";
        [ForeignKey("Class")] public string ClassId { get; set; } = "";
        public double ScoreValue { get; set; } = 0;
        public string CourseName { get; set; } = "";

        public Student Student { get; set; } = null!;
        public Class Class { get; set; } = null!;
    }
}
