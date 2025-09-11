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
            try
            {
                await using var conn = new MySqlConnection(_connectionString);
                await conn.OpenAsync();
                return Content("Connected to MariaDB successfully!");
            }
            catch (Exception ex)
            {
                return Content("Failed to connect to MariaDB: " + ex.Message);
            }

        }

    }
}
