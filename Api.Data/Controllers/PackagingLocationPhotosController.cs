using Api.Data.Repositories.PackagingLocationPhotoRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PackagingLocationPhotosController : ControllerBase
    {
        private readonly IPackagingLocationPhotoRepository _packagingLocationPhotoRepository;

        public PackagingLocationPhotosController(IPackagingLocationPhotoRepository packagingLocationPhotoRepository)
        {
            _packagingLocationPhotoRepository = packagingLocationPhotoRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère une photo d'emplacement par ArticleUid et UseCase",
            Description = "Retourne la photo associée à un article pour un cas d'usage donné.",
            OperationId = "GetPackagingLocationPhotoByArticleUidAndUseCase"
        )]
        [HttpGet("by-article-uid-and-usecase")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackagingLocationPhoto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PackagingLocationPhoto>> GetPackagingLocationPhotoByArticleUidAndUseCase([FromQuery] string articleUid, [FromQuery] string useCase)
        {
            if (string.IsNullOrWhiteSpace(articleUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'articleUid' est requis."
                });
            }
            if (string.IsNullOrWhiteSpace(useCase))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'useCase' est requis."
                });
            }

            try
            {
                Log.Information("GetPackagingLocationPhotoByArticleUidAndUseCase called with ArticleUid: {ArticleUid}, UseCase: {UseCase}", articleUid, useCase);
                var photo = await _packagingLocationPhotoRepository.GetPackagingLocationPhotoByArticleUidAndUseCase(articleUid, useCase);
                if (photo == null)
                {
                    Log.Information("PackagingLocationPhoto not found for ArticleUid: {ArticleUid} and UseCase: {UseCase}", articleUid, useCase);
                    return NotFound();
                }
                Log.Information("GetPackagingLocationPhotoByArticleUidAndUseCase successfully returned PackagingLocationPhotoUid: {PackagingLocationPhotoUid}", photo.PackagingLocationPhotoUid);
                return Ok(photo);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetPackagingLocationPhotoByArticleUidAndUseCase for ArticleUid: {ArticleUid} and UseCase: {UseCase}", articleUid, useCase);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
