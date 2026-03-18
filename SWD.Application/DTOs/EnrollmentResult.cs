using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Application.DTOs
{
    public record EnrollmentResult(bool IsSuccess, string Message, string? RegistrationId = null);
}
