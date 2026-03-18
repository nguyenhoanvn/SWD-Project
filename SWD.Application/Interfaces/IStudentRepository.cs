using SWD.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Application.Interfaces
{
    public interface IStudentRepository
    {
        Student? Read(string studentId);
    }
}
