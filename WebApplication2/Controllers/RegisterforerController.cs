using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Registerforer")]
    public class RegisterforerController : Controller
    {
        // Enkel oversiktsside for registerfører
        public IActionResult Index()
        {
            return View();
        }
    }
}
