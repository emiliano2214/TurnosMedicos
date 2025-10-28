using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers
{
    [Authorize] // todos logueados pueden entrar al Index; abajo filtramos por rol/claim
    public class HistoriaClinicaController : GenericController<HistoriaClinica>
    {
        public HistoriaClinicaController(ApplicationDbContext context) : base(context) { }

        // INDEX: Paciente ve solo las suyas; Medico ve las que cargó; Admin ve todas
        public override async Task<IActionResult> Index()
        {
            var q = _context.Set<HistoriaClinica>()
                .Include(h => h.Paciente)
                .Include(h => h.Medico)
                .AsQueryable();

            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (int.TryParse(pidStr, out var pid))
                    q = q.Where(h => h.IdPaciente == pid);
            }
            else if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (int.TryParse(midStr, out var mid))
                    q = q.Where(h => h.IdMedico == mid);
            }
            // Admin: sin filtro

            var lista = await q.AsNoTracking().ToListAsync();
            return View(lista);
        }

        // ---------- CREAR ----------
        // Solo Admin y Medico pueden crear
        [Authorize(Roles = "Admin,Medico")]
        public override IActionResult Crear()
        {
            CargarSelects(paraMedico: User.IsInRole("Medico"));
            return base.Crear();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Medico")]
        public override async Task<IActionResult> Crear(HistoriaClinica entity)
        {
            // Si es médico, se fuerza que la historia quede asociada a SU MedicoId
            if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid)) return Forbid();
                entity.IdMedico = mid;
            }

            if (!ModelState.IsValid)
            {
                CargarSelects(paraMedico: User.IsInRole("Medico"));
                return View(entity);
            }

            return await base.Crear(entity);
        }

        // ---------- EDITAR ----------
        [Authorize(Roles = "Admin,Medico")]
        public override async Task<IActionResult> Editar(int id)
        {
            var model = await _context.Set<HistoriaClinica>()
                .Include(h => h.Paciente)
                .Include(h => h.Medico)
                .FirstOrDefaultAsync(h => h.IdHistoria == id);

            if (model is null) return NotFound();

            // Si es médico, solo puede editar historias que él cargó
            if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid) || model.IdMedico != mid)
                    return Forbid();
            }

            CargarSelects(paraMedico: User.IsInRole("Medico"), model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Medico")]
        public override async Task<IActionResult> Editar(int id, HistoriaClinica entity)
        {
            var original = await _context.Set<HistoriaClinica>()
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.IdHistoria == id);

            if (original is null) return NotFound();

            if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid) || original.IdMedico != mid)
                    return Forbid();

                // Evitar que el médico “cambie” el médico de la historia
                entity.IdMedico = mid;
            }

            if (!ModelState.IsValid)
            {
                CargarSelects(paraMedico: User.IsInRole("Medico"), entity);
                return View(entity);
            }

            return await base.Editar(id, entity);
        }

        // ---------- BORRAR ----------
        [Authorize(Roles = "Admin,Medico")]
        public override async Task<IActionResult> Borrar(int id)
        {
            var model = await _context.Set<HistoriaClinica>()
                .Include(h => h.Paciente)
                .Include(h => h.Medico)
                .FirstOrDefaultAsync(h => h.IdHistoria == id);

            if (model is null) return NotFound();

            if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid) || model.IdMedico != mid)
                    return Forbid();
            }

            return View(model);
        }

        // (si tu GenericController ya borra en GET, dejalo; si tenés POST Confirmado, replicá el check de ownership)

        // ---------- Helpers ----------
        private void CargarSelects(bool paraMedico, HistoriaClinica? model = null)
        {
            // Pacientes: siempre se listan (Admin y Medico pueden elegir)
            ViewBag.Pacientes = _context.Set<Paciente>()
                .Select(p => new SelectListItem
                {
                    Value = p.IdPaciente.ToString(),
                    Text = p.Nombre + " " + p.Apellido,
                    Selected = model != null && model.IdPaciente == p.IdPaciente
                })
                .ToList();

            if (paraMedico)
            {
                // Médico: no debe elegir otro médico; se fuerza por claim
                ViewBag.Medicos = new List<SelectListItem>();
            }
            else
            {
                // Admin: puede elegir médico
                ViewBag.Medicos = _context.Set<Medico>()
                    .Select(m => new SelectListItem
                    {
                        Value = m.IdMedico.ToString(),
                        Text = m.Nombre + " " + m.Apellido,
                        Selected = model != null && model.IdMedico == m.IdMedico
                    })
                    .ToList();
            }
        }
    }
}
