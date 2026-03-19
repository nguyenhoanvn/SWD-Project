using Microsoft.EntityFrameworkCore;
using SWD.Application.Interfaces;
using SWD.Data;
using SWD.Infrastructure.Repositories;
using SWD.Infrastructure.Services;
using SWD.Interfaces;
using SWD.Services;

namespace SWD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase("CambridgeDB"));

            builder.Services.AddScoped<IPaymentGatewayProxy, PaymentGatewayProxy>();
            builder.Services.AddScoped<INotificationProxy, NotificationProxy>();

            builder.Services.AddScoped<IAcademicManager, AcademicManager>();
            builder.Services.AddScoped<IEnrollmentManager, EnrollmentManager>();
            builder.Services.AddScoped<IFinancialService, FinancialService>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            builder.Services.AddScoped<IClassRepository, ClassRepository>();
            builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
            builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
            builder.Services.AddScoped<IScoreRepository, ScoreRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();

            builder.Services.AddScoped<IEnrollmentCoordinator, EnrollmentCoordinator>();

            var app = builder.Build();

            SeedData.Initialize(app.Services);

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapRazorPages();
            app.Run();
        }
    }
}