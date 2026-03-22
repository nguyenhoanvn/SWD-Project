using SWD.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Wrappers
{
    public class ScoreWrapper
    {
        private readonly CECMSContext _db = CECMSContext.Instance;

        /// <summary>Read(in studentId, in classId, out score)</summary>
        public Score? Read(string studentId, string classId)
            => _db.Scores.FirstOrDefault(s => s.StudentId == studentId && s.ClassId == classId);
    }
}
