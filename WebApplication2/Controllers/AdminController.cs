using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Brukerliste + roller
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<AdminUserViewModel>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);

                model.Add(new AdminUserViewModel
                {
                    UserId = u.Id,
                    Email = u.Email ?? u.UserName ?? "(no email)",
                    Roles = roles,
                    IsAdmin = roles.Contains("Admin"),
                    IsRegisterforer = roles.Contains("Registerforer"),
                    IsPilot = roles.Contains("Pilot")
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAsPilot(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Fjern andre "operasjonelle" roller
            await _userManager.RemoveFromRoleAsync(user, "Registerforer");
            await _userManager.AddToRoleAsync(user, "Pilot");

            TempData["Message"] = $"User {user.Email} set as Pilot.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAsRegisterforer(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.RemoveFromRoleAsync(user, "Pilot");
            await _userManager.AddToRoleAsync(user, "Registerforer");

            TempData["Message"] = $"User {user.Email} set as Registerfører.";
            return RedirectToAction(nameof(Index));
        }

        // Valgfritt: fjern alle "operasjonelle" roller (kun Pilot/Registerforer)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearOperationalRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.RemoveFromRoleAsync(user, "Pilot");
            await _userManager.RemoveFromRoleAsync(user, "Registerforer");

            TempData["Message"] = $"User {user.Email} has no operational role.";
            return RedirectToAction(nameof(Index));
        }
    }
}
