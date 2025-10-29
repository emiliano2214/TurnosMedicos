using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Models;
using TurnosMedicos.Models.ViewModels;
using TurnosMedicos.Services; // ⬅️ para usar TurnosServicePa

namespace TurnosMedicos.Controllers
{
    [Authorize]
    public class TurnoController : GenericController<Turno>
    {
        private readonly ApplicationDbContext _context;
        private readonly TurnosServicePa _sp; // servicio para ejecutar el procedimiento almacenado

        public TurnoController(ApplicationDbContext context, TurnosServicePa sp) : base(context)
        {
            _context = context;
            _sp = sp;
        }


        // CRUD ORIGINAL


        public override async Task<IActionResult> Index()
        {
            var q = _context.Set<Turno>()
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

            var lista = await q.AsNoTracking().ToListAsync();
            return View(lista);
        }

        [Authorize(Policy = "EsPaciente")]
        public override IActionResult Crear()
        {
            CargarSelects(paraPaciente: true);
            return base.Crear();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EsPaciente")]
        public override async Task<IActionResult> Crear(Turno entity)
        {
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

        public override async Task<IActionResult> Editar(int id)
        {
            var turno = await _context.Set<Turno>()
                .Include(t => t.Paciente)
                .Include(t => t.Medico).ThenInclude(m => m.Especialidad)
                .FirstOrDefaultAsync(t => t.IdTurno == id);

            if (turno is null) return NotFound();

            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (!int.TryParse(pidStr, out var pid) || turno.IdPaciente != pid)
                    return Forbid();
                CargarSelects(paraPaciente: true);
            }
            else if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid) || turno.IdMedico != mid)
                    return Forbid();
                CargarSelects(paraPaciente: false);
            }
            else
            {
                CargarSelects(paraPaciente: false);
            }

            return View(turno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Editar(int id, Turno entity)
        {
            var actual = await _context.Set<Turno>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdTurno == id);

            if (actual is null) return NotFound();

            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (!int.TryParse(pidStr, out var pid) || actual.IdPaciente != pid)
                    return Forbid();
                entity.IdPaciente = pid;
            }

            if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid) || actual.IdMedico != mid)
                    return Forbid();
                entity.IdPaciente = actual.IdPaciente;
                entity.IdMedico = actual.IdMedico;
            }

            if (!ModelState.IsValid)
            {
                CargarSelects(paraPaciente: User.IsInRole("Paciente"));
                return View(entity);
            }

            return await base.Editar(id, entity);
        }

        [Authorize(Policy = "Staff")]
        public override async Task<IActionResult> Borrar(int id)
        {
            return await base.Borrar(id);
        }

        private void CargarSelects(bool paraPaciente)
        {
            if (!paraPaciente)
            {
                ViewBag.Pacientes = _context.Set<Paciente>()
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdPaciente.ToString(),
                        Text = p.Nombre + " " + p.Apellido
                    })
                    .ToList();
            }
            else
            {
                ViewBag.Pacientes = Enumerable.Empty<SelectListItem>().ToList();
            }

            ViewBag.Medicos = _context.Set<Medico>()
                .Include(m => m.Especialidad)
                .Select(m => new SelectListItem
                {
                    Value = m.IdMedico.ToString(),
                    Text = m.Nombre + " " + m.Apellido + " - " + m.Especialidad.Nombre
                })
                .ToList();
        }

 
        // NUEVO: PROCEDIMIENTO ALMACENADO


        [Authorize(Policy = "EsPaciente")]
        [HttpGet]
        public IActionResult Solicitar()
        {
            ViewBag.Especialidades = _context.Set<Especialidad>()
                .Select(e => new SelectListItem
                {
                    Value = e.IdEspecialidad.ToString(),
                    Text = e.Nombre
                })
                .ToList();

            return View(); // View: Views/Turno/Solicitar.cshtml
        }

        [Authorize(Policy = "EsPaciente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(int idEspecialidad, DateTime fechaAtencion)
        {
            var pidStr = User.FindFirst("PacienteId")?.Value;
            if (!int.TryParse(pidStr, out var idPaciente))
                return Forbid();

            if (idEspecialidad <= 0 || fechaAtencion == default)
            {
                ModelState.AddModelError("", "Completá todos los datos requeridos.");
                return await ReRenderSolicitarForm(idEspecialidad);
            }

            try
            {
                var comprobante = await _sp.SolicitarTurnoAsync(idPaciente, idEspecialidad, fechaAtencion);
                return View("Comprobante", comprobante);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await ReRenderSolicitarForm(idEspecialidad);
            }
        }

        private async Task<IActionResult> ReRenderSolicitarForm(int idEspecialidad)
        {
            ViewBag.Especialidades = await _context.Set<Especialidad>()
                .Select(e => new SelectListItem
                {
                    Value = e.IdEspecialidad.ToString(),
                    Text = e.Nombre
                })
                .ToListAsync();

            ViewBag.SelectedEspecialidad = idEspecialidad;
            return View("Solicitar");
        }
    }
}
