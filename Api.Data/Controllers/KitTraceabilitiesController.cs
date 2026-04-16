using Api.Data.Dtos;
using Api.Data.Repositories.KitTraceabilityRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class KitTraceabilityController : ControllerBase
    {
        private readonly IKitTraceabilityRepository _kitRepository;

        public KitTraceabilityController(IKitTraceabilityRepository kitRepository)
        {
            _kitRepository = kitRepository;
        }

        [SwaggerOperation(
            Summary = "Crée un nouveau Kit",
            Description = "Ce endpoint permet de créer un nouveau kit en fournissant les informations nécessaires dans la requête.",
            OperationId = "CreateKit"
        )]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KitTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<KitTraceability>> CreateKit([FromBody] KitTraceabilityDto request)
        {
            if (request == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le corps de la requête est requis."
                });
            }
            if (request.ManufacturingOrderUid <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'ManufacturingOrderUid' est requis et doit être un entier positif."
                });
            }
            if (string.IsNullOrWhiteSpace(request.Product))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'Product' est requis."
                });
            }

            Log.Information("CreateKit called with request: {@Request}", request);

            var kit = new KitTraceability
            {
                KitTraceabilityUid = Guid.NewGuid().ToString(),
                ClientOrder = request.ClientOrder,
                ManufacturingOrderUid = request.ManufacturingOrderUid,
                Product = request.Product,
                SerialNumber = request.SerialNumber,
                ScanFrame = request.ScanFrame,
                HorizonComponentCode = request.HorizonComponentCode,
                Date = request.Date,
                UserUid = request.UserUid,
            };

            try
            {
                await _kitRepository.CreateKit(kit);
                Log.Information("CreateKit successfully created a kit with UidKit: {UidKit}", kit.KitTraceabilityUid);
                return Ok(kit);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateKit for ManufacturingOrderUid: {ManufacturingOrderUid}", request.ManufacturingOrderUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Vérifie si un kit est déjà traité",
            Description = "Ce endpoint vérifie si un kit spécifique, identifié par son uidOf, a déjà été traité.",
            OperationId = "IsAlreadyProcessed"
        )]
        [HttpGet("is-already-processed")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> IsAlreadyProcessed([FromQuery] int uidOf)
        {
            if (uidOf <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'uidOf' est requis et doit être un entier positif."
                });
            }

            try
            {
                Log.Information("IsAlreadyProcessed called with uidOf: {UidOf}", uidOf);

                bool isAlreadyProcessed = await _kitRepository.AlreadyProcessed(uidOf);
                Log.Information("IsAlreadyProcessed result for uidOf {UidOf}: {IsAlreadyProcessed}", uidOf, isAlreadyProcessed);
                return Ok(isAlreadyProcessed);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing IsAlreadyProcessed for uidOf: {UidOf}", uidOf);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère un kit par numéro d'OF et numéro de série",
            Description = "Ce endpoint permet de récupérer un kit spécifique en fournissant l'identifiant de l'ordre de fabrication et le numéro de série.",
            OperationId = "GetKitByManufacturingOrderAndSerialNumber"
        )]
        [HttpGet("by-product-serial")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KitTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<KitTraceability>> GetKitByProductAndSerialNumber(
            [FromQuery] string productUid,
            [FromQuery] int serialNumber)
        {
            

            try
            {
                Log.Information("GetKitByManufacturingOrderAndSerialNumber called with productUid: {productUid}, serialNumber: {SerialNumber}",
                    productUid, serialNumber);

                var kit = await _kitRepository.GetKitByProductUidAndSerialNumber(productUid, serialNumber);

                if (kit == null)
                {
                    Log.Information("Kit not found for manufacturingOrderUid: {ManufacturingOrderUid}, serialNumber: {SerialNumber}",
                        productUid, serialNumber);
                    return NotFound(new ProblemDetails
                    {
                        Title = "Kit non trouvé",
                        Detail = $"Aucun kit trouvé pour l'OF {productUid} et le numéro de série {serialNumber}."
                    });
                }

                Log.Information("Kit found successfully: {KitUid}", kit.KitTraceabilityUid);
                return Ok(kit);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while getting kit for productUid: {productUid}, serialNumber: {SerialNumber}",
                    productUid, serialNumber);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
