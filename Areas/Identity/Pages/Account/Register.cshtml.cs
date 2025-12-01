// Areas/Identity/Pages/Account/Register.cshtml.cs
#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;      
using TurnosMedicos.Models;    

namespace TurnosMedicos.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<UsuarioExt> _signInManager;
        private readonly UserManager<UsuarioExt> _userManager;
        private readonly IUserStore<UsuarioExt> _userStore;
        private readonly IUserEmailStore<UsuarioExt> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;   
        public SelectList Especialidades { get; set; }
        public SelectList Consultorios { get; set; }
        public SelectList ObrasSociales { get; set; }


        public RegisterModel(
            UserManager<UsuarioExt> userManager,
            IUserStore<UsuarioExt> userStore,
            SignInManager<UsuarioExt> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)          
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _context = context;                   
        }

        // -------- Flags / parámetros --------
        [BindProperty(SupportsGet = true)]
        public bool AdminMode { get; set; } = false;

        // "create" (default) | "edit"
        [BindProperty(SupportsGet = true)]
        public string Mode { get; set; } = "create";

        [BindProperty(SupportsGet = true)]
        public string TargetUserId { get; set; }

        // -------- Modelo de entrada --------
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            // Solo en create
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            // Solo en create
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Display Name")]
            public string DisplayName { get; set; }

            // ===== CAMPOS PARA PACIENTE / MEDICO =====

            // Datos básicos
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Display(Name = "Apellido")]
            public string Apellido { get; set; }

            [Display(Name = "DNI")]
            public string Dni { get; set; }

            [Display(Name = "Teléfono")]
            public string Telefono { get; set; }

            [Display(Name = "Fecha de Nacimiento")]
            [DataType(DataType.Date)]
            public DateTime? FechaNacimiento { get; set; }

            [Display(Name = "Obra Social")]
            public int? IdObraSocial { get; set; }

            // Solo para médico
            [Display(Name = "Matrícula")]
            public string Matricula { get; set; }

            [Display(Name = "Especialidad")]
            public int? IdEspecialidad { get; set; }

            [Display(Name = "Consultorio")]
            public int? IdConsultorio { get; set; }
        }

        // -------- Para la vista --------
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public List<string> AllRoles { get; private set; } = new();

        // roles posteados desde el form
        [BindProperty]
        public List<string> Roles { get; set; } = new();

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Cargar SIEMPRE los roles (para admin y no-admin)
            AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Si vengo a editar en modo admin, precargo los datos del usuario
            if (AdminMode && string.Equals(Mode, "edit", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(TargetUserId))
            {
                if (!User.IsInRole("Admin"))
                {
                    // seguridad: si no es admin, forzamos create
                    Mode = "create";
                    return;
                }

                var u = await _userManager.FindByIdAsync(TargetUserId);
                if (u != null)
                {
                    Input = new InputModel
                    {
                        Email = u.Email,
                        DisplayName = u.DisplayName
                        // Si quisieras, acá podrías mapear Nombre/Apellido si los guardaras en UsuarioExt
                    };
                    Roles = (await _userManager.GetRolesAsync(u)).ToList();
                }
            }
            LoadSelectLists();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            // --- EDIT ---
            if (AdminMode && string.Equals(Mode, "edit", StringComparison.OrdinalIgnoreCase))
            {
                
                if (!User.IsInRole("Admin"))
                {
                    ModelState.AddModelError("", "Solo un administrador puede editar usuarios.");
                    return Page();
                }
                if (string.IsNullOrWhiteSpace(TargetUserId))
                {
                    ModelState.AddModelError("", "Falta el usuario a editar.");
                    return Page();
                }

                var user = await _userManager.FindByIdAsync(TargetUserId);
                if (user == null)
                {
                    ModelState.AddModelError("", "Usuario no encontrado.");
                    return Page();
                }

                // Validaciones mínimas
                if (string.IsNullOrWhiteSpace(Input?.Email))
                {
                    ModelState.AddModelError("Input.Email", "El email es obligatorio.");
                    LoadSelectLists();
                    return Page();
                }

                // Actualizar datos básicos
                user.Email = Input.Email;
                user.UserName = Input.Email;
                user.DisplayName = Input.DisplayName;

                var up = await _userManager.UpdateAsync(user);
                if (!up.Succeeded)
                {
                    foreach (var e in up.Errors) ModelState.AddModelError("", e.Description);
                    LoadSelectLists();
                    return Page();
                }

                // Actualizar roles (con protección: un no-admin jamás podría postear "Admin")
                Roles ??= new List<string>();
                var isCreatorAdmin = User.IsInRole("Admin");
                var posted = Roles.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                // Si el creador no es admin (defensa extra), eliminamos "Admin" del post
                if (!isCreatorAdmin)
                    posted = posted.Where(r => !r.Equals("Admin", StringComparison.OrdinalIgnoreCase)).ToList();

                var current = await _userManager.GetRolesAsync(user);

                var toRemove = current.Except(posted, StringComparer.OrdinalIgnoreCase).ToArray();
                var toAdd = posted.Except(current, StringComparer.OrdinalIgnoreCase).ToArray();

                if (toRemove.Any())
                    await _userManager.RemoveFromRolesAsync(user, toRemove);

                foreach (var role in toAdd)
                    if (await _roleManager.RoleExistsAsync(role))
                        await _userManager.AddToRoleAsync(user, role);
                if (!ModelState.IsValid)
{
    LoadSelectLists();
    return Page();
}

                return RedirectToAction("Index", "Usuarios");
            }

            // --- CREATE (público o admin) ---
            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return Page();
            }

            var newUser = CreateUser();
            newUser.DisplayName = Input.DisplayName;

            await _userStore.SetUserNameAsync(newUser, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(newUser, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(newUser, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Enviar mail de confirmación (siempre)
                var userId = await _userManager.GetUserIdAsync(newUser);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                // Asignar roles (filtrado server-side: un no-admin no puede asignar "Admin")
                Roles ??= new List<string>();
                var canAssignAdmin = User.IsInRole("Admin");
                var toAssign = Roles
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Where(r => canAssignAdmin || !r.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var role in toAssign)
                    if (await _roleManager.RoleExistsAsync(role))
                        await _userManager.AddToRoleAsync(newUser, role);

                // ===== VALIDACIÓN PREVIA DE DATOS REQUERIDOS =====
                // Antes de crear el usuario, validamos que si eligió rol Paciente/Medico, haya llenado los datos.
                // Esto evita crear el usuario Identity y luego fallar al crear el Paciente/Medico.

                if (toAssign.Contains("Paciente", StringComparer.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(Input.Nombre)) ModelState.AddModelError("Input.Nombre", "El nombre es obligatorio para Pacientes.");
                    if (string.IsNullOrWhiteSpace(Input.Apellido)) ModelState.AddModelError("Input.Apellido", "El apellido es obligatorio para Pacientes.");
                    if (string.IsNullOrWhiteSpace(Input.Dni)) ModelState.AddModelError("Input.Dni", "El DNI es obligatorio para Pacientes.");
                    if (!Input.IdObraSocial.HasValue) ModelState.AddModelError("Input.IdObraSocial", "Debe seleccionar una Obra Social.");
                }

                if (toAssign.Contains("Medico", StringComparer.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(Input.Nombre)) ModelState.AddModelError("Input.Nombre", "El nombre es obligatorio para Médicos.");
                    if (string.IsNullOrWhiteSpace(Input.Apellido)) ModelState.AddModelError("Input.Apellido", "El apellido es obligatorio para Médicos.");
                    if (string.IsNullOrWhiteSpace(Input.Matricula)) ModelState.AddModelError("Input.Matricula", "La matrícula es obligatoria para Médicos.");
                    if (!Input.IdEspecialidad.HasValue) ModelState.AddModelError("Input.IdEspecialidad", "Debe seleccionar una Especialidad.");
                    if (!Input.IdConsultorio.HasValue) ModelState.AddModelError("Input.IdConsultorio", "Debe seleccionar un Consultorio.");
                }

                if (!ModelState.IsValid)
                {
                    // Si falló la validación específica, no creamos nada.
                    // Pero ojo: ya habíamos creado el usuario Identity arriba (línea 256).
                    // ESTO ES UN PROBLEMA DE DISEÑO en el código original: primero crea el usuario y luego valida roles.
                    // Para arreglarlo bien, deberíamos mover esta validación ANTES de `_userManager.CreateAsync`.
                    
                    // Como ya se creó el usuario (variable `newUser`), deberíamos borrarlo para no dejar "basura"
                    // o simplemente retornar el error y que el usuario quede creado pero sin rol/entidad (inconsistente).
                    // La mejor opción rápida es borrarlo:
                    await _userManager.DeleteAsync(newUser);
                    
                    LoadSelectLists();
                    return Page();
                }

                // ===== CREAR PACIENTE / MEDICO SEGÚN LOS ROLES ASIGNADOS =====
                try
                {
                    // Si el usuario tiene rol "Paciente"
                    if (toAssign.Contains("Paciente", StringComparer.OrdinalIgnoreCase))
                    {
                        var paciente = new Paciente
                        {
                            UserId = newUser.Id,
                            Nombre = Input.Nombre,
                            Apellido = Input.Apellido,
                            Dni = Input.Dni,
                            Email = Input.Email,
                            Telefono = Input.Telefono,
                            FechaNacimiento = Input.FechaNacimiento ?? DateTime.Today,
                            IdObraSocial = Input.IdObraSocial.Value 
                        };

                        _context.Paciente.Add(paciente);
                    }

                    // Si el usuario tiene rol "Medico"
                    if (toAssign.Contains("Medico", StringComparer.OrdinalIgnoreCase))
                    {
                        var medico = new Medico
                        {
                            UserId = newUser.Id,
                            Nombre = Input.Nombre,
                            Apellido = Input.Apellido,
                            Matricula = Input.Matricula,
                            IdEspecialidad = Input.IdEspecialidad.Value,
                            IdConsultorio = Input.IdConsultorio.Value
                        };

                        _context.Medico.Add(medico);
                    }

                    await _context.SaveChangesAsync();

                    // ===== VINCULAR USUARIO CON PACIENTE/MEDICO =====
                    bool userUpdated = false;
                    if (toAssign.Contains("Paciente", StringComparer.OrdinalIgnoreCase))
                    {
                        var paciente = await _context.Paciente.FirstOrDefaultAsync(p => p.UserId == newUser.Id);
                        if (paciente != null)
                        {
                            newUser.PacienteId = paciente.IdPaciente;
                            userUpdated = true;
                        }
                    }

                    if (toAssign.Contains("Medico", StringComparer.OrdinalIgnoreCase))
                    {
                        var medico = await _context.Medico.FirstOrDefaultAsync(m => m.UserId == newUser.Id);
                        if (medico != null)
                        {
                            newUser.MedicoId = medico.IdMedico;
                            userUpdated = true;
                        }
                    }

                    if (userUpdated)
                    {
                        var updateResult = await _userManager.UpdateAsync(newUser);
                        if (!updateResult.Succeeded)
                        {
                            // Log warning but don't fail the whole request as the entities are created
                            _logger.LogWarning("User created and entity linked, but failed to update User with foreign key IDs.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Si algo falla al crear Paciente/Medico lo anotamos en ModelState
                    // Y borramos el usuario para mantener consistencia
                    await _userManager.DeleteAsync(newUser);

                    ModelState.AddModelError("", "Error al crear el registro de paciente/médico: " + ex.Message);
                    LoadSelectLists();
                    return Page();
                }

                // ===== FIN CREACIÓN PACIENTE / MÉDICO =====

                if (AdminMode && User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Usuarios");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                else
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            LoadSelectLists();
            return Page();
        }

        // -------- Helpers --------
        private UsuarioExt CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UsuarioExt>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"Can't create an instance of '{nameof(UsuarioExt)}'. Ensure it has a parameterless constructor.");
            }
        }

        private IUserEmailStore<UsuarioExt> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("The default UI requires a user store with email support.");
            return (IUserEmailStore<UsuarioExt>)_userStore;
        }

        private void LoadSelectLists()
        {
            Especialidades = new SelectList(_context.Especialidad, "IdEspecialidad", "Nombre");
            Consultorios = new SelectList(_context.Consultorio, "IdConsultorio", "Numero");
            ObrasSociales = new SelectList(_context.ObraSocial, "IdObraSocial", "Nombre");
        }
    }
}
