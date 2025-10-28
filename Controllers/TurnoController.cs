using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers
{
    [Authorize] // requiere sesión para todo el controller
    public class TurnoController : GenericController<Turno>
    {
        private readonly ApplicationDbContext _ctx;

        public TurnoController(ApplicationDbContext context) : base(context)
        {
            _ctx = context;
        }

        // LISTA: Paciente -> sus turnos | Medico -> sus turnos | Staff -> todos
        public override async Task<IActionResult> Index()
        {
            var q = _ctx.Set<Turno>()
                .Include(t => t.Paciente)
                .Include(t => t.Medico).ThenInclude(m => m.Especialidad)
                .AsQueryable();

            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (int.TryParse(pidStr, out var pid))
                    q = q.Where(t => t.IdPaciente == pid);
            }
            else if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (int.TryParse(midStr, out var mid))
                    q = q.Where(t => t.IdMedico == mid);
            }
            // Staff ve todo

            var lista = await q.AsNoTracking().ToListAsync();
            return View(lista);
        }

        // ====== CREAR ====== (solo Paciente) 
        [Authorize(Policy = "EsPaciente")]
        public override IActionResult Crear()
        {
            CargarSelects(paraPaciente: true); // no mostrar lista de pacientes
            return base.Crear();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EsPaciente")]
        public override async Task<IActionResult> Crear(Turno entity)
        {
            // Forzar que el turno pertenezca al Paciente logueado (evita manipulación del form)
            var pidStr = User.FindFirst("PacienteId")?.Value;
            if (!int.TryParse(pidStr, out var pid)) return Forbid();
            entity.IdPaciente = pid;

            if (!ModelState.IsValid)
            {
                CargarSelects(paraPaciente: true);
                return View(entity);
            }

            return await base.Crear(entity);
        }

        // ====== EDITAR ====== (Paciente/Médico/Staff con ownership checks)
        public override async Task<IActionResult> Editar(int id)
        {
            var turno = await _ctx.Set<Turno>()
                .Include(t => t.Paciente)
                .Include(t => t.Medico).ThenInclude(m => m.Especialidad)
                .FirstOrDefaultAsync(t => t.IdTurno == id);

            if (turno is null) return NotFound();

            // Paciente solo edita sus turnos
            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (!int.TryParse(pidStr, out var pid) || turno.IdPaciente != pid)
                    return Forbid();
                // En edición de paciente, no permitimos cambiar el paciente
                CargarSelects(paraPaciente: true);
            }
            // Médico solo edita turnos que atiende
            else if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid) || turno.IdMedico != mid)
                    return Forbid();
                // Médico puede cambiar estado/fecha si querés; mostramos lista de médicos (opcional)
                CargarSelects(paraPaciente: false);
            }
            else
            {
                // Staff
                CargarSelects(paraPaciente: false);
            }

            return View(turno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Editar(int id, Turno entity)
        {
            var actual = await _ctx.Set<Turno>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdTurno == id);

            if (actual is null) return NotFound();

            // Paciente solo si es suyo y no puede cambiar IdPaciente
            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (!int.TryParse(pidStr, out var pid) || actual.IdPaciente != pid)
                    return Forbid();
                entity.IdPaciente = pid; // refuerza
            }

            // Médico solo si le pertenece (mismo IdMedico)
            if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid) || actual.IdMedico != mid)
                    return Forbid();
                // Si no querés que el médico cambie el paciente: mantener el IdPaciente original
                entity.IdPaciente = actual.IdPaciente;
                entity.IdMedico = actual.IdMedico; // evita reasignar a otro médico
            }

            if (!ModelState.IsValid)
            {
                CargarSelects(paraPaciente: User.IsInRole("Paciente"));
                return View(entity);
            }

            return await base.Editar(id, entity);
        }

        // ====== BORRAR ====== (solo Staff)
        [Authorize(Policy = "Staff")]
        public override async Task<IActionResult> Borrar(int id)
        {
            // Podés mostrar confirmación en una vista si tu GenericController no la usa.
            return await base.Borrar(id);
        }

        // -------- Helpers --------
        private void CargarSelects(bool paraPaciente)
        {
            if (!paraPaciente)
            {
                // Staff puede elegir paciente
                ViewBag.Pacientes = _ctx.Set<Paciente>()
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdPaciente.ToString(),
                        Text = p.Nombre + " " + p.Apellido
                    })
                    .ToList();
            }
            else
            {
                // Paciente no elige paciente (se fuerza por claim)
                ViewBag.Pacientes = Enumerable.Empty<SelectListItem>().ToList();
            }

            ViewBag.Medicos = _ctx.Set<Medico>()
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
