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
    public class FirmwareVersionsController : ControllerBase
    {
        private readonly IFirmwareVersionRepository _firmwareVersionRepository;

        public FirmwareVersionsController(IFirmwareVersionRepository firmwareVersionRepository)
        {
            _firmwareVersionRepository = firmwareVersionRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère la version firmware par identifiant d'article",
            Description = "Ce endpoint retourne la version firmware en fonction de l'identifiant d'article.",
            OperationId = "GetFirmwareVersionByArticleUid"
        )]
        [HttpGet("by-article-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FirmwareVersion))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFirmwareVersionByArticleUid([FromQuery] string articleUid)
        {
            if (string.IsNullOrWhiteSpace(articleUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'articleUid' est requis."
                });
            }

            Log.Information("GetFirmwareVersionByArticleUid called with articleUid: {ArticleUid}", articleUid);

            try
            {
                var version = await _firmwareVersionRepository.GetFirmwareVersionByArticleUid(articleUid);

                if (version == null)
                {
                    Log.Information("No FirmwareVersion found for articleUid: {ArticleUid}", articleUid);
                    return NotFound();
                }

                Log.Information("Successfully retrieved FirmwareVersion for articleUid: {ArticleUid}", articleUid);
                return Ok(version);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve FirmwareVersion for articleUid: {ArticleUid}", articleUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
