using Api.Data.Repositories.ArticleVersionRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ArticleVersionsController : ControllerBase
    {
        private readonly IArticleVersionRepository _versionArticleRepository;

        public ArticleVersionsController(IArticleVersionRepository versionArticleRepository)
        {
            _versionArticleRepository = versionArticleRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère la version d'un article par identifiant d'article",
            Description = "Ce endpoint retourne la version d'un article en fonction de son identifiant unique.",
            OperationId = "GetArticleVersionByArticleUid"
        )]
        [HttpGet("by-article-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleVersion))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArticleVersionByArticleUid([FromQuery] string articleUid)
        {
            if (string.IsNullOrWhiteSpace(articleUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'articleUid' est requis."
                });
            }

            Log.Information("GetArticleVersionByArticleUid called with articleUid: {ArticleUid}", articleUid);

            try
            {
                var version = await _versionArticleRepository.GetArticleVersionByArticleUid(articleUid);

                if (version == null)
                {
                    Log.Information("No ArticleVersion found for articleUid: {ArticleUid}", articleUid);
                    return NotFound();
                }

                Log.Information("Successfully retrieved ArticleVersion for articleUid: {ArticleUid}", articleUid);
                return Ok(version);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve ArticleVersion for articleUid: {ArticleUid}", articleUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
