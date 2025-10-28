using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers
{
    public class TratamientoController : GenericController<Tratamiento>
    {
        public TratamientoController(ApplicationDbContext context) : base(context) { }

        // Listado con include (opcional, por si querés mostrar el nombre del paciente en la tabla)
        public override async Task<IActionResult> Index()
        {
            var datos = await _context.Set<Tratamiento>()
                .Include(t => t.Paciente)
                .AsNoTracking()
                .ToListAsync();

            return View(datos);
        }

        // GET: Crear  → cargo combos y delego al base
        public override IActionResult Crear()
        {
            CargarSelects();
            return base.Crear();
        }

        // POST: Crear  → si hay error, vuelvo a cargar combos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Crear(Tratamiento entity)
        {
            if (!ModelState.IsValid)
            {
                CargarSelects();
                return View(entity);
            }

            return await base.Crear(entity);
        }

        // GET: Editar/id  → cargo combos antes de mostrar el form
        public override async Task<IActionResult> Editar(int id)
        {
            CargarSelects();
            return await base.Editar(id);
        }

        // POST: Editar/id  → si hay error, recargo combos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Editar(int id, Tratamiento entity)
        {
            if (!ModelState.IsValid)
            {
                CargarSelects();
                return View(entity);
            }

            return await base.Editar(id, entity);
        }

        // --------- helpers ----------
        private void CargarSelects()
        {
            ViewBag.Pacientes = _context.Set<Paciente>()
                .Select(p => new SelectListItem
                {
                    Value = p.IdPaciente.ToString(),
                    Text = p.Nombre + " " + p.Apellido
                })
                .ToList();
        }
    }
}
