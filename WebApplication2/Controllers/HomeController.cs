using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication2.Models;
using MySqlConnector;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //   _logger = logger;
        //}

        private readonly string _connectionString;

        public HomeController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
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

            return View(model: status); // Sender status-strengen som model
        }



        // blir kalt etter at vi trykker på "Register Obstacle" lenken i Index Viewet
        [HttpGet]
        public ActionResult DataForm()
        {
            return View();
        }

        // blir kalt etter at vi trykker på "Submit Data" knapp i DataForm viewet
        [HttpPost]
        public ActionResult DataForm(ObstacleData obstacledata)
        {
            return View("Overview", obstacledata);
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }

}
  

