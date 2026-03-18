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
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;
        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }
        public Schedule? Read(string studentId, string classId)
        {
            return _context.Schedules
                .Include(s => s.Student)
                .Include(s => s.Class)
                .FirstOrDefault(s => s.StudentId == studentId && s.ClassId == classId);
        }
    }
}
