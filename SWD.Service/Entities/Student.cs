using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Entities
{
    public class Student
    {
        public string StudentId { get; set; } = Guid.NewGuid().ToString();
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
