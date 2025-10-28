using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers
{
    [Authorize]
    public class TurnoController : GenericController<Turno>
    {
        public TurnoController(ApplicationDbContext context) : base(context) { }

        // GET: /Turno
        public override async Task<IActionResult> Index()
        {
            var turnos = await _context.Set<Turno>()
                .Include(t => t.Paciente)
                .Include(t => t.Medico).ThenInclude(m => m.Especialidad)
                .AsNoTracking()
                .ToListAsync();

            return View(turnos);
        }

        // GET: /Turno/Crear  (solo cargo selects y delego al base)
        public override IActionResult Crear()
        {
            CargarSelects();
            return base.Crear();
        }

        // POST: /Turno/Crear  (cargo selects por si falla ModelState y delego al base)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Crear(Turno entity)
        {
            CargarSelects();
            return await base.Crear(entity);
        }

        // GET: /Turno/Editar/{id}
        public override async Task<IActionResult> Editar(int id)
        {
            CargarSelects();
            return await base.Editar(id);
        }

        // POST: /Turno/Editar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Editar(int id, Turno entity)
        {
            CargarSelects();
            return await base.Editar(id, entity);
        }

        // -------- Helpers --------
        private void CargarSelects()
        {
            ViewBag.Pacientes = _context.Set<Paciente>()
                .Select(p => new SelectListItem
                {
                    Value = p.IdPaciente.ToString(),
                    Text = p.Nombre + " " + p.Apellido
                })
                .ToList();

            ViewBag.Medicos = _context.Set<Medico>()
                .Include(m => m.Especialidad)
                .Select(m => new SelectListItem
                {
                    Value = m.IdMedico.ToString(),
                    Text = m.Nombre + " " + m.Apellido + " - " + m.Especialidad.Nombre
                })
                .ToList();
        }
    }
}
