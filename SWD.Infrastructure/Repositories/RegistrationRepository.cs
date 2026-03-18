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
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly AppDbContext _context;
        public RegistrationRepository(AppDbContext context)
        {
            _context = context;
        }
        public Registration? Read(string registrationId)
        {
            return _context.Registrations
                .Include(r => r.Class).ThenInclude(c => c.Course)
                .Include(r => r.Student)
                .FirstOrDefault(r => r.RegistrationId == registrationId);
        }
        public string Create(string studentId, string classId)
        {
            var reg = new Registration
            {
                StudentId = studentId,
                ClassId = classId
            };

            _context.Registrations.Add(reg);
            _context.SaveChanges();

            return reg.RegistrationId;
        }

        public void Update(string registrationId, RegistrationStatus status)
        {
            var reg = _context.Registrations.Find(registrationId);

            if (reg != null)
            {
                reg.Status = status;
            }

            _context.SaveChanges();
        }
    }
}
