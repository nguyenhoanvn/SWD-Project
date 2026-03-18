using Microsoft.EntityFrameworkCore;
using SWD.Application.Interfaces;
using SWD.Data;
using SWD.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;
        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }
        public Student? Read(string studentId)
        {
            return _context.Students
                .Include(s => s.Registrations)
                .Include(s => s.Scores)
                .FirstOrDefault(s => s.StudentId == studentId);
        }
    }
}
