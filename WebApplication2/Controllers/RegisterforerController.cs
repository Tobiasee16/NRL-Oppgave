using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    // Kun registerførere skal kunne se og endre innmeldinger
    [Authorize(Roles = "Registerforer")]
    public class RegisterforerController : Controller
    {
        private readonly IObstacleRepository _obstacleRepository;

        public RegisterforerController(IObstacleRepository obstacleRepository)
        {
            _obstacleRepository = obstacleRepository;
        }

        // ----------------------------------------------------
        // LISTE / OVERSIKT
        // ----------------------------------------------------

        // Hovedsiden for registerfører: viser alle innmeldinger i tabell
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var obstacles = await _obstacleRepository.ListAsync();
            return View(obstacles);   // Views/Registerforer/Index.cshtml
        }

        // Kartoversikt over alle innmeldinger
        [HttpGet]
        public async Task<IActionResult> MapAll()
        {
            var obstacles = await _obstacleRepository.ListAsync();

            // Gjenbruker eksisterende MapAll-view som lå under Admin
            // (du kan eventuelt flytte fila til Views/Registerforer/MapAll.cshtml senere)
            return View("~/Views/Admin/MapAll.cshtml", obstacles);
        }

        // ----------------------------------------------------
        // DETALJVISNING
        // ----------------------------------------------------

        // Detaljside for én innmelding (lese alt + kart)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var obstacle = await _obstacleRepository.GetByIdAsync(id);
            if (obstacle == null)
                return NotFound();

            return View(obstacle);    // Views/Registerforer/Details.cshtml
        }

        // ----------------------------------------------------
        // REDIGERING
        // ----------------------------------------------------

        // GET: /Registerforer/Edit/5 – viser skjema med eksisterende verdier
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var obstacle = await _obstacleRepository.GetByIdAsync(id);
            if (obstacle == null)
                return NotFound();

            return View(obstacle);    // Views/Registerforer/Edit.cshtml
        }

        // POST: /Registerforer/Edit/5 – lagrer endringer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ObstacleData formModel)
        {
            if (!ModelState.IsValid)
            {
                // Viser valideringsfeil i samme skjema
                return View(formModel);
            }

            // Hent eksisterende rad slik at vi ikke overskriver CreatedAt, ReporterEmail, Status osv.
            var existing = await _obstacleRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            // Feltene registerfører faktisk skal kunne endre:
            existing.ObstacleName = formModel.ObstacleName;
            existing.ObstacleHeight = formModel.ObstacleHeight;
            existing.ObstacleDescription = formModel.ObstacleDescription;
            existing.GeometryGeoJson = formModel.GeometryGeoJson;
            existing.Latitude = formModel.Latitude;
            existing.Longitude = formModel.Longitude;

            await _obstacleRepository.UpdateAsync(existing);

            TempData["Message"] = "Innmelding oppdatert.";

            // Tilbake til detaljer for samme hinder
            return RedirectToAction(nameof(Details), new { id = existing.Id });
        }

        // ----------------------------------------------------
        // STATUS / SLETT
        // ----------------------------------------------------

        // Godkjenn en innmelding
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            await _obstacleRepository.UpdateStatusAsync(id, "Approved");
            return RedirectToAction(nameof(Index));
        }

        // Avslå en innmelding
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            await _obstacleRepository.UpdateStatusAsync(id, "Rejected");
            return RedirectToAction(nameof(Index));
        }

        // Slett en innmelding
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _obstacleRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
