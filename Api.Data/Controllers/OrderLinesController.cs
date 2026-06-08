using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Data.Repositories.OrderLineRepo;
using DataLib;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [Route("api/OrderLine")]
    [ApiController]
    [Produces("application/json")]
    public class OrderLinesController : ControllerBase
    {
        private readonly IOrderLineRepository _orderLineRepository;

        public OrderLinesController(IOrderLineRepository orderLineRepository)
        {
            _orderLineRepository = orderLineRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère les lignes de commande par commande Horizon",
            Description = "Ce endpoint permet de récupérer toutes les lignes de commande associées à une commande Horizon donnée.",
            OperationId = "GetCollectionOrderLinesByHorizonOrder"
        )]
        [HttpGet("by-horizon-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<OrderLine>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<OrderLine>>> GetCollectionOrderLinesByHorizonOrder([FromQuery] string horizonOrder)
        {
            if (string.IsNullOrWhiteSpace(horizonOrder))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'horizonOrder' est requis."
                });
            }

            try
            {
                Log.Information("GetCollectionOrderLinesByHorizonOrder called with horizonOrder: {HorizonOrder}", horizonOrder);
                var orderLines = await _orderLineRepository.GetCollectionOrderLinesByHorizonOrder(horizonOrder);
                if (orderLines == null || !orderLines.Any())
                {
                    Log.Information("GetCollectionOrderLinesByHorizonOrder returned no content for horizonOrder: {HorizonOrder}", horizonOrder);
                    return NoContent();
                }
                Log.Information("GetCollectionOrderLinesByHorizonOrder successfully returned {Count} order lines for horizonOrder: {HorizonOrder}", orderLines.Count, horizonOrder);
                return Ok(orderLines);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionOrderLinesByHorizonOrder with horizonOrder: {HorizonOrder}", horizonOrder);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
    
}
