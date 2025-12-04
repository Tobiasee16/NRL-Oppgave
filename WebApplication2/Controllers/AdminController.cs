using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;

// Admin-oversikt for hindere (read-only)
namespace WebApplication2.Controllers
{
    // Kun Admin (eller legg til Registerfoerer hvis du vil at de også skal se denne)
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IObstacleRepository _repo;

        public AdminController(IObstacleRepository repo)
        {
            _repo = repo;
        }

        // Oversikt over alle rapporter (lese)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var obstacles = await _repo.ListAsync();
            return View(obstacles);
        }

        // Kartoversikt over alle rapporter (lese)
        [HttpGet]
        public async Task<IActionResult> MapAll()
        {
            var rows = await _repo.ListAsync();
            return View(rows);
        }
    }
}
