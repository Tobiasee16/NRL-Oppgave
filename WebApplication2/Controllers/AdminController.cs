using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
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

        // Godta / approve rapport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            await _repo.UpdateStatusAsync(id, "Approved");
            return RedirectToAction(nameof(Index));
        }

        // Slette rapport
        [HttpPost]
        [ValidateAntiForgeryToken]
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