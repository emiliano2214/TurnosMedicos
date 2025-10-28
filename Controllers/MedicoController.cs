using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TurnosMedicos.Models;
using TurnosMedicos.Data;
using System.Linq;

namespace TurnosMedicos.Controllers
{
    public class MedicoController : GenericController<Medico>
    {
        public MedicoController(ApplicationDbContext context) : base(context) { }

        // GET: Crear
        public override IActionResult Crear()
        {
            CargarSelects();
            return base.Crear();
        }

        // POST: Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Crear(Medico entity)
        {
            CargarSelects();
            return await base.Crear(entity);
        }

        // GET: Editar
        public override async Task<IActionResult> Editar(int id)
        {
            CargarSelects();
            return await base.Editar(id);
        }

        // POST: Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Editar(int id, Medico entity)
        {
            CargarSelects();
            return await base.Editar(id, entity);
        }

        private void CargarSelects()
        {
            // Listado de Especialidades
            ViewBag.Especialidades = _context.Set<Especialidad>()
                .Select(e => new SelectListItem
                {
                    Value = e.IdEspecialidad.ToString(),
                    Text = e.Nombre
                }).ToList();

            // Listado de Consultorios
            ViewBag.Consultorios = _context.Set<Consultorio>()
                .Select(c => new SelectListItem
                {
                    Value = c.IdConsultorio.ToString(),
                    Text = c.Numero
                }).ToList();
        }
    }
}
