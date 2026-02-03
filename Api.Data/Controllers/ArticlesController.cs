using Api.Data.Repositories.ArticleRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;

        public ArticlesController(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        [HttpGet("by-manufacturing-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Article>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Article>>> GetCollectionArticleByManufacturingOrderUid([FromQuery] int manufacturingOrderUid)
        {
            if (manufacturingOrderUid <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'manufacturingOrderUid' est requis."
                });
            }

            try
            {
                Log.Information(
                    "GetCollectionArticleByManufacturingOrderUid called with manufacturingOrderUid: {ManufacturingOrderUid}",
                    manufacturingOrderUid);

                var articles = await _articleRepository
                    .GetArticleCollectionByManufacturingOrderUid(manufacturingOrderUid);

                if (articles == null || !articles.Any())
                {
                    Log.Information(
                        "No articles found for manufacturingOrderUid: {ManufacturingOrderUid}",
                        manufacturingOrderUid);

                    return NoContent();
                }

                Log.Information(
                    "GetCollectionArticleByManufacturingOrderUid returned {Count} articles",
                    articles.Count);

                return Ok(articles);
            }
            catch (Exception ex)
            {
                Log.Error(
                    ex,
                    "Exception occurred while processing GetCollectionArticleByManufacturingOrderUid with manufacturingOrderUid: {ManufacturingOrderUid}",
                    manufacturingOrderUid);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère un article par identifiant d'article",
            Description = "Ce endpoint retourne les détails d'un article en fonction de son identifiant unique.",
            OperationId = "GetArticleByArticleUid"
        )]
        [HttpGet("by-article-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Article))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Article>> GetArticleByArticleUid([FromQuery] string articleUid)
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
                Log.Information("GetArticleByArticleUid called with articleUid: {ArticleUid}", articleUid);

                var article = await _articleRepository.GetArticleByArticleUid(articleUid);
                if (article is null)
                {
                    Log.Information("Article not found for articleUid: {ArticleUid}", articleUid);
                    return NotFound();
                }

                Log.Information("GetArticleByArticleUid successfully returned an article for articleUid: {ArticleUid}", articleUid);
                return Ok(article);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetArticleByArticleUid with articleUid: {ArticleUid}", articleUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère un magasin par identifiant d'article",
            Description = "Ce endpoint retourne les informations de magasin liées à un article spécifique.",
            OperationId = "GetArticleStoreByArticleUid"
        )]
        [HttpGet("store-by-article-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> GetArticleStoreByArticleUid([FromQuery] string articleUid)
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
                Log.Information("GetArticleStoreByArticleUid called with articleUid: {ArticleUid}", articleUid);

                var store = await _articleRepository.GetArticleStoreByArticleUid(articleUid);
                if (string.IsNullOrWhiteSpace(store))
                {
                    Log.Information("Store not found for articleUid: {ArticleUid}", articleUid);
                    return NotFound();
                }

                Log.Information("GetArticleStoreByArticleUid successfully returned a store for articleUid: {ArticleUid}", articleUid);
                return Ok(store);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetArticleStoreByArticleUid with articleUid: {ArticleUid}", articleUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère un article par référence fournisseur",
            Description = "Ce endpoint retourne les informations d'un article en fonction de la référence fournisseur.",
            OperationId = "GetArticleByArticleSupplierReference"
        )]
        [HttpGet("by-supplier-reference")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Article))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Article>> GetArticleByArticleSupplierReference([FromQuery] string supplierReference)
        {
            if (string.IsNullOrWhiteSpace(supplierReference))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'supplierReference' est requis."
                });
            }

            try
            {
                Log.Information("GetArticleByArticleSupplierReference called with supplierReference: {SupplierReference}", supplierReference);

                var article = await _articleRepository.GetArticleByArticleSupplierReference(supplierReference);
                if (article is null)
                {
                    Log.Information("Article not found for supplierReference: {SupplierReference}", supplierReference);
                    return NotFound();
                }

                Log.Information("GetArticleByArticleSupplierReference successfully returned an article for supplierReference: {SupplierReference}", supplierReference);
                return Ok(article);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetArticleByArticleSupplierReference with supplierReference: {SupplierReference}", supplierReference);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère tous les articles",
            Description = "Ce endpoint retourne la liste complète de tous les articles disponibles.",
            OperationId = "GetAllArticles"
        )]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Article>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Article>>> GetAllArticles()
        {
            try
            {
                Log.Information("GetAllArticles called");

                var articles = await _articleRepository.GetAllArticles();

                if (articles == null || articles.Count == 0)
                {
                    Log.Information("GetAllArticles returned no content");
                    return NoContent();
                }

                Log.Information("GetAllArticles successfully returned {Count} articles", articles.Count);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetAllArticles");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère un article par référence fournisseur et référence produit",
            Description = "Ce endpoint retourne les informations d'un article spécifique basé sur la référence fournisseur et produit.",
            OperationId = "GetArticleByArticleSupplierReferenceKitBornes"
        )]
        [HttpGet("kit-bornes-by-supplier-and-product")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Article))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Article>> GetArticleByArticleSupplierReferenceKitBornes([FromQuery] string supplierReference, [FromQuery] string productReference)
        {
            if (string.IsNullOrWhiteSpace(supplierReference))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'supplierReference' est requis."
                });
            }

            if (string.IsNullOrWhiteSpace(productReference))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'productReference' est requis."
                });
            }

            try
            {
                Log.Information("GetArticleByArticleSupplierReferenceKitBornes called with supplierReference: {SupplierReference} and productReference: {ProductReference}", supplierReference, productReference);

                var article = await _articleRepository.GetArticleByArticleSupplierReferenceKitBornes(supplierReference, productReference);
                if (article is null)
                {
                    Log.Information("Article not found for supplierReference: {SupplierReference} and productReference: {ProductReference}", supplierReference, productReference);
                    return NotFound();
                }

                Log.Information("GetArticleByArticleSupplierReferenceKitBornes successfully returned an article for supplierReference: {SupplierReference} and productReference: {ProductReference}", supplierReference, productReference);
                return Ok(article);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetArticleByArticleSupplierReferenceKitBornes with supplierReference: {SupplierReference} and productReference: {ProductReference}", supplierReference, productReference);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
