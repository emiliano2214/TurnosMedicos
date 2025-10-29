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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

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

        public RegisterModel(
            UserManager<UsuarioExt> userManager,
            IUserStore<UsuarioExt> userStore,
            SignInManager<UsuarioExt> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
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
                    };
                    Roles = (await _userManager.GetRolesAsync(u)).ToList();
                }
            }
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

                return RedirectToAction("Index", "Usuarios");
            }

            // --- CREATE (público o admin) ---
            if (!ModelState.IsValid)
                return Page();

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
    }
}
