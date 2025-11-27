using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Registerforer")]
    public class RegisterforerController : Controller
    {
        private readonly IObstacleRepository _obstacleRepository;

        public RegisterforerController(IObstacleRepository obstacleRepository)
        {
            _obstacleRepository = obstacleRepository;
        }

        // Hovedsiden for registerfører: viser alle innmeldinger
        public async Task<IActionResult> Index()
        {
            var obstacles = await _obstacleRepository.ListAsync();
            return View(obstacles);
        }

        // Godkjenn en innmelding
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            await _obstacleRepository.UpdateStatusAsync(id, "Approved"); // evt. "Godkjent"
            return RedirectToAction(nameof(Index));
        }

        // Avslå en innmelding
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            await _obstacleRepository.UpdateStatusAsync(id, "Rejected"); // evt. "Avslått"
            return RedirectToAction(nameof(Index));
        }

        // Slett en innmelding
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _obstacleRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
