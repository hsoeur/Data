using Api.Data.Dtos;
using Api.Data.Repositories.MacAddressRepo;
using Api.Data.Repositories.MeasurementTraceabilityRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMacAddressRepository _macAddressRepository;
        private readonly AppDbContext _context;

        public MeasurementTraceabilityController(
            IMeasurementTraceabilityRepository traceabilityRepository,
            IMacAddressRepository macAddressRepository,
            AppDbContext context)
        {
            _traceabilityRepository = traceabilityRepository;
            _macAddressRepository = macAddressRepository;
            _context = context;
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

        [SwaggerOperation(
            Summary = "Crée une nouvelle traçabilité de mesure avec adresse MAC",
            Description = "Ce endpoint permet de créer une nouvelle entrée de traçabilité de mesure en utilisant une adresse MAC comme valeur. L'adresse MAC sera automatiquement marquée comme utilisée.",
            OperationId = "CreateMeasurementTraceabilityWithMac"
      )]
        [HttpPost("create-with-mac")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeasurementTraceabilityWithMacResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MeasurementTraceabilityWithMacResponseDto>> CreateMeasurementTraceabilityWithMac([FromBody] MeasurementTraceabilityDto traceabilityDto)
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

            if (string.IsNullOrWhiteSpace(traceabilityDto.Value))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'Value' (adresse MAC) est requis."
                });
            }

            try
            {
                // Vérifier que l'adresse MAC existe et est disponible
                var macAddress = await _macAddressRepository.GetByMacAddressValueAsync(traceabilityDto.Value);

                if (macAddress == null)
                {
                    Log.Warning("CreateMeasurementTraceabilityWithMac - MAC address not found: {MacAddress}", traceabilityDto.Value);
                    return NotFound(new ProblemDetails
                    {
                        Title = "Adresse MAC non trouvée",
                        Detail = $"L'adresse MAC '{traceabilityDto.Value}' n'existe pas dans la base de données."
                    });
                }

                if (macAddress.Status == MacAddressStatus.USED)
                {
                    Log.Warning("CreateMeasurementTraceabilityWithMac - MAC address already used: {MacAddress}", traceabilityDto.Value);
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Adresse MAC déjà utilisée",
                        Detail = $"L'adresse MAC '{traceabilityDto.Value}' a déjà été utilisée."
                    });
                }

                var unitUid = DataLib.Unit.Unknown;
                if (!string.IsNullOrWhiteSpace(traceabilityDto.UnitUid))
                {
                    unitUid = traceabilityDto.UnitUid;
                }

                // Créer la traçabilité de mesure
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
                    ManufacturingOrderUid = traceabilityDto.ManufacturingOrderUid,
                };

                _traceabilityRepository.CreateMeasurementTraceability(traceability);

                // Marquer l'adresse MAC comme utilisée
                var updatedMacAddress = await _macAddressRepository.MarkMacAddressAsUsedAsync(traceabilityDto.Value);

                if (updatedMacAddress == null)
                {
                    Log.Error("CreateMeasurementTraceabilityWithMac - Failed to mark MAC address as used: {MacAddress}", traceabilityDto.Value);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la mise à jour de l'adresse MAC.");
                }

                Log.Information("CreateMeasurementTraceabilityWithMac successfully created measurement traceability with UID: {MeasurementTraceabilityUid} and marked MAC {MacAddress} as USED",
                        traceability.MeasurementTraceabilityUid, updatedMacAddress.MacAddressValue);

                var response = new MeasurementTraceabilityWithMacResponseDto
                {
                    MeasurementTraceability = traceability,
                    UsedMacAddress = updatedMacAddress
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateMeasurementTraceabilityWithMac");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
