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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var obstacles = await _repo.ListAsync();
            return View(obstacles);
        }

        [HttpGet]
        public async Task<IActionResult> MapAll()
        {
            var rows = await _repo.ListAsync();
            return View(rows);
        }
    }
}