using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;
 
 // Henter alle hindere fra databasen og viser dem enten i en liste (Index) eller på kart (MapAll)
namespace WebApplication2.Controllers
{
    // Krever innlogging generelt
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IObstacleRepository _repo;

        public AdminController(IObstacleRepository repo)
        {
            _repo = repo;
        }

        // Oversikt over alle rapporter
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var obstacles = await _repo.ListAsync();
            return View(obstacles);
        }

        // Kun Admin kan godkjenne
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            await _repo.UpdateStatusAsync(id, "Approved");
            return RedirectToAction(nameof(Index));
        }

        // Kun Admin kan slette
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Kartoversikt over alle rapporter
        [HttpGet]
        public async Task<IActionResult> MapAll()
        {
            var rows = await _repo.ListAsync();
            return View(rows);
        }
    }
}