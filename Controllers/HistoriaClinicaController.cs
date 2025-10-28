using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers
{
    public class HistoriaClinicaController : GenericController<HistoriaClinica>
    {
        public HistoriaClinicaController(ApplicationDbContext context) : base(context) { }

        // LISTADO con Include para mostrar nombres en la grilla
        public override async Task<IActionResult> Index()
        {
            var items = await _context.Set<HistoriaClinica>()
                .Include(h => h.Paciente)
                .Include(h => h.Medico)
                .AsNoTracking()
                .ToListAsync();

            return View(items);
        }

        // GET: Crear -> cargar combos
        public override IActionResult Crear()
        {
            CargarSelects();
            return base.Crear();
        }

        // POST: Crear -> si hay error de validación, volver a cargar combos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Crear(HistoriaClinica entity)
        {
            if (!ModelState.IsValid)
            {
                CargarSelects();
                return View(entity);
            }
            return await base.Crear(entity);
        }

        // GET: Editar/{id} -> cargar combos
        public override async Task<IActionResult> Editar(int id)
        {
            CargarSelects();
            return await base.Editar(id);
        }

        // POST: Editar/{id} -> si hay error, recargar combos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Editar(int id, HistoriaClinica entity)
        {
            if (!ModelState.IsValid)
            {
                CargarSelects();
                return View(entity);
            }
            return await base.Editar(id, entity);
        }

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
                .Select(m => new SelectListItem
                {
                    Value = m.IdMedico.ToString(),
                    Text = m.Nombre + " " + m.Apellido
                })
                .ToList();
        }
    }
}
