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
    public class ClassRepository : IClassRepository
    {
        private readonly AppDbContext _context;
        public ClassRepository(AppDbContext context)
        {
            _context = context;
        }
        public Class? Read(string classId)
        {
            return _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Schedule)
                .Include(c => c.Registrations)
                .FirstOrDefault(c => c.ClassId == classId);
        }
    }
}
