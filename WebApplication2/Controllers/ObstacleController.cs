using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication2.Controllers
{
    [Authorize] // hele controlleren krever login
    public class ObstacleController : Controller
    {
        private readonly IObstacleRepository _repo;

        public ObstacleController(IObstacleRepository repo)
        {
            _repo = repo;
        }

        // GET: /Obstacle/DataForm
        [HttpGet]
        public IActionResult DataForm() => View();

        // POST: /Obstacle/DataForm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DataForm(ObstacleData obstacledata)
        {
            if (!ModelState.IsValid)
                return View(obstacledata);

            try
            {
                var newId = await _repo.InsertAsync(obstacledata);
                return RedirectToAction(nameof(Overview), new { id = newId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Database error: " + ex.Message);
                return View(obstacledata);
            }
        }

        // GET: /Obstacle/Overview/{id}
        [HttpGet]
        public async Task<IActionResult> Overview(int id)
        {
            var row = await _repo.GetByIdAsync(id);
            if (row is null) return NotFound();
            return View(row);
        }

        // (Valgfritt) /Obstacle/List for admin/oversikt
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List()
        {
            var items = await _repo.ListAsync();
            return View(items);
        }
    }
}