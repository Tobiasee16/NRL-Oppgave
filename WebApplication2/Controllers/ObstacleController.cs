using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using Dapper;
using MySqlConnector;

namespace WebApplication2.Controllers
{
    public class ObstacleController : Controller
    {
        private readonly string _cs;

        public ObstacleController(IConfiguration config)
        {
            _cs = config.GetConnectionString("DefaultConnection")!;
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
                await using var conn = new MySqlConnection(_cs);

                const string insertSql = @"
INSERT INTO `obstacles`
  (ObstacleName, ObstacleHeight, ObstacleDescription, Latitude, Longitude, GeometryGeoJson)
VALUES
  (@ObstacleName, @ObstacleHeight, @ObstacleDescription, @Latitude, @Longitude, @GeometryGeoJson);
SELECT LAST_INSERT_ID();";

                var newId = await conn.ExecuteScalarAsync<int>(insertSql, obstacledata);

                // PRG: redirect til GET-Overview for å unngå resubmits ved refresh
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
            await using var conn = new MySqlConnection(_cs);
            var row = await conn.QuerySingleOrDefaultAsync<ObstacleData>(
                "SELECT * FROM `obstacles` WHERE Id = @id;", new { id });

            if (row is null) return NotFound();
            return View(row);
        }

        // (Valgfritt) /Obstacle/List for admin/oversikt
        [HttpGet]
        public async Task<IActionResult> List()
        {
            await using var conn = new MySqlConnection(_cs);
            var items = await conn.QueryAsync<ObstacleData>(
                "SELECT * FROM `obstacles` ORDER BY CreatedAt DESC;");
            return View(items);
        }
    }
}

