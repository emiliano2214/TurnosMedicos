using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TurnosMedicos.Data;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers
{
    [Authorize]
    public class TratamientoController : GenericController<Tratamiento>
    {
        public ApplicationDbContext _context {  get; set; }

        public TratamientoController(ApplicationDbContext context) : base(context) {
        
            _context = context;
        }

        // INDEX: Paciente ve solo los suyos; Medico/Admin ven todos (no hay ownership)
        public override async Task<IActionResult> Index()
        {
            IQueryable<Tratamiento> q = _context.Tratamiento
                .Include(t => t.Paciente);  // 👈 trae los datos del Paciente

            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirstValue("PacienteId");
                if (int.TryParse(pidStr, out var pid))
                    q = q.Where(t => t.IdPaciente == pid);
                else
                    q = q.Where(t => false);
            }

            var lista = await q
                .AsNoTracking()
                .OrderByDescending(t => t.FechaInicio)
                .ThenByDescending(t => t.IdTratamiento)
                .ToListAsync();

            return View(lista);
        }


        // ABM solo Admin y Medico
        [Authorize(Roles = "Admin,Medico")]
        public override IActionResult Crear()
        {
            CargarSelects();
            return base.Crear();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Medico")]
        public override async Task<IActionResult> Crear(Tratamiento entity)
        {
            if (!ModelState.IsValid)
            {
                CargarSelects();
                return View(entity);
            }
            return await base.Crear(entity);
        }

        [Authorize(Roles = "Admin,Medico")]
        public override async Task<IActionResult> Editar(int id)
        {
            CargarSelects();
            return await base.Editar(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Medico")]
        public override async Task<IActionResult> Editar(int id, Tratamiento entity)
        {
            if (!ModelState.IsValid)
            {
                CargarSelects();
                return View(entity);
            }
            return await base.Editar(id, entity);
        }

        [Authorize(Roles = "Admin,Medico")]
        public override async Task<IActionResult> Borrar(int id)
        {
            return await base.Borrar(id);
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
        }
    }
}
