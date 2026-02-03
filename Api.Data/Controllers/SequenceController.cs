using Api.Data.Repositories.SequenceRepo;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SequenceController : ControllerBase
    {
        private readonly ISequenceRepository _sequenceRepository;

        public SequenceController(ISequenceRepository sequenceRepository)
        {
            _sequenceRepository = sequenceRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère la dernière séquence valide par produit",
            Description = "Retourne la dernière séquence valide pour un produit et un numéro d'opération donnés.",
            OperationId = "GetLatestSequenceByProduct"
        )]
        [HttpGet("latest-by-product")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLatestSequenceByProduct([FromQuery] string productUid, [FromQuery] int operationNumber)
        {
            if (string.IsNullOrWhiteSpace(productUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'productUid' est requis." });
            }

            try
            {
                Log.Information("GetLatestSequenceByProduct called with productUid: {ProductUid}, operationNumber: {OperationNumber}", productUid, operationNumber);
                var sequence = await _sequenceRepository.GetLatestValidSequenceByProductAsync(productUid, operationNumber);
                if (sequence == null)
                {
                    Log.Information("No valid sequence found for productUid: {ProductUid}, operationNumber: {OperationNumber}", productUid, operationNumber);
                    return NotFound($"Aucune séquence valide trouvée pour le produit {productUid}");
                }
                return Ok(sequence);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetLatestSequenceByProduct");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère le contenu JSON du fichier de la dernière séquence valide",
            Description = "Lit et retourne le contenu JSON du fichier lié à la dernière séquence valide pour un produit et un numéro d'opération donnés.",
            OperationId = "GetLatestSequenceFileContentByProduct"
        )]
        [HttpGet("latest-file-content-by-product")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLatestSequenceFileContentByProduct([FromQuery] string productUid, [FromQuery] int operationNumber)
        {
            if (string.IsNullOrWhiteSpace(productUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'productUid' est requis." });
            }

            try
            {
                Log.Information("GetLatestSequenceFileContentByProduct called with ProductUid: {ProductUid}, OperationNumber: {OperationNumber}", productUid, operationNumber);
                var sequence = await _sequenceRepository.GetLatestValidSequenceByProductAsync(productUid, operationNumber);
                if (sequence == null)
                {
                    Log.Information("No valid sequence found for productUid: {ProductUid}", productUid);
                    return NotFound($"Aucune séquence valide trouvée pour le produit {productUid}");
                }
                if (string.IsNullOrWhiteSpace(sequence.File))
                {
                    Log.Information("Sequence file path is empty for sequence uid: {SequenceUid}", sequence.SequenceUid);
                    return NotFound($"Fichier JSON introuvable pour la séquence {sequence.SequenceUid}");
                }
                string file = "C:\\Sequences\\" + sequence.File;
                if (!System.IO.File.Exists(file))
                {
                    Log.Information("Sequence file does not exist at path: {File}", file);
                    return NotFound($"Fichier JSON introuvable pour la séquence {sequence.SequenceUid}");
                }
                var jsonContent = await System.IO.File.ReadAllTextAsync(file);
                using var jsonDoc = JsonDocument.Parse(jsonContent);
                return Ok(jsonContent);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Erreur lors de la lecture ou du parsing du fichier de séquence");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
