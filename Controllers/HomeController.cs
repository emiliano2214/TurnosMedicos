using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Data;
using TurnosMedicos.Models.ViewModels;
using TurnosMedicos.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TurnosMedicos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new ResumenSistemaViewModel
            {
                Usuario = User.Identity?.Name ?? "Invitado"
            };

            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (int.TryParse(pidStr, out var pid))
                {
                    vm.Turnos = await _context.Turno
                        .Include(t => t.Medico).ThenInclude(m => m.Especialidad)
                        .Where(t => t.IdPaciente == pid && t.FechaHora >= DateTime.Today)
                        .OrderBy(t => t.FechaHora)
                        .Take(5)
                        .ToListAsync();
                }
            }
            else if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (int.TryParse(midStr, out var mid))
                {
                    vm.Turnos = await _context.Turno
                        .Include(t => t.Paciente)
                        .Where(t => t.IdMedico == mid && t.FechaHora >= DateTime.Today)
                        .OrderBy(t => t.FechaHora)
                        .Take(10)
                        .ToListAsync();
                }
            }
            else if (User.IsInRole("Admin") || User.IsInRole("Administrativo"))
            {
                // Solo Admin/Staff ve las estadísticas globales
                vm.TotalPacientes = await _context.Paciente.CountAsync();
                vm.TotalMedicos = await _context.Medico.CountAsync();
                vm.TotalUsuarios = await _context.Users.CountAsync();
                vm.TotalUsuariosConfirm = await _context.Users.CountAsync(u => u.EmailConfirmed);
                vm.TotalTurnos = await _context.Turno.CountAsync();
                vm.TurnosPendientes = await _context.Turno.CountAsync(t => t.Estado == "Pendiente");
                vm.TurnosConfirmados = await _context.Turno.CountAsync(t => t.Estado == "Confirmado");
                vm.TotalEspecialidades = await _context.Especialidad.CountAsync();
                vm.TotalObrasSociales = await _context.ObraSocial.CountAsync();
                vm.TotalConsultorios = await _context.Consultorio.CountAsync();
            }

            return View(vm);
        }
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> FixUserLinks()
        {
            var users = await _context.Users.ToListAsync();
            int updatedCount = 0;
            var log = new List<string>();

            foreach (var user in users)
            {
                bool changed = false;

                // 1. Check Paciente
                if (!user.PacienteId.HasValue)
                {
                    var paciente = await _context.Paciente.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (paciente != null)
                    {
                        user.PacienteId = paciente.IdPaciente;
                        changed = true;
                        log.Add($"User {user.Email}: Linked to Paciente {paciente.IdPaciente}");
                    }
                }

                // 2. Check Medico
                if (!user.MedicoId.HasValue)
                {
                    var medico = await _context.Medico.FirstOrDefaultAsync(m => m.UserId == user.Id);
                    if (medico != null)
                    {
                        user.MedicoId = medico.IdMedico;
                        changed = true;
                        log.Add($"User {user.Email}: Linked to Medico {medico.IdMedico}");
                    }
                }

                if (changed)
                {
                    _context.Update(user);
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return Content($"Proceso finalizado. Usuarios actualizados: {updatedCount}.\n\nDetalles:\n" + string.Join("\n", log));
        }
    }
}
