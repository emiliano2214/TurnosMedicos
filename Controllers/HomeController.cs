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
    }
}
