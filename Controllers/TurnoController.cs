using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Models;
using TurnosMedicos.Models.ViewModels;
using TurnosMedicos.Services;

namespace TurnosMedicos.Controllers
{
    [Authorize]
    public class TurnoController : GenericController<Turno>
    {
        private readonly ApplicationDbContext _contexts;
        private readonly TurnosServicePa _pa;

        public TurnoController(ApplicationDbContext context, TurnosServicePa pa) : base(context)
        {
            _contexts = context;
            _pa = pa;
        }

        // ==========================
        //  COMPROBANTE (GET)
        // ==========================
        [HttpGet]
        [Authorize(Roles = "Admin,Administrativo,Medico,Paciente")]
        public async Task<IActionResult> Comprobante(int id)
        {
            var turno = await _contexts.Turno
                .Include(t => t.Paciente)
                .Include(t => t.Medico).ThenInclude(m => m.Especialidad)
                .FirstOrDefaultAsync(t => t.IdTurno == id);

            if (turno == null) return NotFound();

            var hc = await _contexts.HistoriaClinica
                .AsNoTracking()
                .Where(h => h.IdTurno == turno.IdTurno)
                .OrderByDescending(h => h.FechaRegistro)
                .FirstOrDefaultAsync();

            var comprobante = new SolicitarTurnoResultado
            {
                IdTurno = turno.IdTurno,
                Paciente = $"{turno.Paciente?.Nombre} {turno.Paciente?.Apellido}".Trim(),
                Medico = $"{turno.Medico?.Nombre} {turno.Medico?.Apellido}".Trim(),
                Especialidad = turno.Medico?.Especialidad?.Nombre,
                FechaEmisionTurno = DateTime.Now,
                FechaAtencion = turno.FechaHora,
                Diagnostico = string.IsNullOrWhiteSpace(hc?.Diagnostico) ? "No registrado" : hc!.Diagnostico!,
                Tratamiento = string.IsNullOrWhiteSpace(hc?.Tratamiento) ? "No registrado" : hc!.Tratamiento!,
                Estado = turno.Estado,
                FechaHora = turno.FechaHora
            };

            return View("Comprobante", comprobante);
        }

        // ==========================
        //  INDEX (override)
        // ==========================
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

        // ==========================
        //  CREAR (GET) -> TurnoForm (override)
        //  Paciente: id fijado por claim + nombre solo lectura.
        //  Admin/Staff: combo de pacientes.
        // ==========================
        [HttpGet]
        [Authorize(Roles = "Admin,Administrativo,Medico,Paciente")]
        public override IActionResult Crear()
        {
            var vm = new TurnoForm();

            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (int.TryParse(pidStr, out var pid))
                {
                    vm.IdPaciente = pid;

                    var paciente = _contexts.Paciente.FirstOrDefault(p => p.IdPaciente == pid);
                    if (paciente != null)
                    {
                        vm.NombrePaciente = $"{paciente.Nombre} {paciente.Apellido}";
                    }
                }
            }
            // Medico, Admin, Staff -> CargarSelects() se encarga de llenar la lista de pacientes


            CargarSelects();
            return View(vm);
        }

        // 🚫 Desactivar CREAR genérico como acción MVC
        [NonAction]
        public override async Task<IActionResult> Crear(Turno entity)
        {
            return await base.Crear(entity);
        }

        // ==========================
        //  CREAR (POST) -> TurnoForm
        //  Paciente: fuerza IdPaciente por claim.
        //  Admin/Staff: usa el IdPaciente del form (combo).
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Administrativo,Medico,Paciente")]
        public async Task<IActionResult> Crear(TurnoForm vm)
        {
            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (!int.TryParse(pidStr, out var pid)) return Forbid();
                vm.IdPaciente = pid;
            }
            else if (User.IsInRole("Medico"))
            {
                // Medico crea turno para sí mismo
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid)) return Forbid();
                vm.IdMedico = mid;

                // Debe seleccionar un paciente
                if (vm.IdPaciente <= 0)
                {
                    ModelState.AddModelError(nameof(vm.IdPaciente), "Debe seleccionar un paciente.");
                }
            }
            else
            {
                // Admin / Staff -> debe venir un paciente seleccionado
                if (vm.IdPaciente <= 0)
                {
                    ModelState.AddModelError(nameof(vm.IdPaciente), "Debe seleccionar un paciente.");
                }
            }

            if (!ModelState.IsValid)
            {
                CargarSelects();
                return View(vm);
            }

            var entity = new Turno
            {
                IdPaciente = vm.IdPaciente,
                IdMedico = vm.IdMedico,
                FechaHora = vm.FechaHora,
                Estado = string.IsNullOrWhiteSpace(vm.Estado) ? "Pendiente" : vm.Estado
            };

            _contexts.Add(entity);
            await _contexts.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        //  EDITAR (GET) -> TurnoForm (override)
        // ==========================
        [HttpGet]
        public override async Task<IActionResult> Editar(int id)
        {
            var turno = await _contexts.Turno
                .Include(t => t.Paciente)
                .Include(t => t.Medico).ThenInclude(m => m.Especialidad)
                .FirstOrDefaultAsync(t => t.IdTurno == id);

            if (turno is null) return NotFound();

            // Autorización
            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (!int.TryParse(pidStr, out var pid) || turno.IdPaciente != pid)
                    return Forbid();
            }
            else if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid) || turno.IdMedico != mid)
                    return Forbid();
            }

            CargarSelects();

            var hc = await _contexts.HistoriaClinica
                .AsNoTracking()
                .Where(h => h.IdTurno == turno.IdTurno)
                .OrderByDescending(h => h.FechaRegistro)
                .FirstOrDefaultAsync();

            var vm = new TurnoForm
            {
                IdTurno = turno.IdTurno,
                IdPaciente = turno.IdPaciente,
                IdMedico = turno.IdMedico,
                FechaHora = turno.FechaHora,
                Estado = turno.Estado,
                Diagnostico = hc?.Diagnostico,
                Tratamiento = hc?.Tratamiento,
                NombrePaciente = turno.Paciente != null
                    ? $"{turno.Paciente.Nombre} {turno.Paciente.Apellido}"
                    : string.Empty
            };

            return View(vm);
        }

        // 🚫 Desactivar EDITAR genérico como acción MVC
        [NonAction]
        public override async Task<IActionResult> Editar(int id, Turno entity)
        {
            return await base.Editar(id, entity);
        }

        // ==========================
        //  EDITAR (POST) -> TurnoForm
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, TurnoForm vm)
        {
            if (id != vm.IdTurno) return BadRequest();

            var actual = await _contexts.Turno
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdTurno == id);

            if (actual is null) return NotFound();

            if (User.IsInRole("Paciente"))
            {
                var pidStr = User.FindFirst("PacienteId")?.Value;
                if (!int.TryParse(pidStr, out var pid) || actual.IdPaciente != pid)
                    return Forbid();

                vm.IdPaciente = pid;
                vm.IdMedico = actual.IdMedico;
            }
            else if (User.IsInRole("Medico"))
            {
                var midStr = User.FindFirst("MedicoId")?.Value;
                if (!int.TryParse(midStr, out var mid) || actual.IdMedico != mid)
                    return Forbid();

                vm.IdPaciente = actual.IdPaciente;
                vm.IdMedico = actual.IdMedico;
            }

            if (!ModelState.IsValid)
            {
                CargarSelects();
                return View(vm);
            }

            var entity = await _contexts.Turno.FindAsync(id);
            if (entity is null) return NotFound();

            entity.IdPaciente = vm.IdPaciente;
            entity.IdMedico = vm.IdMedico;
            entity.FechaHora = vm.FechaHora;
            entity.Estado = string.IsNullOrWhiteSpace(vm.Estado) ? entity.Estado : vm.Estado;

            _contexts.Update(entity);
            await _contexts.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        //  BORRAR (override)
        // ==========================
        [Authorize(Policy = "Staff")]
        public override async Task<IActionResult> Borrar(int id)
        {
            return await base.Borrar(id);
        }

        // ==========================
        //  SELECTS (SIEMPRE CARGA LISTAS)
        // ==========================
        private void CargarSelects()
        {
            ViewBag.Pacientes = _contexts.Paciente
                .Select(p => new SelectListItem
                {
                    Value = p.IdPaciente.ToString(),
                    Text = p.Nombre + " " + p.Apellido
                })
                .ToList();

            ViewBag.Medicos = _contexts.Medico
                .Include(m => m.Especialidad)
                .Select(m => new SelectListItem
                {
                    Value = m.IdMedico.ToString(),
                    Text = m.Nombre + " " + m.Apellido + " - " + m.Especialidad.Nombre
                })
                .ToList();
        }

        // ==========================
        //  SOLICITAR (SP) – SOLO PACIENTE
        // ==========================
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

            return View();
        }

        [Authorize(Roles = "Admin,Administrativo,Paciente")]
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
                var comprobante = await _pa.SolicitarTurnoAsync(idPaciente, idEspecialidad, fechaAtencion);
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
