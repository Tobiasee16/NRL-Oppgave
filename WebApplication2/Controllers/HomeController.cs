using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication2.Models;
using MySqlConnector;
using WebApplication2.Data;   
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration; // viktig

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _env;
        private readonly IObstacleRepository _obstacleRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration config,
            IWebHostEnvironment env,
            IObstacleRepository obstacleRepository)
        {
            _logger = logger;
            _env = env;
            _obstacleRepository = obstacleRepository;

            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IActionResult> Index()
        {
            string status;
            try
            {
                await using var conn = new MySqlConnection(_connectionString);
                await conn.OpenAsync();
                status = "✅ Connected to MariaDB successfully!";
            }
            catch (Exception ex)
            {
                status = "❌ Failed to connect to MariaDB: " + ex.Message;
            }

            return View(model: status);
        }

        [HttpGet]
        public ActionResult DataForm()
        {
            return View();
        }

        // ✅ dette er den ENESTE POST-varianten
        [HttpPost]
        public async Task<IActionResult> DataForm(ObstacleData obstacledata)
        {
            // Hent innlogget bruker (e-post)
            var reporterEmail = User.Identity?.Name;

            obstacledata.ReporterEmail = reporterEmail;
            obstacledata.CreatedAt = DateTime.UtcNow;   // hvis du har dette feltet

            // Lagre i databasen
            await _obstacleRepository.InsertAsync(obstacledata);

            // Vis oversikts-siden
            return View("Overview", obstacledata);
        }


        public ActionResult Privacy()
        {
            return View();
        }

        // Side med heatmap
        public IActionResult ObstacleHeatmap()
        {
            return View();
        }

        // PilotMode side
        public IActionResult PilotMode()
        {
            return View();
        }

        // Returnerer GeoJSON-fila som API-endepunkt
        [HttpGet]
        public IActionResult NrlPunktData()
        {
            var path = Path.Combine(_env.WebRootPath, "data", "nrl_punkt.geojson");

            if (!System.IO.File.Exists(path))
                return NotFound("GeoJSON-fil ikke funnet.");

            var json = System.IO.File.ReadAllText(path);
            return Content(json, "application/json");
        }


    }
}
