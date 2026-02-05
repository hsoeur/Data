using Api.Data.Repositories.DatabaseNotificationRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Api.Data.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DatabaseNotificationController : ControllerBase
    {
        private readonly IDatabaseNotificationRepository _repository;

        public DatabaseNotificationController(
            IDatabaseNotificationRepository repository,
            ILogger<DatabaseNotificationController> logger)
        {
            _repository = repository;
        }

        // 1. GET: api/DatabaseNotification/GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var notifications = await _repository.GetAllAsync();

                Log.Information("[DatabaseNotificationController] Récupération de {Count} notifications", notifications.Count());

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[DatabaseNotificationController] Erreur dans GetAll()");
                return StatusCode(500, "Erreur interne lors de la récupération des notifications.");
            }
        }

        // 2. POST: api/DatabaseNotification/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DatabaseNotification notif)
        {
            if (notif == null)
                return BadRequest("Notification invalide.");

            try
            {
                var created = await _repository.CreateAsync(notif);

                Log.Information("[DatabaseNotificationController] Notification créée (Id={DatabaseNotificationUid})", created.DatabaseNotificationUid);

                return CreatedAtAction(nameof(GetAll), new { id = created.DatabaseNotificationUid }, created);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[DatabaseNotificationController] Erreur dans Create()");
                return StatusCode(500, "Erreur interne lors de la création de la notification.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Le mot-clé ne peut pas être vide.");

            try
            {
                var results = await _repository.SearchAsync(keyword);

                Log.Information("[DatabaseNotificationController] Recherche '{Keyword}' - {Count} résultats", keyword, results.Count());

                return Ok(results);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[DatabaseNotificationController] Erreur dans Search('{Keyword}')", keyword);
                return StatusCode(500, "Erreur interne lors de la recherche des notifications.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Process([FromQuery] string uid, [FromQuery] string processedBy)
        {
            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(processedBy))
                return BadRequest("Uid et processedBy doivent être fournis.");

            try
            {
                var notif = await _repository.ProcessAsync(uid, processedBy);

                if (notif == null)
                    return NotFound($"Notification {uid} introuvable.");

                return Ok(notif);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[DatabaseNotificationController] Erreur dans Process()");
                return StatusCode(500, "Erreur interne lors du traitement de la notification.");
            }
        }

    }
}
