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
