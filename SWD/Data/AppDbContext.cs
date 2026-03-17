using Microsoft.EntityFrameworkCore;
using SWD.Models;

namespace SWD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
        }
    }
}