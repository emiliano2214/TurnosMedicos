using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace TurnosMedicos.Controllers
{
    [Authorize(Policy = "EsAdmin")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<UsuarioExt> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosController(UserManager<UsuarioExt> userManager,
                                  RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Usuarios
        public IActionResult Index() => View();

        // GET: /Usuarios/List  (para DataTables, sin VM)
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = _userManager.Users.ToList();
            var data = new List<object>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                data.Add(new
                {
                    u.Id,
                    u.Email,
                    u.DisplayName,
                    Roles = roles
                });
            }
            return Json(new { data });
        }

        // GET: /Usuarios/Edit/{id}  (Model = UsuarioExt; roles via ViewBag)
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.UserRoles = (await _userManager.GetRolesAsync(user)).ToList();

            return View(user); // Model = UsuarioExt
        }

        // POST: /Usuarios/Edit  (bind plano, sin VM)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string email, string displayName, string[] roles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Actualizar datos básicos
            user.Email = email;
            user.UserName = email;
            user.DisplayName = displayName;

            var updateRes = await _userManager.UpdateAsync(user);
            if (!updateRes.Succeeded)
            {
                foreach (var e in updateRes.Errors) ModelState.AddModelError("", e.Description);
                ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                ViewBag.UserRoles = (await _userManager.GetRolesAsync(user)).ToList();
                return View(user);
            }

            // Actualizar roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            roles ??= Array.Empty<string>();

            var toRemove = currentRoles.Except(roles).ToArray();
            var toAdd = roles.Except(currentRoles).ToArray();

            if (toRemove.Any())
                await _userManager.RemoveFromRolesAsync(user, toRemove);

            foreach (var role in toAdd)
                if (await _roleManager.RoleExistsAsync(role))
                    await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction(nameof(Index));
        }

        // POST: /Usuarios/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var res = await _userManager.DeleteAsync(user);
            if (!res.Succeeded)
                TempData["Error"] = string.Join(" | ", res.Errors.Select(e => e.Description));

            return RedirectToAction(nameof(Index));
        }
    }

}
