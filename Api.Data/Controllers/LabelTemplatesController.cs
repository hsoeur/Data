using Api.Data.Repositories.LabelTemplateRepo;
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
    public class LabelTemplatesController : ControllerBase
    {
        private readonly ILabelTemplateRepository _repository;

        public LabelTemplatesController(ILabelTemplateRepository repository)
        {
            _repository = repository;
        }

        [SwaggerOperation(
            Summary = "Récupère un template par UID Produit et Usage",
            Description = "Ce endpoint retourne un template correspondant au produit et à l'usage donnés.",
            OperationId = "GetLabelTemplateByArticleUidAndUsageUid"
        )]
        [HttpGet("by-article-uid-and-usage")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LabelTemplate))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LabelTemplate>> GetLabelTemplateByArticleUidAndUsageUid([FromQuery] string uidProduit, [FromQuery] string usage)
        {
            if (string.IsNullOrWhiteSpace(uidProduit))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'uidProduit' est requis." });
            }
            if (string.IsNullOrWhiteSpace(usage))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'usage' est requis." });
            }

            try
            {
                Log.Information("GetLabelTemplateByArticleUidAndUsageUid called with uidProduit: {UidProduit}, usage: {Usage}", uidProduit, usage);
                var labelTemplate = await _repository.GetLabelTemplateByArticleUidAndUsageUid(uidProduit, usage);
                return Ok(labelTemplate);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetLabelTemplateByArticleUidAndUsageUid failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les templates associés à un produit",
            Description = "Ce endpoint retourne une liste de templates associés à l'UID Produit fourni.",
            OperationId = "GetCollectionLabelTemplateByArticleUid"
        )]
        [HttpGet("by-article-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<LabelTemplate>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<LabelTemplate>>> GetCollectionLabelTemplateByArticleUid([FromQuery] string uidProduit)
        {
            if (string.IsNullOrWhiteSpace(uidProduit))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'uidProduit' est requis." });
            }

            try
            {
                Log.Information("GetCollectionLabelTemplateByArticleUid called with uidProduit: {UidProduit}", uidProduit);
                var templates = await _repository.GetCollectionLabelTemplateByArticleUid(uidProduit);
                if (templates == null || !templates.Any())
                {
                    Log.Information("GetCollectionLabelTemplateByArticleUid returned no content for uidProduit: {UidProduit}", uidProduit);
                    return NoContent();
                }
                Log.Information("GetCollectionLabelTemplateByArticleUid successfully returned {Count} templates", templates.Count);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetCollectionLabelTemplateByArticleUid failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère tous les templates",
            Description = "Ce endpoint retourne la liste complète de tous les templates existants.",
            OperationId = "GetCollectionLabelTemplates"
        )]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<LabelTemplate>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<LabelTemplate>>> GetCollectionLabelTemplates()
        {
            try
            {
                Log.Information("GetCollectionLabelTemplates called");
                var templates = await _repository.GetCollectionLabelTemplates();
                if (templates == null || !templates.Any())
                {
                    Log.Information("GetCollectionLabelTemplates returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionLabelTemplates successfully returned {Count} templates", templates.Count);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetCollectionLabelTemplates failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les templates par mot-clé",
            Description = "Ce endpoint retourne une liste de templates correspondant au mot-clé fourni.",
            OperationId = "GetCollectionLabelTemplateByKeywordSearch"
        )]
        [HttpGet("search-by-keyword")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<LabelTemplate>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<LabelTemplate>>> GetCollectionLabelTemplateByKeywordSearch([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'keyword' est requis." });
            }

            try
            {
                Log.Information("GetCollectionLabelTemplateByKeywordSearch called");
                var templates = await _repository.GetCollectionLabelTemplateByKeywordSearch(keyword);
                if (templates == null || !templates.Any())
                {
                    Log.Information("GetCollectionLabelTemplateByKeywordSearch returned no content for keyword: {Keyword}", keyword);
                    return NoContent();
                }
                Log.Information("GetCollectionLabelTemplateByKeywordSearch successfully returned {Count} templates", templates.Count);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetCollectionLabelTemplateByKeywordSearch failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les templates actifs par mot-clé",
            Description = "Ce endpoint retourne une liste de templates actifs correspondant au mot-clé fourni.",
            OperationId = "GetCollectionLabelTemplateByKeywordSearchOnActiveOnes"
        )]
        [HttpGet("active-search-by-keyword")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<LabelTemplate>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<LabelTemplate>>> GetCollectionLabelTemplateByKeywordSearchOnActiveOnes([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'keyword' est requis." });
            }

            try
            {
                Log.Information("GetCollectionLabelTemplateByKeywordSearchOnActiveOnes called");
                var templates = await _repository.GetCollectionLabelTemplateByKeywordSearchOnAcitveOnes(keyword);
                if (templates == null || !templates.Any())
                {
                    Log.Information("GetCollectionLabelTemplateByKeywordSearchOnActiveOnes returned no content for keyword: {Keyword}", keyword);
                    return NoContent();
                }
                Log.Information("GetCollectionLabelTemplateByKeywordSearchOnActiveOnes successfully returned {Count} templates", templates.Count);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetCollectionLabelTemplateByKeywordSearchOnActiveOnes failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère tous les templates actifs",
            Description = "Ce endpoint retourne la liste complète de tous les templates actifs.",
            OperationId = "GetCollectionLabelTemplateActiveOnes"
        )]
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<LabelTemplate>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<LabelTemplate>>> GetCollectionLabelTemplateActiveOnes()
        {
            try
            {
                Log.Information("GetCollectionLabelTemplateActiveOnes called");
                var templates = await _repository.GetCollectionLabelTemplateActiveOnes();
                if (templates == null || !templates.Any())
                {
                    Log.Information("GetCollectionLabelTemplateActiveOnes returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionLabelTemplateActiveOnes successfully returned {Count} templates", templates.Count);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetCollectionLabelTemplateActiveOnes failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Crée un nouveau template",
            Description = "Ce endpoint permet de créer un nouveau template en fonction des données fournies.",
            OperationId = "CreateLabelTemplate"
        )]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LabelTemplate))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LabelTemplate>> CreateLabelTemplate([FromBody] LabelTemplateDto request)
        {
            if (request == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }
            if (string.IsNullOrWhiteSpace(request.Product))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'Product' est requis." });
            }
            if (string.IsNullOrWhiteSpace(request.LabelTypeUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'LabelTypeUid' est requis." });
            }

            Log.Information("CreateLabelTemplate called with request: {@Request}", request);

            try
            {
                var now = DateTimeOffset.Now;
                var template = new LabelTemplate
                {
                    LabelTemplateUid = Guid.NewGuid().ToString(),
                    Product = request.Product,
                    LabelTypeUid = request.LabelTypeUid,
                    Photo = request.Photo,
                    TemplateFile = request.TemplateFile,
                    StartDate = now,
                    EndDate = DateTimeOffset.Parse("9999-12-31T23:59:59Z"),
                    UserUid = request.UserUid
                };
                await _repository.CreateLabelTemplate(template);
                Log.Information("CreateLabelTemplate successfully created template uid: {LabelTemplateUid}", template.LabelTemplateUid);
                return Ok(template);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "CreateLabelTemplate failed with InvalidOperationException");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CreateLabelTemplate failed with exception");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Met à jour un template",
            Description = "Ce endpoint permet de mettre à jour un template existant.",
            OperationId = "UpdateLabelTemplate"
        )]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateLabelTemplate([FromBody] LabelTemplate usageCorrespondanceProduit)
        {
            if (usageCorrespondanceProduit == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }
            if (string.IsNullOrWhiteSpace(usageCorrespondanceProduit.LabelTemplateUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'LabelTemplateUid' est requis." });
            }

            Log.Information("UpdateLabelTemplate called with template uid: {LabelTemplateUid}", usageCorrespondanceProduit.LabelTemplateUid);

            try
            {
                await _repository.UpdateLabelTemplate(usageCorrespondanceProduit);
                Log.Information("UpdateLabelTemplate successfully updated template uid: {LabelTemplateUid}", usageCorrespondanceProduit.LabelTemplateUid);
                return Ok("SUCESS UPDATED");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateLabelTemplate failed with exception");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
