using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;

namespace TurnosMedicos
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuración de servicios
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configuración del pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // ✅ Asegurate de tener esto ANTES de Authorization
            app.UseAuthorization();

            // Middleware para redirigir si no hay sesión
            app.Use(async (context, next) =>
            {
                var user = context.User;
                var path = context.Request.Path;

                // Si no está autenticado y no está yendo al login o registro
                if (!user.Identity!.IsAuthenticated &&
                    !path.StartsWithSegments("/Identity/Account/Login") &&
                    !path.StartsWithSegments("/Identity/Account/Register") &&
                    !path.StartsWithSegments("/css") &&
                    !path.StartsWithSegments("/js"))
                {
                    context.Response.Redirect("/Identity/Account/Login");
                    return;
                }

                await next();
            });

            // Rutas
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

                await DbInitializer.SeedAsync(context, userManager);
            }

            app.Run();
        }
    }
}
