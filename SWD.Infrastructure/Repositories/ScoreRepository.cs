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
    public class ScoreRepository : IScoreRepository
    {
        private readonly AppDbContext _context;
        public ScoreRepository(AppDbContext context)
        {
            _context = context;
        }
        public Score? Read(string studentId, string classId)
        {
            return _context.Scores
                .Include(s => s.Student)
                .Include(s => s.Class)
                .FirstOrDefault(s => s.StudentId == studentId && s.ClassId == classId);
        }
    }
}
