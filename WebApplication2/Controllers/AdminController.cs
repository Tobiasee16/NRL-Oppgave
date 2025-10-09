using Microsoft.AspNetCore.Mvc;
using Dapper;
using MySqlConnector;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class AdminController : Controller
    {
        private readonly string _cs;
        public AdminController(IConfiguration config)
        {
            _cs = config.GetConnectionString("DefaultConnection")!;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await using var conn = new MySqlConnection(_cs);
            var obstacles = await conn.QueryAsync<ObstacleData>(
                "SELECT * FROM `obstacles` ORDER BY CreatedAt DESC;");
            return View(obstacles);
        }

        [HttpGet]
        public async Task<IActionResult> MapAll()
        {
            await using var conn = new MySqlConnection(_cs);
            var rows = await conn.QueryAsync<ObstacleData>(
                "SELECT * FROM `obstacles` ORDER BY CreatedAt DESC;");
            return View(rows);
        }
    }
}
