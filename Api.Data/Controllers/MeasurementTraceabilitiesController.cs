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
    public class MeasurementTraceabilitiesController : ControllerBase
    {
        private readonly IMeasurementTraceabilityRepository _measurementTraceabilityRepository;

        public MeasurementTraceabilitiesController(IMeasurementTraceabilityRepository measurementTraceabilityRepository)
        {
            _measurementTraceabilityRepository = measurementTraceabilityRepository;
        }

        [SwaggerOperation(Summary = "Cree une trace de mesure", OperationId = "CreateMeasurementTraceability")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeasurementTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MeasurementTraceability>> CreateMeasurementTraceability([FromBody] MeasurementTraceabilityDto request)
        {
            var validationError = ValidateMeasurementTraceability(request);
            if (validationError != null)
            {
                return validationError;
            }

            try
            {
                var created = await _measurementTraceabilityRepository.CreateMeasurementTraceability(MapToEntity(request));
                Log.Information("CreateMeasurementTraceability successfully created with uid: {MeasurementTraceabilityUid}", created.MeasurementTraceabilityUid);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Invalid measurement traceability coherence in CreateMeasurementTraceability");
                return BadRequest(new ProblemDetails { Title = "Parametre invalide", Detail = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateMeasurementTraceability");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(Summary = "Cree des traces de mesure en batch atomique", OperationId = "CreateMeasurementTraceabilityBatchAtomic")]
        [HttpPost("batch/atomic")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<MeasurementTraceability>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<MeasurementTraceability>>> CreateMeasurementTraceabilityBatchAtomic([FromBody] List<MeasurementTraceabilityDto> request)
        {
            var validationError = ValidateBatch(request);
            if (validationError != null)
            {
                return validationError;
            }

            try
            {
                var created = await _measurementTraceabilityRepository.CreateMeasurementTraceabilitiesAtomic(request.Select(MapToEntity));
                Log.Information("CreateMeasurementTraceabilityBatchAtomic successfully created {Count} records", created.Count);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                Log.Warning(ex, "Invalid measurement traceability coherence in CreateMeasurementTraceabilityBatchAtomic");
                return BadRequest(new ProblemDetails { Title = "Parametre invalide", Detail = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateMeasurementTraceabilityBatchAtomic");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(Summary = "Cree des traces de mesure en batch non atomique", OperationId = "CreateMeasurementTraceabilityBatchBestEffort")]
        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeasurementTraceabilityBatchResultDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MeasurementTraceabilityBatchResultDto>> CreateMeasurementTraceabilityBatchBestEffort([FromBody] List<MeasurementTraceabilityDto> request)
        {
            if (request == null || request.Count == 0)
            {
                return BadRequest(new ProblemDetails { Title = "Parametre invalide", Detail = "Le corps de la requete doit contenir au moins un element." });
            }

            var validEntities = new List<(int Index, MeasurementTraceability MeasurementTraceability)>();
            var result = new MeasurementTraceabilityBatchResultDto();

            for (var i = 0; i < request.Count; i++)
            {
                var error = GetValidationError(request[i]);
                if (error != null)
                {
                    result.Errors.Add(new MeasurementTraceabilityBatchErrorDto { Index = i, Item = request[i], Error = error });
                    Log.Warning("Invalid measurement traceability in best-effort batch at index {Index}: {Error}", i, error);
                    continue;
                }

                validEntities.Add((i, MapToEntity(request[i])));
            }

            try
            {
                var dbResult = await _measurementTraceabilityRepository.CreateMeasurementTraceabilitiesBestEffort(validEntities);
                result.Created.AddRange(dbResult.Created);
                result.Errors.AddRange(dbResult.Errors);

                Log.Information("CreateMeasurementTraceabilityBatchBestEffort created {CreatedCount} records and returned {ErrorCount} errors", result.Created.Count, result.Errors.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateMeasurementTraceabilityBatchBestEffort");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(Summary = "Recupere les traces de mesure par produit et operation", OperationId = "GetMeasurementTraceabilitiesByProductUidAndOperationNumber")]
        [HttpGet("by-product-and-operation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<MeasurementTraceability>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<MeasurementTraceability>>> GetMeasurementTraceabilitiesByProductUidAndOperationNumber([FromQuery] string productUid, [FromQuery] int operationNumber)
        {
            if (string.IsNullOrWhiteSpace(productUid))
            {
                return BadRequest(new ProblemDetails { Title = "Parametre invalide", Detail = "Le parametre 'productUid' est requis." });
            }

            try
            {
                var result = await _measurementTraceabilityRepository.GetCollectionByProductUidAndOperationNumber(productUid, operationNumber);
                if (result.Count == 0)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetMeasurementTraceabilitiesByProductUidAndOperationNumber");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private static BadRequestObjectResult? ValidateMeasurementTraceability(MeasurementTraceabilityDto request)
        {
            var error = GetValidationError(request);
            return error == null
                ? null
                : new BadRequestObjectResult(new ProblemDetails { Title = "Parametre invalide", Detail = error });
        }

        private static BadRequestObjectResult? ValidateBatch(List<MeasurementTraceabilityDto> request)
        {
            if (request == null || request.Count == 0)
            {
                return new BadRequestObjectResult(new ProblemDetails { Title = "Parametre invalide", Detail = "Le corps de la requete doit contenir au moins un element." });
            }

            for (var i = 0; i < request.Count; i++)
            {
                var error = GetValidationError(request[i]);
                if (error != null)
                {
                    return new BadRequestObjectResult(new ProblemDetails { Title = "Parametre invalide", Detail = $"Element {i}: {error}" });
                }
            }

            return null;
        }

        private static string? GetValidationError(MeasurementTraceabilityDto request)
        {
            if (request == null)
            {
                return "Le corps de la requete est requis.";
            }

            if (string.IsNullOrWhiteSpace(request.ProductTraceabilityUid))
            {
                return "Le parametre 'ProductTraceabilityUid' est requis.";
            }

            if (string.IsNullOrWhiteSpace(request.ToleranceUid))
            {
                return "Le parametre 'ToleranceUid' est requis.";
            }

            if (request.Value.Length > 128)
            {
                return "Le parametre 'Value' ne peut pas depasser 128 caracteres.";
            }

            if (request.Classification.Length > 32)
            {
                return "Le parametre 'Classification' ne peut pas depasser 32 caracteres.";
            }

            return null;
        }

        private static MeasurementTraceability MapToEntity(MeasurementTraceabilityDto request)
        {
            return new MeasurementTraceability
            {
                MeasurementTraceabilityUid = Guid.NewGuid().ToString(),
                ProductTraceabilityUid = request.ProductTraceabilityUid,
                ToleranceUid = request.ToleranceUid,
                Value = request.Value,
                Classification = request.Classification
            };
        }
    }
}
