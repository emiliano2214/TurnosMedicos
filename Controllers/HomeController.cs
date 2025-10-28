using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Data;
using TurnosMedicos.Models.ViewModels;
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

        public IActionResult Index()
        {
            var resumen = new ResumenSistemaViewModel
            {
                Usuario = User.Identity?.Name ?? "Invitado",
                TotalPacientes = _context.Paciente.Count(),
                TotalMedicos = _context.Medico.Count(),
                TotalTurnos = _context.Turno.Count(),
                TurnosPendientes = _context.Turno.Count(t => t.Estado == "Pendiente"),
                TurnosConfirmados = _context.Turno.Count(t => t.Estado == "Confirmado"),
                TotalEspecialidades = _context.Especialidad.Count(),
                TotalObrasSociales = _context.ObraSocial.Count(),
                TotalConsultorios = _context.Consultorio.Count()
            };

            return View(resumen);
        }
    }
}
