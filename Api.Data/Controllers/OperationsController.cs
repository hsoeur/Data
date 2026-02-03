using Api.Data.Repositories.OperationRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OperationController : ControllerBase
    {
        private readonly IOperationRepository _processRepository;

        public OperationController(IOperationRepository processRepository)
        {
            _processRepository = processRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère les labels distincts des opérations",
            Description = "Retourne une liste de labels distincts associés aux opérations d'un produit spécifique.",
            OperationId = "GetDistinctOperationLabels"
        )]
        [HttpGet("distinct-labels-by-product-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<string>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<string>>> GetDistinctOperationLabels([FromQuery] string productUid)
        {
            if (string.IsNullOrWhiteSpace(productUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'productUid' est requis."
                });
            }

            try
            {
                Log.Information("GetDistinctOperationLabels called with productUid: {ProductUid}", productUid);
                var processes = await _processRepository.GetCollectionOperationByProductUid(productUid);
                var distinctLabels = (processes ?? new List<Operation>())
                    .Select(p => p.Label)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Distinct()
                    .ToList();
                if (distinctLabels.Count == 0)
                {
                    Log.Information("GetDistinctOperationLabels returned no content for productUid: {ProductUid}", productUid);
                    return NoContent();
                }
                Log.Information("GetDistinctOperationLabels successfully returned {Count} distinct labels", distinctLabels.Count);
                return Ok(distinctLabels);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetDistinctOperationLabels for productUid: {ProductUid}", productUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les opérations par produit",
            Description = "Retourne la collection des opérations associées à un produit donné.",
            OperationId = "GetOperationsByProductUid"
        )]
        [HttpGet("by-product-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Operation>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Operation>>> GetOperationsByProductUid([FromQuery] string productUid)
        {
            if (string.IsNullOrWhiteSpace(productUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'productUid' est requis."
                });
            }

            try
            {
                Log.Information("GetOperationsByProductUid called with productUid: {ProductUid}", productUid);
                var processes = await _processRepository.GetCollectionOperationByProductUid(productUid) ?? new List<Operation>();
                if (processes.Count == 0)
                {
                    Log.Information("GetOperationsByProductUid returned no content for productUid: {ProductUid}", productUid);
                    return NoContent();
                }
                Log.Information("GetOperationsByProductUid successfully returned {Count} operations", processes.Count);
                return Ok(processes);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetOperationsByProductUid for productUid: {ProductUid}", productUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère l'opération précédente (N-1)",
            Description = "Retourne l'opération précédente (N-1) à partir du numéro d'opération actuel fourni.",
            OperationId = "GetPreviousOperationByProductUid"
        )]
        [HttpGet("previous-by-product-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Operation))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Operation>> GetPreviousOperationByProductUid([FromQuery] string productUid, [FromQuery] int currentOperationNumber)
        {
            if (string.IsNullOrWhiteSpace(productUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'productUid' est requis."
                });
            }

            try
            {
                Log.Information("GetPreviousOperationByProductUid called with productUid: {ProductUid} and currentOperationNumber: {CurrentOperationNumber}", productUid, currentOperationNumber);
                var prevOperation = await _processRepository.GetPreviousOperationByProductUid(productUid, currentOperationNumber);
                if (prevOperation == null)
                {
                    Log.Information("GetPreviousOperationByProductUid did not find previous operation for productUid: {ProductUid} and currentOperationNumber: {CurrentOperationNumber}", productUid, currentOperationNumber);
                    return NotFound();
                }
                Log.Information("GetPreviousOperationByProductUid successfully returned operation {OperationNumber}", prevOperation.OperationNumber);
                return Ok(prevOperation);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetPreviousOperationByProductUid for productUid: {ProductUid} with currentOperationNumber: {CurrentOperationNumber}", productUid, currentOperationNumber);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les opérations basiques par produit",
            Description = "Retourne la collection d'opérations basiques liée au produit pour le test GSM.",
            OperationId = "GetCollectionOperationTestGSM"
        )]
        [HttpGet("basic-by-product-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Operation>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Operation>>> GetCollectionOperationBasic([FromQuery] string productUid)
        {
            if (string.IsNullOrWhiteSpace(productUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'productUid' est requis."
                });
            }

            try
            {
                Log.Information("GetCollectionOperationBasic called with productUid: {ProductUid}", productUid);
                var operations = await _processRepository.GetCollectionOperationBasic(productUid) ?? new List<Operation>();
                if (operations.Count == 0)
                {
                    Log.Information("GetCollectionOperationBasic returned no content for productUid: {ProductUid}", productUid);
                    return NoContent();
                }
                Log.Information("GetCollectionOperationBasic successfully returned {Count} operations", operations.Count);
                return Ok(operations);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionOperationBasic for productUid: {ProductUid}", productUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
