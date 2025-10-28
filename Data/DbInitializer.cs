using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Models;

namespace TurnosMedicos.Data
{
    public static class DbInitializer
    {

        public static async Task SeedAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            // Asegurarse de que la BD está creada
            await context.Database.MigrateAsync();

            // 🔹 CONSULTORIOS
            if (!context.Consultorio.Any())
            {
                context.Consultorio.Add(new Consultorio { Numero = "101", Ubicacion = "Planta Baja" });
                await context.SaveChangesAsync();
            }

            // 🔹 ESPECIALIDADES
            if (!context.Especialidad.Any())
            {
                context.Especialidad.Add(new Especialidad { Nombre = "Cardiología", Descripcion = "Enfermedades del corazón" });
                await context.SaveChangesAsync();
            }

            // 🔹 OBRAS SOCIALES
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

            // 🔹 PACIENTES
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

            // 🔹 MÉDICOS
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

            // 🔹 HISTORIA CLÍNICA
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

            // 🔹 TRATAMIENTOS
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

            // 🔹 TURNOS
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

            // 🔹 USUARIO ADMINISTRADOR
            string adminEmail = "admin@turnos.com";
            string adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // Si querés roles, podrías agregarlos aquí
                    Console.WriteLine("Usuario administrador creado correctamente.");
                }
                else
                {
                    Console.WriteLine("Error al crear usuario administrador:");
                    foreach (var error in result.Errors)
                        Console.WriteLine($" - {error.Description}");
                }
            }
        }
    }
}
