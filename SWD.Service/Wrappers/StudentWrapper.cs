using SWD.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Wrappers
{
    public class StudentWrapper
    {
        private readonly CECMSContext _db = CECMSContext.Instance;

        /// <summary>Read(in studentId, out Student)</summary>
        public Student? Read(string studentId)
            => _db.Students.FirstOrDefault(s => s.StudentId == studentId);
    }
}
