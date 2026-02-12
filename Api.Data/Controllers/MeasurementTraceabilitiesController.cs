using Api.Data.Dtos;
using Api.Data.Repositories.MeasurementTraceabilityRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class MeasurementTraceabilityController : ControllerBase
    {
        private readonly IMeasurementTraceabilityRepository _traceabilityRepository;

        public MeasurementTraceabilityController(IMeasurementTraceabilityRepository traceabilityRepository)
        {
            _traceabilityRepository = traceabilityRepository;
        }

        [SwaggerOperation(
            Summary = "Crée une nouvelle traçabilité de mesure",
            Description = "Ce endpoint permet de créer une nouvelle entrée de traçabilité de mesure en fournissant les informations nécessaires dans la requête.",
            OperationId = "CreateMeasurementTraceability"
        )]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeasurementTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<MeasurementTraceability> CreateMeasurementTraceability([FromBody] MeasurementTraceabilityDto traceabilityDto)
        {
            if (traceabilityDto == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le corps de la requête est requis."
                });
            }
            if (string.IsNullOrWhiteSpace(traceabilityDto.ProductUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'ProductUid' est requis."
                });
            }

            var unitUid = DataLib.Unit.Unknown;
            if (!string.IsNullOrWhiteSpace(traceabilityDto.UnitUid))
            {
                unitUid = traceabilityDto.UnitUid;
            }

            var traceability = new MeasurementTraceability
            {
                MeasurementTraceabilityUid = Guid.NewGuid().ToString(),
                ProductUid = traceabilityDto.ProductUid,
                SerialNumber = traceabilityDto.SerialNumber,
                MachineUid = traceabilityDto.MachineUid,
                Label = traceabilityDto.Label,
                Value = traceabilityDto.Value,
                Classification = traceabilityDto.Classification,
                CreationDate = traceabilityDto.CreationDate,
                UserUid = traceabilityDto.UserUid,
                UnitUid = unitUid,
                OperationNumber = traceabilityDto.OperationNumber,
            };

            try
            {
                _traceabilityRepository.CreateMeasurementTraceability(traceability);
                Log.Information("CreateMeasurementTraceability successfully created measurement traceability with UID: {MeasurementTraceabilityUid}", traceability.MeasurementTraceabilityUid);
                return Ok(traceability);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateMeasurementTraceability");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les traçabilités de mesure par ArticleUid et OperationUid",
            Description = "Ce endpoint retourne une collection de traçabilités de mesure correspondant à un identifiant d'article et d'opération donnés.",
            OperationId = "GetCollectionMeasurementTraceabilityByArticleUidByOperationUid"
        )]
        [HttpGet("by-article-and-operation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<MeasurementTraceability>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<MeasurementTraceability>>> GetCollectionMeasurementTraceabilityByArticleUidByOperationUid([FromQuery] string articleUid, [FromQuery] int operationNumber)
        {
            if (string.IsNullOrWhiteSpace(articleUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'articleUid' est requis."
                });
            }

            try
            {
                Log.Information("GetCollectionMeasurementTraceabilityByArticleUidByOperationUid called with articleUid: {ArticleUid}, operationNumber: {OperationNumber}", articleUid, operationNumber);
                var measurementTraceabilities = await _traceabilityRepository.GetCollectionMeasurementTraceabilityByArticleUidByOperationUid(articleUid, operationNumber);
                if (measurementTraceabilities == null || !measurementTraceabilities.Any())
                {
                    Log.Information("GetCollectionMeasurementTraceabilityByArticleUidByOperationUid returned no content for articleUid: {ArticleUid}, operationNumber: {OperationNumber}", articleUid, operationNumber);
                    return NoContent();
                }
                Log.Information("GetCollectionMeasurementTraceabilityByArticleUidByOperationUid successfully returned {Count} measurement traceabilities", measurementTraceabilities.Count);
                return Ok(measurementTraceabilities);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionMeasurementTraceabilityByArticleUidByOperationUid with articleUid: {ArticleUid}, operationNumber: {OperationNumber}", articleUid, operationNumber);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
