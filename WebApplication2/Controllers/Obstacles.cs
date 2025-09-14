using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    public class Obstacles : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
