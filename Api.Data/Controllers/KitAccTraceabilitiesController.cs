using Api.Data.Repositories.KitAccTraceabilityRepo;
using Api.Data.Dtos;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class KitAccTraceabilitiesController : ControllerBase
    {
        private readonly IKitAccTraceabilityRepository _kitAccessoryRepository;

        public KitAccTraceabilitiesController(IKitAccTraceabilityRepository kitAccessoryRepository)
        {
            _kitAccessoryRepository = kitAccessoryRepository;
        }

        [SwaggerOperation(
            Summary = "Crée un nouvel accessoire de kit",
            Description = "Ce endpoint permet de créer un nouvel accessoire de kit en fournissant les informations nécessaires dans la requête.",
            OperationId = "CreateKitAccessory"
        )]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KitAccTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<KitAccTraceability>> CreateKitAccessory([FromBody] KitAccTraceabilityDto request)
        {
            if (request == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le corps de la requête est requis."
                });
            }
            if (string.IsNullOrWhiteSpace(request.OrderLineUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'OrderLineUid' est requis."
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

            try
            {
                Log.Information("CreateKitAccessory called with OrderLineUid: {OrderLineUid}, Product: {Product}, ClientOrder: {ClientOrder}, SerialNumber: {SerialNumber}",
                    request.OrderLineUid, request.Product, request.ClientOrder, request.SerialNumber);

                var kitAcc = new KitAccTraceability
                {
                    KitAccTraceabilityUid = Guid.NewGuid().ToString(),
                    OrderLineUid = request.OrderLineUid,
                    Product = request.Product,
                    ClientOrder = request.ClientOrder,
                    SerialNumber = request.SerialNumber,
                    UserUid = request.UserUid,
                    ScanFrame = request.ScanFrame,
                    Date = DateTimeOffset.Now,
                };

                Log.Information("Creating KitAccessory with generated UID: {KitAccTraceabilityUid}", kitAcc.KitAccTraceabilityUid);

                await _kitAccessoryRepository.CreateKitAccessory(kitAcc);

                Log.Information("Successfully created KitAccessory with UID: {KitAccTraceabilityUid}", kitAcc.KitAccTraceabilityUid);

                return Ok(kitAcc);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateKitAccessory for OrderLineUid: {OrderLineUid}", request.OrderLineUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère un accessoire de kit par ProductUid et OrderLineUid",
            Description = "Ce endpoint permet de récupérer un accessoire de kit spécifique en utilisant le ProductUid et l'OrderLineUid.",
            OperationId = "GetKitAccessoryByProductUidByOrderLineUid"
        )]
        [HttpGet("by-product-and-orderline")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KitAccTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<KitAccTraceability>> GetKitAccessoryByProductUidByOrderLineUid([FromQuery] string productUid, [FromQuery] string orderLineUid)
        {
            if (string.IsNullOrWhiteSpace(productUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'productUid' est requis."
                });
            }
            if (string.IsNullOrWhiteSpace(orderLineUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'orderLineUid' est requis."
                });
            }

            try
            {
                Log.Information("GetKitAccessoryByProductUidByOrderLineUid called with ProductUid: {ProductUid} and OrderLineUid: {OrderLineUid}", productUid, orderLineUid);

                var kitAcc = await _kitAccessoryRepository.GetKitAccessoryByProductUidByOrderLineUid(productUid, orderLineUid);

                if (kitAcc == null)
                {
                    Log.Information("KitAccessory not found for ProductUid: {ProductUid} and OrderLineUid: {OrderLineUid}", productUid, orderLineUid);
                    return NotFound();
                }

                Log.Information("GetKitAccessoryByProductUidByOrderLineUid successfully returned KitAccessory with UID: {KitAccTraceabilityUid}", kitAcc.KitAccTraceabilityUid);
                return Ok(kitAcc);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetKitAccessoryByProductUidByOrderLineUid with ProductUid: {ProductUid} and OrderLineUid: {OrderLineUid}", productUid, orderLineUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
