﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Models;
using TurnosMedicos.Models.ViewModels; // <- asegura este using
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

            // 🔎 HC vinculada al turno (preferida)
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
        //  INDEX
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
        //  CREAR (GET) -> TurnoForm
        // ==========================
        [Authorize(Policy = "EsPaciente")]
        [HttpGet]
        public IActionResult Crear()
        {
            var vm = new TurnoForm();

            var pidStr = User.FindFirst("PacienteId")?.Value;
            if (int.TryParse(pidStr, out var pid))
                vm.IdPaciente = pid;

            CargarSelects(paraPaciente: true);
            return View(vm);
        }

        // ==========================
        //  CREAR (POST) -> TurnoForm
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EsPaciente")]
        public async Task<IActionResult> Crear(TurnoForm vm)
        {
            var pidStr = User.FindFirst("PacienteId")?.Value;
            if (!int.TryParse(pidStr, out var pid)) return Forbid();
            vm.IdPaciente = pid;

            if (!ModelState.IsValid)
            {
                CargarSelects(paraPaciente: true);
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

            // NOTA: si luego querés persistir Diagnóstico/Tratamiento, definimos bien el modelo y lo hacemos acá.
            return RedirectToAction(nameof(Index));
        }

        // ==========================
        //  EDITAR (GET) -> TurnoForm
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var turno = await _contexts.Turno
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

            var vm = new TurnoForm
            {
                IdTurno = turno.IdTurno,
                IdPaciente = turno.IdPaciente,
                IdMedico = turno.IdMedico,
                FechaHora = turno.FechaHora,
                Estado = turno.Estado,
                // Diagnostico / Tratamiento: por ahora no se cargan desde BD
            };

            return View(vm);
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
                CargarSelects(paraPaciente: User.IsInRole("Paciente"));
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

            // NOTA: cuando definamos bien el modelo clínico, acá hacemos upsert.
            return RedirectToAction(nameof(Index));
        }

        // ==========================
        //  BORRAR
        // ==========================
        [Authorize(Policy = "Staff")]
        public override async Task<IActionResult> Borrar(int id)
        {
            return await base.Borrar(id);
        }

        // ==========================
        //  SELECTS
        // ==========================
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

        // ==========================
        //  SOLICITAR (SP)
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
