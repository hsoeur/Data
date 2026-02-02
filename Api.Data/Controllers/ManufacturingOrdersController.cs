using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Data.Repositories.ManufacturingOrderRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ManufacturingOrderController : ControllerBase
    {
        private readonly IManufacturingOrderRepository _ordreFabricationRepository;

        public ManufacturingOrderController(IManufacturingOrderRepository ordreFabricationRepository)
        {
            _ordreFabricationRepository = ordreFabricationRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère tous les ordres de fabrication",
            Description = "Ce endpoint retourne la collection complète des ordres de fabrication.",
            OperationId = "GetCollectionManufacturingOrders"
        )]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<ManufacturingOrder>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, "Collection d'ordres de fabrication.", typeof(ICollection<ManufacturingOrder>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Aucun ordre de fabrication trouvé.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erreur interne lors de la récupération des ordres de fabrication.", typeof(string))]
        public async Task<ActionResult<ICollection<ManufacturingOrder>>> GetCollectionManufacturingOrders()
        {
            try
            {
                Log.Information("GetCollectionManufacturingOrders called");
                var ordreFabrication = await _ordreFabricationRepository.GetCollectionManufacturingOrders();
                if (ordreFabrication == null || !ordreFabrication.Any())
                {
                    Log.Information("GetCollectionManufacturingOrders returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionManufacturingOrders successfully returned {Count} ordreFabrications", ordreFabrication.Count());
                return Ok(ordreFabrication.ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionManufacturingOrders");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les ordres de fabrication par identifiant d'article",
            Description = "Ce endpoint retourne les ordres de fabrication associés à un identifiant d'article donné.",
            OperationId = "GetCollectionManufacturingOrderByArticleUid"
        )]
        [HttpGet("by-article-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<ManufacturingOrder>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, "Collection d'ordres de fabrication correspondant à l'article demandé.", typeof(ICollection<ManufacturingOrder>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Aucun ordre de fabrication pour cet article.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Paramètre manquant: `articleUid`.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erreur interne lors de la recherche des ordres par article.", typeof(string))]
        public async Task<ActionResult<ICollection<ManufacturingOrder>>> GetCollectionManufacturingOrderByArticleUid([FromQuery] string articleUid)
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
                Log.Information("GetCollectionManufacturingOrderByArticleUid called with articleUid: {ArticleUid}", articleUid);
                var ordreFabrication = await _ordreFabricationRepository.GetCollectionManufacturingOrderByArticleUid(articleUid);
                if (ordreFabrication == null || !ordreFabrication.Any())
                {
                    Log.Information("GetCollectionManufacturingOrderByArticleUid returned no content for articleUid: {ArticleUid}", articleUid);
                    return NoContent();
                }
                Log.Information("GetCollectionManufacturingOrderByArticleUid successfully returned {Count} ordreFabrications", ordreFabrication.Count());
                return Ok(ordreFabrication.ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionManufacturingOrderByArticleUid with articleUid: {ArticleUid}", articleUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère un ordre de fabrication par son identifiant",
            Description = "Ce endpoint retourne un ordre de fabrication spécifique basé sur son identifiant unique.",
            OperationId = "GetManufacturingOrderByManufacturingOrderUid"
        )]
        [HttpGet("by-manufacturing-order-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ManufacturingOrder))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, "Ordre de fabrication correspondant.", typeof(ManufacturingOrder))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Paramètre manquant: `manufacturingOrderUid`.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Aucun ordre de fabrication correspondant.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erreur interne lors de la récupération de l'ordre de fabrication.", typeof(string))]
        public async Task<ActionResult<ManufacturingOrder>> GetManufacturingOrderByManufacturingOrderUid([FromQuery] string manufacturingOrderUid)
        {
            if (string.IsNullOrWhiteSpace(manufacturingOrderUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'manufacturingOrderUid' est requis."
                });
            }

            try
            {
                Log.Information("GetManufacturingOrderByManufacturingOrderUid called with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                var ordreFabrication = await _ordreFabricationRepository.GetManufacturingOrderByManufacturingOrderUid(manufacturingOrderUid);
                if (ordreFabrication == null)
                {
                    Log.Information("Manufacturing order not found for manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return NotFound();
                }
                Log.Information("GetManufacturingOrderByManufacturingOrderUid successfully returned a manufacturing order with Uid: {Uid}", ordreFabrication.ManufacturingOrderUid);
                return Ok(ordreFabrication);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetManufacturingOrderByManufacturingOrderUid with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les ordres de fabrication par commande client",
            Description = "Ce endpoint retourne les ordres de fabrication associés à une commande client donnée.",
            OperationId = "GetCollectionManufacturingOrderByManufacturinfOrderAmphitechOrder"
        )]
        [HttpGet("by-client-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<ManufacturingOrder>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, "Collection d'ordres de fabrication pour la commande client.", typeof(ICollection<ManufacturingOrder>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Aucun ordre de fabrication pour cette commande.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Paramètre manquant: `clientOrder`.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erreur interne lors de la recherche par commande client.", typeof(string))]
        public async Task<ActionResult<ICollection<ManufacturingOrder>>> GetCollectionManufacturingOrderByManufacturinfOrderClienthOrder([FromQuery] string clientOrder)
        {
            if (string.IsNullOrWhiteSpace(clientOrder))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'amphitechOrder' est requis."
                });
            }

            try
            {
                Log.Information("GetCollectionManufacturingOrderByManufacturinfOrderAmphitechOrder called with amphitechOrder: {AmphitechOrder}", clientOrder);
                var ordreFabrication = await _ordreFabricationRepository.GetCollectionManufacturingOrderByManufacturingOrderAmphitechOrder(clientOrder);
                if (ordreFabrication == null || !ordreFabrication.Any())
                {
                    Log.Information("GetCollectionManufacturingOrderByManufacturinfOrderAmphitechOrder returned no content for amphitechOrder: {AmphitechOrder}", clientOrder);
                    return NoContent();
                }
                Log.Information("GetCollectionManufacturingOrderByManufacturinfOrderAmphitechOrder successfully returned {Count} ordreFabrications", ordreFabrication.Count());
                return Ok(ordreFabrication.ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionManufacturingOrderByManufacturinfOrderAmphitechOrder with amphitechOrder: {AmphitechOrder}", clientOrder);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les ordres de fabrication par commande Horizon",
            Description = "Ce endpoint retourne les ordres de fabrication associés à une commande Horizon donnée.",
            OperationId = "GetCollectionManufacturingOrdersByHorizonOrder"
        )]
        [HttpGet("by-horizon-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<ManufacturingOrder>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, "Collection d'ordres de fabrication pour la commande Horizon.", typeof(ICollection<ManufacturingOrder>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Aucun ordre de fabrication pour cette commande Horizon.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Paramètre manquant: `horizonOrder`.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erreur interne lors de la recherche par commande Horizon.", typeof(string))]
        public async Task<ActionResult<ICollection<ManufacturingOrder>>> GetCollectionManufacturingOrdersByHorizonOrder([FromQuery] string horizonOrder)
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
                Log.Information("GetCollectionManufacturingOrdersByHorizonOrder called with horizonOrder: {HorizonOrder}", horizonOrder);
                var list = await _ordreFabricationRepository.GetCollectionManufacturingOrdersByHorizonOrder(horizonOrder);
                list ??= new List<ManufacturingOrder>();
                if (list.Count ==0)
                {
                    Log.Information("GetCollectionManufacturingOrdersByHorizonOrder returned no content for horizonOrder: {HorizonOrder}", horizonOrder);
                    return NoContent();
                }
                Log.Information("GetCollectionManufacturingOrdersByHorizonOrder successfully returned {Count} ordreFabrications for HorizonOrder: {HorizonOrder}", list.Count, horizonOrder);
                return Ok(list);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionManufacturingOrdersByHorizonOrder with horizonOrder: {HorizonOrder}", horizonOrder);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Modifie la quantité et le premier numéro de série d'un ordre de fabrication",
            Description = "Ce endpoint met à jour la quantité et le premier numéro de série d'un ordre de fabrication spécifique.",
            OperationId = "UpdateManufacturingOrderQuantityAndFirstSerial"
        )]
        [HttpPut("update-quantity-and-first-serial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, "Mise à jour effectuée avec succès", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Paramètres manquants: `manufacturingOrderUid` et/ou `firstSerialNumber`.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Aucun ordre de fabrication correspondant.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erreur interne lors de la mise à jour de l'ordre de fabrication.", typeof(string))]
        public async Task<IActionResult> UpdateManufacturingOrderQuantityAndFirstSerial([FromQuery] string manufacturingOrderUid, [FromQuery] string quantity, [FromQuery] string firstSerialNumber, [FromQuery] string operatorUid)
        {
            if (string.IsNullOrWhiteSpace(manufacturingOrderUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'manufacturingOrderUid' est requis."
                });
            }
            if (string.IsNullOrWhiteSpace(firstSerialNumber))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'firstSerialNumber' est requis."
                });
            }

            try
            {
                Log.Information("UpdateManufacturingOrderQuantityAndFirstSerial called with manufacturingOrderUid: {ManufacturingOrderUid}, quantity: {Quantity}, firstSerialNumber: {FirstSerialNumber}, operatorUid: {OperatorUid}", manufacturingOrderUid, quantity, firstSerialNumber, operatorUid);
                var result = await _ordreFabricationRepository.UpdateManufacturingOrderQuantityAndFirstSerial(manufacturingOrderUid, quantity, firstSerialNumber, operatorUid);
                if (!result)
                {
                    Log.Information("UpdateManufacturingOrderQuantityAndFirstSerial did not find manufacturing order with uid: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return NotFound();
                }
                Log.Information("UpdateManufacturingOrderQuantityAndFirstSerial successfully updated manufacturing order with uid: {ManufacturingOrderUid}", manufacturingOrderUid);
                return Ok("Mise à jour effectuée avec succès");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing UpdateManufacturingOrderQuantityAndFirstSerial with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
