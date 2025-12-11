using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Models;
using TurnosMedicos.Services;

namespace TurnosMedicos
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 🔹 Cadena de conexión
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // 🔹 DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // 🔹 Identity con roles y confirmación de cuenta
            builder.Services
                .AddDefaultIdentity<UsuarioExt>(opts =>
                {
                    opts.SignIn.RequireConfirmedAccount = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // 🔹 Claims personalizados
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<UsuarioExt>, AppClaimsFactory>();

            // 🔹 Autorización y políticas por rol  (⟵ AJUSTADAS: incluyen Admin)
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("EsAdmin", p => p.RequireRole("Admin"));
                options.AddPolicy("Staff", p => p.RequireRole("Admin", "Administrativo"));
                options.AddPolicy("EsMedico", p => p.RequireRole("Admin", "Medico"));
                options.AddPolicy("EsPaciente", p => p.RequireRole("Admin", "Paciente"));
            });

            // 🔹 Servicio que ejecuta el procedimiento almacenado
            builder.Services.AddScoped<TurnosServicePa>();

            // 🔹 MVC
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // 🔹 Entorno
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // 🔹 Middleware
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // 🔹 Redirección a login si no autenticado
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

            // 🔹 Rutas
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // 🔹 Seed inicial
            using (var scope = app.Services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var context = sp.GetRequiredService<ApplicationDbContext>();
                var userMgr = sp.GetRequiredService<UserManager<UsuarioExt>>();
                var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();

                await Seed.SeedAsync(context, userMgr, roleMgr);
            }

            app.Run();
        }
    }
}
