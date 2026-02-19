using Api.Data.Dtos;
using Api.Data.Repositories.LabelRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LabelsController : ControllerBase
    {
        private readonly ILabelRepository _etiquetteRepository;

        public LabelsController(ILabelRepository etiquetteRepository)
        {
            _etiquetteRepository = etiquetteRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère le dernier numéro de série imprimé par ordre de fabrication",
            Description = "Ce endpoint retourne le dernier numéro de série imprimé associé à un identifiant d'ordre de fabrication.",
            OperationId = "GetLabelLastSerialNumberPrintedByManufacturingOrderUid"
        )]
        [HttpGet("last-serial-number-by-manufacturing-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Label))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Label>> GetLabelLastSerialNumberPrintedByManufacturingOrderUid([FromQuery] int manufacturingOrderUid)
        {

            try
            {
                Log.Information("GetLabelLastSerialNumberPrintedByManufacturingOrderUid called with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                var label = await _etiquetteRepository.GetLabelLastSerialNumberPrintedByManufacturingOrderUid(manufacturingOrderUid);
                if (label == null)
                {
                    Log.Information("No label found for manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return NotFound();
                }
                return Ok(label);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetLabelLastSerialNumberPrintedByManufacturingOrderUid with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère la collection des étiquettes par ordre de fabrication",
            Description = "Ce endpoint retourne la collection complète des étiquettes associées à un identifiant d'ordre de fabrication.",
            OperationId = "GetCollectionLabelByManufacturingOrderUid"
        )]
        [HttpGet("by-manufacturing-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Label>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Label>>> GetCollectionLabelByManufacturingOrderUid([FromQuery] int manufacturingOrderUid)
        {


            try
            {
                Log.Information("GetCollectionLabelByManufacturingOrderUid called with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                var etiquettes = await _etiquetteRepository.GetCollectionLabelsByManufacturingOrderUid(manufacturingOrderUid);
                if (etiquettes == null || etiquettes.Count == 0)
                {
                    Log.Information("GetCollectionLabelByManufacturingOrderUid returned no content for manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return NoContent();
                }
                Log.Information("GetCollectionLabelByManufacturingOrderUid successfully returned {Count} etiquettes", etiquettes.Count);
                return Ok(etiquettes);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionLabelByManufacturingOrderUid with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les étiquettes par AuthentiUid",
            Description = "Ce endpoint retourne les étiquettes associées à un AuthentiUid spécifique.",
            OperationId = "GetLabelByAuthentiUid"
        )]
        [HttpGet("by-authenti-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Label))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Label>> GetLabelByAuthentiUid([FromQuery] string authentiUid)
        {
            if (string.IsNullOrWhiteSpace(authentiUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'authentiUid' est requis."
                });
            }

            try
            {
                Log.Information("GetLabelByAuthentiUid called with authentiUid: {AuthentiUid}", authentiUid);
                var etiquette = await _etiquetteRepository.GetLabelByAuthentiUid(authentiUid);
                if (etiquette == null)
                {
                    Log.Information("GetLabelByAuthentiUid did not find a label for authentiUid: {AuthentiUid}", authentiUid);
                    return NotFound();
                }
                Log.Information("GetLabelByAuthentiUid successfully returned a label with LabelUid: {LabelUid}", etiquette.LabelUid);
                return Ok(etiquette);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetLabelByAuthentiUid with authentiUid: {AuthentiUid}", authentiUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Crée une nouvelle étiquette",
            Description = "Ce endpoint permet de créer une étiquette en fournissant les informations nécessaires dans la requête.",
            OperationId = "CreateLabel"
        )]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Label))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Label>> CreateLabel([FromBody] LabelDto request)
        {
            if (request == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le corps de la requête est requis."
                });
            }
           

            Log.Information("CreateLabel called with request: {@Request}", request);
            var etiquette = new Label
            {
                LabelUid = Guid.NewGuid().ToString(),
                AuthentiUid = request.AuthentiUid,
                UserUid = request.UserUid,
                ManufacturingOrderUid = request.ManufacturingOrderUid,
                CreationDate = request.CreationDate,
                CurrentSerialNumber = request.CurrentSerialNumber,
                ProductVersion = request.ProductVersion,
                LabelTypeUid = request.LabelTypeUid,
                ProductUid = request.ProductUid,
                Template = request.Template,
            };

            try
            {
                await _etiquetteRepository.CreateLabel(etiquette);
                Log.Information("CreateLabel successfully created a label with LabelUid: {LabelUid}", etiquette.LabelUid);
                return Ok(etiquette);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateLabel for ManufacturingOrderUid: {ManufacturingOrderUid}", request.ManufacturingOrderUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Vérifie si une étiquette existe",
            Description = "Ce endpoint vérifie si une étiquette existe pour un AuthentiUid donné.",
            OperationId = "LabelExists"
        )]
        [HttpGet("exists-by-authenti-uid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LabelExists([FromQuery] string authentiUid)
        {
            if (string.IsNullOrWhiteSpace(authentiUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'authentiUid' est requis."
                });
            }

            try
            {
                Log.Information("LabelExists called with authentiUid: {AuthentiUid}", authentiUid);
                var exists = await _etiquetteRepository.LabelExists(authentiUid);
                if (exists)
                {
                    Log.Information("LabelExists found a label with authentiUid: {AuthentiUid}", authentiUid);
                    return Ok();
                }
                Log.Information("LabelExists did not find any label with authentiUid: {AuthentiUid}", authentiUid);
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing LabelExists with authentiUid: {AuthentiUid}", authentiUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les étiquettes les plus récentes par ordre de fabrication",
            Description = "Ce endpoint retourne la collection des étiquettes les plus récentes associées à un identifiant d'ordre de fabrication.",
            OperationId = "GetCollectionLabelMostRecentByManufacturingOrderUid"
        )]
        [HttpGet("most-recent-by-manufacturing-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Label>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Label>>> GetCollectionLabelMostRecentByManufacturingOrderUid([FromQuery] int manufacturingOrderUid)
        {
              

            try
            {
                Log.Information("GetCollectionLabelMostRecentByManufacturingOrderUid called with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                var etiquettes = await _etiquetteRepository.GetCollectionLabelsMostRecentByManufacturingOrderUid(manufacturingOrderUid);
                if (etiquettes == null || etiquettes.Count == 0)
                {
                    Log.Information("GetCollectionLabelMostRecentByManufacturingOrderUid returned no content for manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return NoContent();
                }
                Log.Information("GetCollectionLabelMostRecentByManufacturingOrderUid successfully returned {Count} etiquettes", etiquettes.Count);
                return Ok(etiquettes);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionLabelMostRecentByManufacturingOrderUid with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Vérifie si une étiquette a été imprimée",
            Description = "Ce endpoint vérifie si une étiquette a été imprimée pour un ordre de fabrication et un usage donnés.",
            OperationId = "LabelPrinted"
        )]
        [HttpGet("printed-by-manufacturing-order-and-usage")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> LabelPrinted([FromQuery] int manufacturingOrderUid, [FromQuery] string usageUid)
        {
                
            if (string.IsNullOrWhiteSpace(usageUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'usageUid' est requis."
                });
            }

            try
            {
                Log.Information("LabelPrinted called with manufacturingOrderUid: {ManufacturingOrderUid}, usageUid: {UsageUid}", manufacturingOrderUid, usageUid);
                var exists = await _etiquetteRepository.LabelPrinted(manufacturingOrderUid, usageUid);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing LabelPrinted with manufacturingOrderUid: {ManufacturingOrderUid}, usageUid: {UsageUid}", manufacturingOrderUid, usageUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Recherche des étiquettes par mot-clé",
            Description = "Ce endpoint retourne une collection d'étiquettes correspondant au mot-clé, à l'ordre de fabrication et à l'usage fournis.",
            OperationId = "GetCollectionByKeywordSearch"
        )]
        [HttpGet("search-by-keyword")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Label>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Label>>> GetCollectionByKeywordSearch([FromQuery] string keyword, [FromQuery] int manufacturingOrderUid, [FromQuery] string usageUid)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'keyword' est requis."
                });
            }
                
            if (string.IsNullOrWhiteSpace(usageUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'usageUid' est requis."
                });
            }

            try
            {
                Log.Information("GetCollectionByKeywordSearch called with keyword: {Keyword}, manufacturingOrderUid: {ManufacturingOrderUid}, usageUid: {UsageUid}", keyword, manufacturingOrderUid, usageUid);
                var etiquettes = await _etiquetteRepository.GetCollectionLabelsByKeywordSearch(keyword, manufacturingOrderUid, usageUid);
                if (etiquettes == null || etiquettes.Count == 0)
                {
                    Log.Information("GetCollectionByKeywordSearch returned no content for keyword: {Keyword}", keyword);
                    return NoContent();
                }
                Log.Information("GetCollectionByKeywordSearch successfully returned {Count} etiquettes", etiquettes.Count);
                return Ok(etiquettes);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionByKeywordSearch with keyword: {Keyword}, manufacturingOrderUid: {ManufacturingOrderUid}, usageUid: {UsageUid}", keyword, manufacturingOrderUid, usageUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère des étiquettes par ordre de fabrication et usage",
            Description = "Ce endpoint retourne une collection d'étiquettes associées à un ordre de fabrication et un usage spécifiques.",
            OperationId = "GetCollectionLabelByManufacturingOrderUidAndUsageUid"
        )]
        [HttpGet("by-manufacturing-order-and-usage")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Label>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Label>>> GetCollectionLabelByManufacturingOrderUidAndUsageUid([FromQuery] int manufacturingOrderUid, [FromQuery] string usageUid)
        {
                
            if (string.IsNullOrWhiteSpace(usageUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'usageUid' est requis."
                });
            }

            try
            {
                Log.Information("GetCollectionLabelByManufacturingOrderUidAndUsageUid called with manufacturingOrderUid: {ManufacturingOrderUid}, usageUid: {UsageUid}", manufacturingOrderUid, usageUid);
                var etiquettes = await _etiquetteRepository.GetCollectionLabelsByManufacturingOrderUidAndUsageUid(manufacturingOrderUid, usageUid);
                if (etiquettes == null || etiquettes.Count == 0)
                {
                    Log.Information("GetCollectionLabelByManufacturingOrderUidAndUsageUid returned no content for manufacturingOrderUid: {ManufacturingOrderUid}, usageUid: {UsageUid}", manufacturingOrderUid, usageUid);
                    return NoContent();
                }
                Log.Information("GetCollectionLabelByManufacturingOrderUidAndUsageUid successfully returned {Count} etiquettes", etiquettes.Count);
                return Ok(etiquettes);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionLabelByManufacturingOrderUidAndUsageUid with manufacturingOrderUid: {ManufacturingOrderUid}, usageUid: {UsageUid}", manufacturingOrderUid, usageUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
    
}
