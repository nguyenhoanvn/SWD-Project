using SWD.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Application.Interfaces
{
    public interface IRegistrationRepository
    {
        Registration? Read(string registrationId);
        string Create(string studentId, string classId);
        void Update(string registrationId, RegistrationStatus status);
    }
}
