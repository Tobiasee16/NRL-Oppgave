using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class ObstacleController : Controller
    {
        [HttpGet]
        public ActionResult DataForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DataForm(ObstacleData obstacledata)
        {
            if (!ModelState.IsValid)
            {
                return View(obstacledata);
            }

            // Hvis dere vil kreve kart:
            // if (string.IsNullOrWhiteSpace(obstacledata.GeometryGeoJson))
            // {
            //     ModelState.AddModelError(nameof(obstacledata.GeometryGeoJson), "Please draw the location/area on the map.");
            //     return View(obstacledata);
            // }

            // TODO: Lagre i DB hvis ønskelig (GeometryGeoJson som TEXT/LONGTEXT)

            return View("Overview", obstacledata);
        }
    }
}
