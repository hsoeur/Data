using Api.Data.Dtos;
using Api.Data.Repositories.MeasurementTraceabilityRepo;
using Api.Data.Repositories.MacAddressRepo;
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
        private readonly AppDbContext _context;

        public MeasurementTraceabilityController(
            IMeasurementTraceabilityRepository traceabilityRepository,
            AppDbContext context)
        {
            _traceabilityRepository = traceabilityRepository;
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
            Summary = "Crée une nouvelle traçabilité de mesure avec une adresse MAC",
            Description = "Ce endpoint permet de créer une nouvelle entrée de traçabilité de mesure en utilisant une adresse MAC. L'adresse MAC sera verrouillée (FOR UPDATE), marquée comme USED et sa valeur sera utilisée comme Value de la mesure. Utilise une transaction pour garantir l'atomicité.",
            OperationId = "CreateMeasurementTraceabilityWithMac"
        )]
        [HttpPost("create-with-mac")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MeasurementTraceabilityWithMacResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MeasurementTraceabilityWithMacResponseDto>> CreateMeasurementTraceabilityWithMac([FromBody] MeasurementTraceabilityWithMacDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le corps de la requête est requis."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.ProductUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'ProductUid' est requis."
                });
            }

            // Démarrer une transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
         
            try
            {
                MacAddress? macAddress = null;

                // Récupérer l'adresse MAC avec verrouillage FOR UPDATE
                if (dto.UseNextAvailableMac)
                {
                    // Verrouillage de la prochaine adresse MAC disponible
                    macAddress = await _context.MacAddresses
                        .FromSqlRaw(@"
                            SELECT * FROM MacAddresses 
                            WHERE Status = 'AVAILABLE' 
                            ORDER BY Id 
                            LIMIT 1 
                            FOR UPDATE")
                        .FirstOrDefaultAsync();

                    if (macAddress == null)
                    {
                        await transaction.RollbackAsync();
                        Log.Warning("CreateMeasurementTraceabilityWithMac: No available MAC address found");
                        return NotFound(new ProblemDetails
                        {
                            Title = "Aucune adresse MAC disponible",
                            Detail = "Aucune adresse MAC disponible n'a été trouvée dans le système."
                        });
                    }
                }
                else
                {
                    if (!dto.MacAddressId.HasValue)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(new ProblemDetails
                        {
                            Title = "Paramètre invalide",
                            Detail = "Le paramètre 'MacAddressId' est requis quand 'UseNextAvailableMac' est false."
                        });
                    }

                    // Verrouillage de l'adresse MAC spécifique
                    macAddress = await _context.MacAddresses
                        .FromSqlRaw(@"
                            SELECT * FROM MacAddresses 
                            WHERE Id = {0} 
                            FOR UPDATE", dto.MacAddressId.Value)
                        .FirstOrDefaultAsync();

                    if (macAddress == null)
                    {
                        await transaction.RollbackAsync();
                        Log.Warning("CreateMeasurementTraceabilityWithMac: MAC address with ID {MacAddressId} not found", dto.MacAddressId.Value);
                        return NotFound(new ProblemDetails
                        {
                            Title = "Adresse MAC introuvable",
                            Detail = $"L'adresse MAC avec l'ID {dto.MacAddressId.Value} n'existe pas."
                        });
                    }

                    if (macAddress.Status != MacAddressStatus.AVAILABLE)
                    {
                        await transaction.RollbackAsync();
                        Log.Warning("CreateMeasurementTraceabilityWithMac: MAC address already used - Id={MacAddressId}, Status={Status}", macAddress.Id, macAddress.Status);
                        return BadRequest(new ProblemDetails
                        {
                            Title = "Adresse MAC non disponible",
                            Detail = $"L'adresse MAC {macAddress.MacAddressValue} n'est pas disponible (Status: {macAddress.Status})."
                        });
                    }
                }

                // Marquer l'adresse MAC comme utilisée
                macAddress.Status = MacAddressStatus.USED;
                macAddress.UsedAt = DateTime.UtcNow;
                _context.MacAddresses.Update(macAddress);
                await _context.SaveChangesAsync();

                var unitUid = DataLib.Unit.Unknown;
                if (!string.IsNullOrWhiteSpace(dto.UnitUid))
                {
                    unitUid = dto.UnitUid;
                }

                // Créer la traçabilité de mesure avec l'adresse MAC comme valeur
                var traceability = new MeasurementTraceability
                {
                    MeasurementTraceabilityUid = Guid.NewGuid().ToString(),
                    ProductUid = dto.ProductUid,
                    SerialNumber = dto.SerialNumber,
                    MachineUid = dto.MachineUid,
                    Label = dto.Label,
                    Value = macAddress.MacAddressValue, // Utiliser l'adresse MAC comme valeur
                    Classification = dto.Classification,
                    CreationDate = dto.CreationDate,
                    UserUid = dto.UserUid,
                    UnitUid = unitUid,
                    OperationNumber = dto.OperationNumber,
                    ManufacturingOrderUid = dto.ManufacturingOrderUid
                };

                _context.MeasurementTraceabilities.Add(traceability);
                await _context.SaveChangesAsync();

                // Valider la transaction
                await transaction.CommitAsync();

                var response = new MeasurementTraceabilityWithMacResponseDto
                {
                    MeasurementTraceability = traceability,
                    UsedMacAddress = macAddress
                };

                Log.Information("CreateMeasurementTraceabilityWithMac SUCCESS | UID={MeasurementTraceabilityUid} | MAC={MacAddress} | SN={SerialNumber} | OF={ManufacturingOrderUid}",
                    traceability.MeasurementTraceabilityUid, macAddress.MacAddressValue, traceability.SerialNumber, traceability.ManufacturingOrderUid);

                return Ok(response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, "Database error in CreateMeasurementTraceabilityWithMac");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
