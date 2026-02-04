using Api.Data.Dtos;
using Api.Data.Repositories.OperationRepo;
using Api.Data.Repositories.ProductTraceabilityRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProductTraceabilitiesController : ControllerBase
    {

        private readonly IProductTraceabilityRepository _tracabiliteProduitRepository;
        private readonly IOperationRepository _operationRepository;

        public ProductTraceabilitiesController(IProductTraceabilityRepository tracabiliteProduitRepository, IOperationRepository operationRepository)
        {
            _tracabiliteProduitRepository = tracabiliteProduitRepository;
            _operationRepository = operationRepository;
        }

        [SwaggerOperation(
            Summary = "Crée une nouvelle entrée de traçabilité produit",
            Description = "Ce endpoint permet de créer une nouvelle entrée de traçabilité produit en fournissant les informations nécessaires.",
            OperationId = "CreateProductTraceability"
        )]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ProductTraceability> CreateProductTraceability([FromBody] ProductTraceabilityDto tracabiliteProduitDto)
        {
            if (tracabiliteProduitDto == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }
            if (string.IsNullOrWhiteSpace(tracabiliteProduitDto.ProductUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'ProductUid' est requis." });
            }

            var tracabiliteProduit = new ProductTraceability
            {
                ProductTraceabilityUid = Guid.NewGuid().ToString(),
                ProductUid = tracabiliteProduitDto.ProductUid,
                ProductSerialNumber = tracabiliteProduitDto.ProductSerialNumber,
                OperationNumber = tracabiliteProduitDto.OperationNumber,
                ProductStatusUid = tracabiliteProduitDto.ProductStatusUid,
                CreationDate = tracabiliteProduitDto.CreationDate,
                ManufacturingOrderUid = tracabiliteProduitDto.ManufacturingOrderUid,
                UserUid = tracabiliteProduitDto.UserUid,
                CycleTime = tracabiliteProduitDto.CycleTime,
                MachineUid = tracabiliteProduitDto.MachineUid
            };

            try
            {
                _tracabiliteProduitRepository.CreateProductTraceability(tracabiliteProduit);
                Log.Information("CreateProductTraceability successfully created with uid: {ProductTraceabilityUid}", tracabiliteProduit.ProductTraceabilityUid);
                return Ok(tracabiliteProduit);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateProductTraceability");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + " [Fields Unit, MachineUid, and ProductStatusUid MUST EXIST]");
            }
        }

        [SwaggerOperation(
            Summary = "Récupère l'opération précédente et vérifie le statut OK dans la traçabilité",
            Description = "Retourne true si l'enregistrement de traçabilité (produit, numéro de série, opération précédente) a un statut OK.",
            OperationId = "HasPreviousOperationOkStatus"
        )]
        [HttpGet("has-previous-operation-ok-status")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> HasPreviousOperationOkStatus([FromQuery] string productUid, [FromQuery] string serialNumber, [FromQuery] string currentOperationNumber)
        {
            if (string.IsNullOrWhiteSpace(productUid) || string.IsNullOrWhiteSpace(serialNumber) || string.IsNullOrWhiteSpace(currentOperationNumber))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Les paramètres 'productUid', 'serialNumber' et 'currentOperationNumber' sont requis." });
            }

            if (!int.TryParse(currentOperationNumber, out var currentOpNum))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'currentOperationNumber' doit être un entier." });
            }

            try
            {
                Log.Information("HasPreviousOperationOkStatus called with productUid: {ProductUid}, serialNumber: {SerialNumber}, currentOperationNumber: {CurrentOperationNumber}", productUid, serialNumber, currentOperationNumber);
                var previousOperation = await _operationRepository.GetPreviousOperationByProductUid(productUid, currentOpNum);
                if (previousOperation == null)
                {
                    Log.Information("No previous operation found for productUid: {ProductUid}", productUid);
                    return NotFound("Aucune opération précédente trouvée.");
                }
                var latestTrace = await _tracabiliteProduitRepository.GetProductTraceabilityByProductBySerialNumberByOperationLatestOne(productUid, serialNumber, previousOperation.OperationNumber.ToString());
                if (latestTrace == null)
                {
                    Log.Information("No traceability found for previous operation. productUid: {ProductUid}, serialNumber: {SerialNumber}", productUid, serialNumber);
                    return NotFound("Aucune traçabilité trouvée pour l'opération précédente.");
                }
                var isOk = string.Equals(latestTrace.ProductStatusUid, "OK", StringComparison.OrdinalIgnoreCase);
                return Ok(isOk);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing HasPreviousOperationOkStatus");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère la dernière traçabilité d'un produit",
            Description = "Ce endpoint retourne la dernière entrée de traçabilité associée à un produit donné.",
            OperationId = "GetTraceabilityProductLatest"
        )]
        [HttpGet("latest-by-product-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductTraceability>> GetTraceabilityProductLatest([FromQuery] string uidProduit)
        {
            if (string.IsNullOrWhiteSpace(uidProduit))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'uidProduit' est requis." });
            }

            try
            {
                Log.Information("GetTraceabilityProductLatest called with uidProduit: {UidProduit}", uidProduit);
                var tracabiliteProduit = await _tracabiliteProduitRepository.GetLatestProductTraceabilityWithArticleUid(uidProduit);
                if (tracabiliteProduit == null)
                {
                    Log.Information("Product traceability not found for uidProduit: {UidProduit}", uidProduit);
                    return NotFound("Product traceability not found.");
                }
                return Ok(tracabiliteProduit);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetTraceabilityProductLatest");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère la traçabilité des produits par OF et article",
            Description = "Ce endpoint retourne la liste des traçabilités pour un OF et un article donnés.",
            OperationId = "GetCollectionProductTraceability"
        )]
        [HttpGet("by-manufacturing-order-and-article")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<ProductTraceability>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<ProductTraceability>>> GetCollectionProductTraceability([FromQuery] string manufacturingOrderUid, [FromQuery] string articleUid)
        {
            if (string.IsNullOrWhiteSpace(manufacturingOrderUid) || string.IsNullOrWhiteSpace(articleUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Les paramètres 'manufacturingOrderUid' et 'articleUid' sont requis." });
            }

            try
            {
                Log.Information("GetCollectionProductTraceability called with manufacturingOrderUid: {ManufacturingOrderUid}, articleUid: {ArticleUid}", manufacturingOrderUid, articleUid);
                var traceabilityList = await _tracabiliteProduitRepository.GetCollectionProductTraceability(manufacturingOrderUid, articleUid);
                if (traceabilityList == null || traceabilityList.Count == 0)
                {
                    Log.Information("GetCollectionProductTraceability returned no content for manufacturingOrderUid: {ManufacturingOrderUid}, articleUid: {ArticleUid}", manufacturingOrderUid, articleUid);
                    return NoContent();
                }
                return Ok(traceabilityList);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionProductTraceability");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les traçabilités pour un OF et une opération donnés",
            Description = "Ce endpoint retourne la liste des traçabilités associées à un OF et une opération spécifiques.",
            OperationId = "GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUid"
        )]
        [HttpGet("by-manufacturing-order-and-operation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<ProductTraceability>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<ProductTraceability>>> GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUid([FromQuery] string manufacturingOrderUid, [FromQuery] string operationUid)
        {
            if (string.IsNullOrWhiteSpace(manufacturingOrderUid) || string.IsNullOrWhiteSpace(operationUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Les paramètres 'manufacturingOrderUid' et 'operationUid' sont requis." });
            }

            try
            {
                Log.Information("GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUid called with manufacturingOrderUid: {ManufacturingOrderUid}, operationUid: {OperationUid}", manufacturingOrderUid, operationUid);
                var traceabilityList = await _tracabiliteProduitRepository.GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUid(manufacturingOrderUid, operationUid);
                if (traceabilityList == null || traceabilityList.Count == 0)
                {
                    Log.Information("GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUid returned no content for manufacturingOrderUid: {ManufacturingOrderUid}, operationUid: {OperationUid}", manufacturingOrderUid, operationUid);
                    return NoContent();
                }
                return Ok(traceabilityList);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUid");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère la dernière traçabilité pour un OF et une opération donnés",
            Description = "Ce endpoint retourne la dernière traçabilité associée à un OF et une opération spécifiques.",
            OperationId = "GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUidLatestOne"
        )]
        [HttpGet("latest-by-manufacturing-order-and-operation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductTraceability>> GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUidLatestOne([FromQuery] string manufacturingOrderUid, [FromQuery] string operationUid)
        {
            if (string.IsNullOrWhiteSpace(manufacturingOrderUid) || string.IsNullOrWhiteSpace(operationUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Les paramètres 'manufacturingOrderUid' et 'operationUid' sont requis." });
            }

            try
            {
                Log.Information("GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUidLatestOne called with manufacturingOrderUid: {ManufacturingOrderUid}, operationUid: {OperationUid}", manufacturingOrderUid, operationUid);
                var traceability = await _tracabiliteProduitRepository.GetCollectionProductTraceabilityByManufacturingOrderUidByOperationLatestOne(manufacturingOrderUid, operationUid);
                if (traceability == null)
                {
                    Log.Information("No product traceability records found for manufacturingOrderUid: {ManufacturingOrderUid}, operationUid: {OperationUid}", manufacturingOrderUid, operationUid);
                    return NotFound("No product traceability records found.");
                }
                return Ok(traceability);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUidLatestOne");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère la dernière traçabilité par produit, numéro de série et opération",
            Description = "Ce endpoint retourne la dernière traçabilité pour un produit, un numéro de série et une opération spécifiques.",
            OperationId = "GetProductTraceabilityByProductUidBySerialNumberByOperationUidLatestOne"
        )]
        [HttpGet("latest-by-product-serial-and-operation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductTraceability>> GetProductTraceabilityByProductUidBySerialNumberByOperationUidLatestOne([FromQuery] string product, [FromQuery] string serialNumber, [FromQuery] string operationUid)
        {
            if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(serialNumber) || string.IsNullOrWhiteSpace(operationUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Les paramètres 'product', 'serialNumber' et 'operationUid' sont requis." });
            }

            try
            {
                Log.Information("GetProductTraceabilityByProductUidBySerialNumberByOperationUidLatestOne called with product: {Product}, serialNumber: {SerialNumber}, operationUid: {OperationUid}", product, serialNumber, operationUid);
                var traceability = await _tracabiliteProduitRepository.GetProductTraceabilityByProductBySerialNumberByOperationLatestOne(product, serialNumber, operationUid);
                if (traceability == null)
                {
                    Log.Information("No product traceability records found for given parameters");
                    return NotFound("No product traceability records found for the given manufacturing order and serial number.");
                }
                return Ok(traceability);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetProductTraceabilityByProductUidBySerialNumberByOperationUidLatestOne");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les statistiques de traçabilité",
            Description = "Retourne un objet contenant les statistiques pour un OF et une opération donnés.",
            OperationId = "GetTraceabilityStats"
        )]
        [HttpGet("stats-by-manufacturing-order-and-operation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTraceabilityStats([FromQuery] string manufacturingOrderUid, [FromQuery] string operationUid)
        {
            if (string.IsNullOrWhiteSpace(manufacturingOrderUid) || string.IsNullOrWhiteSpace(operationUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Les paramètres 'manufacturingOrderUid' et 'operationUid' sont requis." });
            }

            try
            {
                Log.Information("GetTraceabilityStats called with manufacturingOrderUid: {ManufacturingOrderUid}, operationUid: {OperationUid}", manufacturingOrderUid, operationUid);
                var stats = await _tracabiliteProduitRepository.GetTraceabilityStatsByManufacturingOrderUidAndOperationUid(manufacturingOrderUid, operationUid);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetTraceabilityStats");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les statistiques de traçabilité pour la journée en cours",
            Description = "Retourne les statistiques de traçabilité pour les enregistrements créés aujourd'hui.",
            OperationId = "GetTraceabilityStatsToday"
        )]
        [HttpGet("stats-today-by-manufacturing-order-and-operation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTraceabilityStatsToday([FromQuery] string manufacturingOrderUid, [FromQuery] string operationUid)
        {
            if (string.IsNullOrWhiteSpace(manufacturingOrderUid) || string.IsNullOrWhiteSpace(operationUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Les paramètres 'manufacturingOrderUid' et 'operationUid' sont requis." });
            }

            try
            {
                Log.Information("GetTraceabilityStatsToday called with manufacturingOrderUid: {ManufacturingOrderUid}, operationUid: {OperationUid}", manufacturingOrderUid, operationUid);
                var stats = await _tracabiliteProduitRepository.GetTraceabilityStatsByManufacturingOrderUidAndOperationUidToday(manufacturingOrderUid, operationUid);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetTraceabilityStatsToday");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère une traçabilité par OF, produit et numéro de série",
            Description = "Ce endpoint retourne une entrée unique de traçabilité associée à un ordre de fabrication (OF), un produit et un numéro de série donnés.",
            OperationId = "GetProductTraceabilityByManufacturingOrderProductAndSerialNumber"
        )]
        [HttpGet("by-manufacturing-order-product-and-serial")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductTraceability>> GetProductTraceabilityByManufacturingOrderProductAndSerialNumber([FromQuery] string manufacturingOrderUid, [FromQuery] string productUid, [FromQuery] string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(manufacturingOrderUid) || string.IsNullOrWhiteSpace(productUid) || string.IsNullOrWhiteSpace(serialNumber))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Les paramètres 'manufacturingOrderUid', 'productUid' et 'serialNumber' sont requis." });
            }

            try
            {
                Log.Information("GetProductTraceabilityByManufacturingOrderProductAndSerialNumber called with manufacturingOrderUid: {ManufacturingOrderUid}, productUid: {ProductUid}, serialNumber: {SerialNumber}", manufacturingOrderUid, productUid, serialNumber);
                var traceability = await _tracabiliteProduitRepository.GetProductTraceabilityByManufacturingOrderProductAndSerialNumber(manufacturingOrderUid, productUid, serialNumber);
                if (traceability == null)
                {
                    Log.Information("No product traceability record found for given parameters");
                    return NotFound("No product traceability record found for the given parameters.");
                }
                return Ok(traceability);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetProductTraceabilityByManufacturingOrderProductAndSerialNumber");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère le plus grand numéro de série par OF",
            Description = "Retourne l’enregistrement ayant le plus grand numéro de série pour un ordre de fabrication donné.",
            OperationId = "GetHighestSerialNumberByManufacturingOrderUid"
        )]
        [HttpGet("highest-serial-by-manufacturing-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductTraceability))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductTraceability>> GetHighestSerialNumberByManufacturingOrderUid([FromQuery] string manufacturingOrderUid)
        {
            if (string.IsNullOrWhiteSpace(manufacturingOrderUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'manufacturingOrderUid' est requis." });
            }

            Log.Information("GetHighestSerialNumberByManufacturingOrderUid called with UID: {ManufacturingOrderUid}", manufacturingOrderUid);

            try
            {
                var record = await _tracabiliteProduitRepository.GetProductTraceabilityWithHighestSerialNumberByManufacturingOrderUid(manufacturingOrderUid);
                if (record == null)
                {
                    Log.Information("Aucun enregistrement trouvé pour ManufacturingOrderUid = {ManufacturingOrderUid}", manufacturingOrderUid);
                    return NotFound($"Aucun enregistrement trouvé pour l’ordre de fabrication {manufacturingOrderUid}.");
                }
                return Ok(record);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur dans GetHighestSerialNumberByManufacturingOrderUid endpoint.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
