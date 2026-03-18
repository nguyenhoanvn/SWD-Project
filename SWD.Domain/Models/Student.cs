using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Domain.Models
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
}
