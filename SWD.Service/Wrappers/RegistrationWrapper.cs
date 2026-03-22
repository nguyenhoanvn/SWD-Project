using SWD.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Wrappers
{
    public class RegistrationWrapper
    {
        private readonly CECMSContext _db = CECMSContext.Instance;

        /// <summary>create(in studentId, in classId, out registrationId)</summary>
        public string Create(string studentId, string classId)
        {
            // Check duplicate
            var existing = _db.Registrations.FirstOrDefault(r =>
                r.StudentId == studentId && r.ClassId == classId && r.Status == 1);
            if (existing != null) return existing.RegistrationId;

            var newRegistrationId = Guid.NewGuid().ToString("N")[..8].ToUpper();
            _db.Registrations.Add(new Registration
            {
                RegistrationId = newRegistrationId,
                StudentId = studentId,
                ClassId = classId,
                RegistrationDate = DateTime.UtcNow,
                Status = 1
            });
            return newRegistrationId;
        }

        public int CountActive(string classId)
            => _db.Registrations.Count(r => r.ClassId == classId && r.Status == 1);

        public List<Registration> ReadByStudent(string studentId)
            => _db.Registrations.Where(r => r.StudentId == studentId).ToList();
    }
}
