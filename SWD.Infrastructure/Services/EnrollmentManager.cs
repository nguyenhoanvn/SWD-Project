using SWD.Application.DTOs;
using SWD.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Infrastructure.Services
{
    public class EnrollmentManager : IEnrollmentManager
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IScoreRepository _scoreRepository;
        private readonly IClassRepository _classRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public EnrollmentManager(IStudentRepository studentRepository, IScoreRepository scoreRepository, IClassRepository classRepository, IScheduleRepository scheduleRepository)
        {
            _studentRepository = studentRepository;
            _scheduleRepository = scheduleRepository;
            _scoreRepository = scoreRepository;
            _classRepository = classRepository;
        }
        public EnrollmentResult EnrollRequest(string studentId, string classId)
        {
            var student = _studentRepository.Read(studentId);

            var score = _scoreRepository.Read(studentId, classId);

            var classObj = _classRepository.Read(classId);

            var schedule = _scheduleRepository.Read(studentId, classId);
            if (schedule == null)
            {
                return new EnrollmentResult(false, "Invalid Schedule");
            }

            if (classObj != null && classObj.Capacity > 30)
            {
                return new EnrollmentResult(false, "Invalid Capacity");
            }

            return new EnrollmentResult(true, "Enroll Completed");

        }
    }
}
