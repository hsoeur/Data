using Api.Data.Repositories.SimCardRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SimCardsController : ControllerBase
    {
        private readonly ISimCardRepository _repository;

        public SimCardsController(ISimCardRepository repository)
        {
            _repository = repository;
        }

        [SwaggerOperation(Summary = "Récupère toutes les cartes SIM", OperationId = "GetCollectionSimCards")]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<SimCard>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<SimCard>>> GetCollectionSimCards()
        {
            try
            {
                Log.Information("GetCollectionSimCards called");
                var items = await _repository.GetAllAsync();
                if (items == null || !items.Any())
                {
                    Log.Information("GetCollectionSimCards returned no content");
                    return NoContent();
                }
                return Ok(items);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "GetCollectionSimCards failed");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération des cartes SIM: {ex.Message}");
            }
        }

        [SwaggerOperation(Summary = "Récupère une carte SIM par UID", OperationId = "GetSimCardByUid")]
        [HttpGet("by-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SimCard))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SimCard>> GetSimCardByUid([FromQuery] string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'uid' est requis." });
            }

            try
            {
                Log.Information("GetSimCardByUid called with uid: {Uid}", uid);
                var item = await _repository.GetByIdAsync(uid);
                if (item == null)
                {
                    Log.Information("GetSimCardByUid not found for uid: {Uid}", uid);
                    return NotFound($"Carte SIM avec UID {uid} introuvable.");
                }
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "GetSimCardByUid failed for UID {Uid}", uid);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération de la carte SIM {uid}: {ex.Message}");
            }
        }

        [SwaggerOperation(Summary = "Crée une carte SIM", OperationId = "CreateSimCard")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SimCard))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SimCard>> CreateSimCard([FromBody] SimCard simCard)
        {
            if (simCard == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Generate GUID (32 chars) for Uid to match varchar(32)
                simCard.SimCardUid = Guid.NewGuid().ToString("N");
                var created = await _repository.CreateAsync(simCard);
                Log.Information("CreateSimCard created sim card with uid: {Uid}", created.SimCardUid);
                return CreatedAtAction(nameof(GetSimCardByUid), new { uid = created.SimCardUid }, created);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "CreateSimCard failed for SimCardNumber {SimCardNumber}", simCard?.SimCardNumber);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la création de la carte SIM: {ex.Message}");
            }
        }

        [SwaggerOperation(Summary = "Met à jour une carte SIM", OperationId = "UpdateSimCard")]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSimCard([FromBody] SimCard simCard)
        {
            if (simCard == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var ok = await _repository.UpdateAsync(simCard);
                if (!ok)
                {
                    Log.Information("UpdateSimCard not found for uid: {Uid}", simCard?.SimCardUid);
                    return NotFound($"Carte SIM avec UID {simCard?.SimCardUid} introuvable pour mise à jour.");
                }
                return NoContent();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "UpdateSimCard failed for UID {Uid}", simCard?.SimCardUid);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la mise à jour de la carte SIM {simCard?.SimCardUid}: {ex.Message}");
            }
        }

        [SwaggerOperation(Summary = "Supprime une carte SIM", OperationId = "DeleteSimCard")]
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSimCard([FromQuery] string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'uid' est requis." });
            }

            try
            {
                var ok = await _repository.DeleteAsync(uid);
                if (!ok)
                {
                    Log.Information("DeleteSimCard not found for uid: {Uid}", uid);
                    return NotFound($"Carte SIM avec UID {uid} introuvable pour suppression.");
                }
                return NoContent();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "DeleteSimCard failed for UID {Uid}", uid);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la suppression de la carte SIM {uid}: {ex.Message}");
            }
        }
    }
    
}
