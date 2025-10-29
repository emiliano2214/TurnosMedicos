using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Models;

namespace TurnosMedicos.Data
{
    public static class Seed
    {
        // ✅ Firma simplificada y consistente con Program.cs
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<UsuarioExt> userMgr,
            RoleManager<IdentityRole> rolManager)
        {
            // Asegurar BD y migraciones
            await context.Database.MigrateAsync();

            // ---------- Datos maestros ----------
            if (!context.Consultorio.Any())
            {
                context.Consultorio.Add(new Consultorio { Numero = "101", Ubicacion = "Planta Baja" });
                await context.SaveChangesAsync();
            }

            if (!context.Especialidad.Any())
            {
                context.Especialidad.Add(new Especialidad { Nombre = "Cardiología", Descripcion = "Enfermedades del corazón" });
                await context.SaveChangesAsync();
            }

            if (!context.ObraSocial.Any())
            {
                context.ObraSocial.Add(new ObraSocial
                {
                    Nombre = "OSDE",
                    Plan = "210",
                    Telefono = "0800-555-6733",
                    Email = "info@osde.com",
                    Direccion = "Av. Corrientes 1234"
                });
                await context.SaveChangesAsync();
            }

            if (!context.Paciente.Any())
            {
                var obra = context.ObraSocial.First();
                context.Paciente.Add(new Paciente
                {
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Dni = "30123456",
                    Email = "juan.perez@example.com",
                    Telefono = "1134567890",
                    FechaNacimiento = new DateTime(1990, 5, 10),
                    IdObraSocial = obra.IdObraSocial
                });
                await context.SaveChangesAsync();
            }

            if (!context.Medico.Any())
            {
                var cons = context.Consultorio.First();
                var esp = context.Especialidad.First();

                context.Medico.Add(new Medico
                {
                    Nombre = "Laura",
                    Apellido = "Gómez",
                    Matricula = 45678,
                    IdEspecialidad = esp.IdEspecialidad,
                    IdConsultorio = cons.IdConsultorio
                });
                await context.SaveChangesAsync();
            }

            if (!context.HistoriaClinica.Any())
            {
                var pac = context.Paciente.First();
                var med = context.Medico.First();

                context.HistoriaClinica.Add(new HistoriaClinica
                {
                    IdPaciente = pac.IdPaciente,
                    IdMedico = med.IdMedico,
                    FechaRegistro = DateTime.Now,
                    Descripcion = "Chequeo general."
                });
                await context.SaveChangesAsync();
            }

            if (!context.Tratamiento.Any())
            {
                var pac = context.Paciente.First();

                context.Tratamiento.Add(new Tratamiento
                {
                    IdPaciente = pac.IdPaciente,
                    Descripcion = "Tratamiento con medicación básica",
                    FechaInicio = DateTime.Now.AddDays(-7),
                    FechaFin = null
                });
                await context.SaveChangesAsync();
            }

            if (!context.Turno.Any())
            {
                var pac = context.Paciente.First();
                var med = context.Medico.First();

                context.Turno.Add(new Turno
                {
                    IdPaciente = pac.IdPaciente,
                    IdMedico = med.IdMedico,
                    FechaHora = DateTime.Now.AddDays(2),
                    Estado = "Pendiente"
                });
                await context.SaveChangesAsync();
            }

            // ---------- Roles ----------
            string[] roles = new[] { "Admin", "Administrativo", "Medico", "Paciente" };
            foreach (var role in roles)
            {
                if (!await rolManager.RoleExistsAsync(role))
                {
                    await rolManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ---------- Usuario Admin (UsuarioExt) ----------
            string adminEmail = "admin@turnos.com";
            string adminPassword = "Admin123!";

            var adminUser = await userMgr.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new UsuarioExt
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    // opcional: valores extra de UsuarioExt
                    DisplayName = "Administrador"
                };

                var result = await userMgr.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    var joined = string.Join(" | ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Error creando usuario admin: {joined}");
                }
            }

            // Asegurar rol Admin
            if (!await userMgr.IsInRoleAsync(adminUser, "Admin"))
            {
                await userMgr.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
