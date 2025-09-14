using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class ObstacleController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        // }

        // blir kalt etter at vi trykker på "Register Obstacle" lenken i index viewet
        [HttpGet]
        public ActionResult DataForm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DataForm(ObstacleData obstacledata)
        {
            if (!ModelState.IsValid)
            {
                return View(obstacledata);
            }
            return View("Overview", obstacledata);
        }
    }
}
