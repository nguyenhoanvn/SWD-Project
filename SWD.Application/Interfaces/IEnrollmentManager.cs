using SWD.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Application.Interfaces
{
    public interface IEnrollmentManager
    {
        EnrollmentResult EnrollRequest(string studentId, string classId);
    }
}
