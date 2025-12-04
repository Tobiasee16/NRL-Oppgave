using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Registerforer")]
    public class RegisterforerController : Controller
    {
        private readonly IObstacleRepository _obstacleRepository;

        public RegisterforerController(IObstacleRepository obstacleRepository)
        {
            _obstacleRepository = obstacleRepository;
        }

        // Hovedsiden for registerfører: viser alle innmeldinger
        public async Task<IActionResult> Index()
        {
            var obstacles = await _obstacleRepository.ListAsync();
            return View(obstacles);
        }

        // Detaljside for en spesifikk innmelding
        public async Task<IActionResult> Details(int id)
        {
            var obstacle = await _obstacleRepository.GetByIdAsync(id);
            if (obstacle == null)
                return NotFound();

            return View(obstacle);
        }

        // --------- REDIGERING ---------

        // GET: /Registerforer/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var obstacle = await _obstacleRepository.GetByIdAsync(id);
            if (obstacle == null)
                return NotFound();

            return View(obstacle);
        }

        // POST: /Registerforer/Edit/5
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

            // Oppdater kun feltene som registerfører skal kunne endre
            existing.ObstacleName = formModel.ObstacleName;
            existing.ObstacleHeight = formModel.ObstacleHeight;
            existing.ObstacleDescription = formModel.ObstacleDescription;
            existing.GeometryGeoJson = formModel.GeometryGeoJson;
            existing.Latitude = formModel.Latitude;
            existing.Longitude = formModel.Longitude;

            await _obstacleRepository.UpdateAsync(existing);

            // Valgfritt: liten beskjed
            TempData["Message"] = "Innmelding oppdatert.";

            return RedirectToAction(nameof(Details), new { id = existing.Id });
        }

        // --------- STATUS / SLETT ---------

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

        [HttpGet]
        public async Task<IActionResult> MapAll()
        {
            var obstacles = await _obstacleRepository.ListAsync();

            // Gjenbruk Admin sitt MapAll-view
            return View("~/Views/Admin/MapAll.cshtml", obstacles);
        }

    }
}
