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

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // ⬇️ Identity con UsuarioExt y Roles
            builder.Services
                .AddDefaultIdentity<UsuarioExt>(opts =>
                {
                    opts.SignIn.RequireConfirmedAccount = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // ⬇️ Claims factory para PacienteId/MedicoId/DisplayName
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<UsuarioExt>, AppClaimsFactory>();

            // ⬇️ Policies por rol
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("EsAdmin", p => p.RequireRole("Admin"));
                options.AddPolicy("Staff", p => p.RequireRole("Administrativo"));
                options.AddPolicy("EsMedico", p => p.RequireRole("Medico"));
                options.AddPolicy("EsPaciente", p => p.RequireRole("Paciente"));
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

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

            app.UseAuthentication();
            app.UseAuthorization();

            // Redirección a Login si no autenticado (permití Identity y estáticos)
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;
                var user = context.User;

                if (!user.Identity!.IsAuthenticated &&
                    !path.StartsWithSegments("/Identity") &&
                    !path.StartsWithSegments("/css") &&
                    !path.StartsWithSegments("/js") &&
                    !path.StartsWithSegments("/lib"))
                {
                    context.Response.Redirect("/Identity/Account/Login");
                    return;
                }

                await next();
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // ⬇️ Seed con Roles y UsuarioExt
            using (var scope = app.Services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var context = sp.GetRequiredService<ApplicationDbContext>();
                var userMgr = sp.GetRequiredService<UserManager<UsuarioExt>>();
                var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();

                await DbInitializer.SeedAsync(context, userMgr, roleMgr);
            }

            app.Run();
        }
    }
}
