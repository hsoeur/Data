using Api.Data.Repositories.BillOfMaterialRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BillOfMaterialsController : ControllerBase
    {
        private readonly IBillOfMaterialRepository _nomenclatureRepository;

        public BillOfMaterialsController(IBillOfMaterialRepository nomenclatureRepository)
        {
            _nomenclatureRepository = nomenclatureRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère la collection de nomenclatures par identifiant d'ordre de fabrication",
            Description = "Ce endpoint retourne une collection de nomenclatures associées à un identifiant d'ordre de fabrication.",
            OperationId = "GetCollectionBillOfMaterialsByManufacturingOrderUid"
        )]
        [HttpGet("by-manufacturing-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<BillOfMaterial>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<BillOfMaterial>>> GetCollectionBillOfMaterialsByManufacturingOrderUid([FromQuery] int manufacturingOrderUid)
        {
            if (manufacturingOrderUid <=0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'manufacturingOrderUid' est requis."
                });
            }

            Log.Information("GetCollectionBillOfMaterialsByManufacturingOrderUid called with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);

            try
            {
                var nomenclatures = await _nomenclatureRepository.GetBillOfMaterialsByManufacturingOrderUid(manufacturingOrderUid);

                if (nomenclatures == null || nomenclatures.Count ==0)
                {
                    Log.Information("No BillOfMaterials found for manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return NoContent();
                }

                Log.Information("GetCollectionBillOfMaterialsByManufacturingOrderUid successfully returned {Count} nomenclatures", nomenclatures.Count);
                return Ok(nomenclatures);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionBillOfMaterialsByManufacturingOrderUid with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les nomenclatures sans étiquettes par identifiant d'ordre de fabrication",
            Description = "Ce endpoint retourne une collection de nomenclatures sans étiquettes associées à un identifiant d'ordre de fabrication.",
            OperationId = "GetCollectionBillOfMaterialsWithoutLabelsByManufacturingOrderUid"
        )]
        [HttpGet("without-labels-by-manufacturing-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<BillOfMaterial>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<BillOfMaterial>>> GetCollectionBillOfMaterialsWithoutLabelsByManufacturingOrderUid([FromQuery] int manufacturingOrderUid)
        {
            if (manufacturingOrderUid <=0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'manufacturingOrderUid' est requis."
                });
            }

            Log.Information("GetCollectionBillOfMaterialsWithoutLabelsByManufacturingOrderUid called with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);

            try
            {
                var nomenclatures = await _nomenclatureRepository.GetBillOfMaterialsWithoutLabelsByManufacturingOrderUid(manufacturingOrderUid);

                if (nomenclatures == null || nomenclatures.Count ==0)
                {
                    Log.Information("No BillOfMaterials without labels found for manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return NoContent();
                }

                Log.Information("GetCollectionBillOfMaterialsWithoutLabelsByManufacturingOrderUid successfully returned {Count} nomenclatures", nomenclatures.Count);
                return Ok(nomenclatures);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionBillOfMaterialsWithoutLabelsByManufacturingOrderUid with manufacturingOrderUid: {ManufacturingOrderUid}", manufacturingOrderUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les étiquettes des nomenclatures par identifiant d'article",
            Description = "Ce endpoint retourne les étiquettes des nomenclatures associées à un identifiant d'article.",
            OperationId = "GetCollectionBillOfMaterialsLabelsByArticleUid"
        )]
        [HttpGet("labels-by-article-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<BillOfMaterial>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<BillOfMaterial>>> GetCollectionBillOfMaterialsLabelsByArticleUid([FromQuery] string articleUid)
        {
            if (string.IsNullOrWhiteSpace(articleUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'articleUid' est requis."
                });
            }

            Log.Information("GetCollectionBillOfMaterialsLabelsByArticleUid called with articleUid: {ArticleUid}", articleUid);

            try
            {
                var nomenclatures = await _nomenclatureRepository.GetBillOfMaterialsLabelsByArticleUid(articleUid);

                if (nomenclatures == null || nomenclatures.Count ==0)
                {
                    Log.Information("No BillOfMaterials labels found for articleUid: {ArticleUid}", articleUid);
                    return NoContent();
                }

                Log.Information("GetCollectionBillOfMaterialsLabelsByArticleUid successfully returned {Count} nomenclatures", nomenclatures.Count);
                return Ok(nomenclatures);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionBillOfMaterialsLabelsByArticleUid with articleUid: {ArticleUid}", articleUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère une nomenclature par identifiants d'article et de composant",
            Description = "Ce endpoint retourne une nomenclature spécifique basée sur les identifiants d'article et de composant.",
            OperationId = "GetBillOfMaterialsLabelsByArticleUidAndComponentUid"
        )]
        [HttpGet("labels-by-article-and-component")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BillOfMaterial))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BillOfMaterial>> GetBillOfMaterialsLabelsByArticleUidAndComponentUid([FromQuery] string articleUid, [FromQuery] string componentUid)
        {
            if (string.IsNullOrWhiteSpace(articleUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'articleUid' est requis."
                });
            }

            if (string.IsNullOrWhiteSpace(componentUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'componentUid' est requis."
                });
            }

            Log.Information("GetBillOfMaterialsLabelsByArticleUidAndComponentUid called with articleUid: {ArticleUid} and componentUid: {ComponentUid}", articleUid, componentUid);

            try
            {
                var nomenclature = await _nomenclatureRepository.GetBillOfMaterialByArticleUidAndbyComponentUid(articleUid, componentUid);

                if (nomenclature == null)
                {
                    Log.Information("No BillOfMaterials found for articleUid: {ArticleUid} and componentUid: {ComponentUid}", articleUid, componentUid);
                    return NotFound();
                }

                Log.Information("GetBillOfMaterialsLabelsByArticleUidAndComponentUid successfully returned a nomenclature");
                return Ok(nomenclature);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetBillOfMaterialsLabelsByArticleUidAndComponentUid with articleUid: {ArticleUid} and componentUid: {ComponentUid}", articleUid, componentUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Génère un CSV des quantités pour les articles dans la nomenclature",
            Description = "Ce endpoint génère un fichier CSV contenant les quantités et les informations associées pour les articles présents dans la nomenclature.",
            OperationId = "GetQuantitiesForArticleInBillOfMaterials"
        )]
        [HttpGet("quantities-csv")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQuantitiesForArticleInBillOfMaterials()
        {
            Log.Information("GetQuantitiesForArticleInBillOfMaterials called");

            try
            {
                var articles = await _nomenclatureRepository.GetQuantitiesForArticlesInBillOfMaterialsByArticleUids();

                if (articles == null || articles.Count ==0)
                {
                    Log.Information("No articles found for CSV generation in GetQuantitiesForArticleInBillOfMaterials");
                    return NoContent();
                }

                Log.Information("Generating CSV for {Count} articles", articles.Count);

                // Création du CSV en mémoire avec points-virgules
                var csvContent = new StringBuilder();
                csvContent.AppendLine("ArticleUid;Designation;Store;Family;Packaging;ManageStock;SupplierReference;GpaoQuantity");

                foreach (var article in articles)
                {
                    csvContent.AppendLine($"{article.ArticleUid};{article.Designation};{article.Store};{article.Family};{article.Packaging};{article.ManageStock};{article.SupplierReference};{article.GpaoQuantity}");
                }

                var byteArray = Encoding.UTF8.GetBytes(csvContent.ToString());
                var stream = new MemoryStream(byteArray);

                Log.Information("CSV generation completed, returning file");

                return File(stream, "text/csv", "Articles_Bornes_Qte_GPAO.csv");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetQuantitiesForArticleInBillOfMaterials");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
