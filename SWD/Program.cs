using Microsoft.EntityFrameworkCore;
using SWD.Data;
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

            // ── Database InMemory ──────────────────────────────
            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase("CambridgeDB"));

            // ── Layer 3: Boundary / Proxy ──────────────────────
            builder.Services.AddScoped<IPaymentGatewayProxy, PaymentGatewayProxy>();
            builder.Services.AddScoped<INotificationProxy,   NotificationProxy>();

            // ── Layer 2: Business Logic + Services ────────────
            builder.Services.AddScoped<IEnrollmentManager,   EnrollmentManager>();   
            builder.Services.AddScoped<IFinancialService,    FinancialService>();
            builder.Services.AddScoped<IAuditLogService,     AuditLogService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            // ── Layer 1: Coordinator ───────────────────────────
            builder.Services.AddScoped<IEnrollmentCoordinator, EnrollmentCoordinator>();

            var app = builder.Build();

            // Seed dữ liệu demo
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
