using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    // Alle actions krever innlogging, roller spesifiseres per action
    [Authorize]
    public class ObstacleController : Controller
    {
        private readonly IObstacleRepository _repo;

        public ObstacleController(IObstacleRepository repo)
        {
            _repo = repo;
        }

        // ----------------------------------------------------
        // PILOT: MELDE INN
        // ----------------------------------------------------

        // GET: /Obstacle/DataForm
        [HttpGet]
        [Authorize(Roles = "Pilot")]
        public IActionResult DataForm()
        {
            ModelState.Clear(); // fjerner automatisk validering i GET
            return View(new ObstacleData());
        }

        // POST: /Obstacle/DataForm
        [HttpPost]
        [Authorize(Roles = "Pilot")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DataForm(ObstacleData obstacledata)
        {
            if (!ModelState.IsValid)
                return View(obstacledata);

            try
            {
                // Hent innlogget pilot sin bruker-ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    // Skal i praksis ikke skje hvis brukeren er innlogget
                    return Forbid();
                }

                // Lagre hvem som meldte inn hindret
                obstacledata.SubmittedByUserId = userId;

                // (Valgfritt) lagre også e-post hvis du fortsatt bruker feltet
                var reporterEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                obstacledata.ReporterEmail = reporterEmail;

                // Sett standard-status hvis den ikke er satt
                if (string.IsNullOrEmpty(obstacledata.Status))
                    obstacledata.Status = "Pending";

                // Lagre og få tilbake ID
                var newId = await _repo.InsertAsync(obstacledata);

                // Etter innsending: kvitteringsside for denne innmeldingen
                return RedirectToAction(nameof(Overview), new { id = newId });

                // Eller, hvis du heller vil til liste:
                // return RedirectToAction(nameof(MyReports));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Database error: " + ex.Message);
                return View(obstacledata);
            }
        }

        // ----------------------------------------------------
        // PILOT: EGNE INNMELDINGER
        // ----------------------------------------------------

        // GET: /Obstacle/MyReports – viser KUN egne innmeldinger (lesevisning)
        [HttpGet]
        [Authorize(Roles = "Pilot")]
        public async Task<IActionResult> MyReports()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Burde egentlig ikke skje hvis brukeren er innlogget
                return Forbid();
            }

            // Bruker eksisterende repo-metode: GetByReporterAsync(userId)
            var myObstacles = await _repo.GetByReporterAsync(userId);
            return View(myObstacles);   // Views/Obstacle/MyReports.cshtml
        }

        // ----------------------------------------------------
        // PILOT: KVITTERING / OVERSIKT FOR ÉN INNMELDING
        // ----------------------------------------------------

        // GET: /Obstacle/Overview/{id}
        [HttpGet]
        [Authorize(Roles = "Pilot")]
        public async Task<IActionResult> Overview(int id)
        {
            var row = await _repo.GetByIdAsync(id);
            if (row is null) return NotFound();

            // (Valgfritt) ekstra sikkerhet: sjekk at dette faktisk er pilotens egen innmelding
            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // if (!string.Equals(row.SubmittedByUserId, userId, StringComparison.OrdinalIgnoreCase))
            //     return Forbid();

            return View(row);
        }

        // ----------------------------------------------------
        // ADMIN: FULL LISTE
        // ----------------------------------------------------

        // GET: /Obstacle/List – kun Admin (full oversikt)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List()
        {
            var items = await _repo.ListAsync();
            return View(items);
        }
    }
}
