using Api.Data.Repositories.FirmwareRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FirmwaresController : ControllerBase
    {
        private readonly IFirmwareRepository _firmwareRepository;

        public FirmwaresController(IFirmwareRepository firmwareRepository)
        {
            _firmwareRepository = firmwareRepository;
        }

        [HttpGet("active")]
        [SwaggerOperation(
            Summary = "Récupère les firmwares actifs",
            Description = "Ce endpoint retourne la liste de tous les firmwares actifs.",
            OperationId = "GetCollectionFirmwareActiveOnes"
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Firmware>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCollectionFirmwareActiveOnes()
        {
            try
            {
                Log.Information("GetCollectionFirmwareActiveOnes called");

                var result = await _firmwareRepository.GetCollectionFirmwareActiveOnes();

                if (result == null || result.Count == 0)
                {
                    Log.Information("No active firmwares found");
                    return NoContent();
                }

                Log.Information("GetCollectionFirmwareActiveOnes successfully returned {Count} firmwares", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching active firmwares.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("by-keyword")]
        [SwaggerOperation(
            Summary = "Récupère les firmwares par mot-clé",
            Description = "Ce endpoint retourne les firmwares correspondant au mot-clé spécifié.",
            OperationId = "GetCollectionFirmwaresByKeyword"
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Firmware>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCollectionFirmwaresByKeyword([FromQuery] string keyword)
        {
            try
            {
                Log.Information("GetCollectionFirmwaresByKeyword called with keyword: {Keyword}", keyword);

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Paramètre invalide",
                        Detail = "Le paramètre 'keyword' est requis."
                    });
                }

                var result = await _firmwareRepository.GetCollectionFirmwareByKeyword(keyword);

                if (result == null || result.Count == 0)
                {
                    Log.Information("No firmwares found for keyword: {Keyword}", keyword);
                    return NoContent();
                }

                Log.Information("GetCollectionFirmwaresByKeyword successfully returned {Count} firmwares for keyword: {Keyword}", result.Count, keyword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching firmwares by keyword: {Keyword}", keyword);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("by-keyword-active")]
        [SwaggerOperation(
            Summary = "Récupère les firmwares actifs par mot-clé",
            Description = "Ce endpoint retourne les firmwares actifs correspondant au mot-clé spécifié.",
            OperationId = "GetCollectionFirmwaresByKeywordActiveOnes"
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Firmware>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCollectionFirmwaresByKeywordActiveOnes([FromQuery] string keyword)
        {
            try
            {
                Log.Information("GetCollectionFirmwaresByKeywordActiveOnes called with keyword: {Keyword}", keyword);

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Paramètre invalide",
                        Detail = "Le paramètre 'keyword' est requis."
                    });
                }

                var result = await _firmwareRepository.GetCollectionFirmwareByKeywordActiveOnes(keyword);

                if (result == null || result.Count == 0)
                {
                    Log.Information("No active firmwares found for keyword: {Keyword}", keyword);
                    return NoContent();
                }

                Log.Information("GetCollectionFirmwaresByKeywordActiveOnes successfully returned {Count} firmwares for keyword: {Keyword}", result.Count, keyword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching active firmwares by keyword: {Keyword}", keyword);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("all")]
        [SwaggerOperation(
            Summary = "Récupère tous les firmwares",
            Description = "Ce endpoint retourne tous les firmwares disponibles.",
            OperationId = "GetCollectionFirmwares"
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Firmware>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCollectionFirmwares()
        {
            try
            {
                Log.Information("GetCollectionFirmwares called");

                var result = await _firmwareRepository.GetCollectionFirmwares();

                if (result == null || result.Count == 0)
                {
                    Log.Information("No firmwares found");
                    return NoContent();
                }

                Log.Information("GetCollectionFirmwares successfully returned {Count} firmwares", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching all firmwares.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("active-by-article-and-label")]
        [SwaggerOperation(
            Summary = "Récupère un firmware actif par articleUid et label",
            Description = "Ce endpoint retourne un firmware actif correspondant à l'articleUid et au label spécifiés.",
            OperationId = "GetFirmwareByArticleUidByLabelActiveOne"
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Firmware))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFirmwareByArticleUidByLabelActiveOne([FromQuery] string articleUid, [FromQuery] string label)
        {
            Log.Information("GetFirmwareByArticleUidByLabelActiveOne called with articleUid: {ArticleUid}, label: {Label}", articleUid, label);

            if (string.IsNullOrWhiteSpace(articleUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'articleUid' est requis."
                });
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'label' est requis."
                });
            }

            try
            {
                var firmware = await _firmwareRepository.GetFirmwareByArticleUidByLabelActiveOne(articleUid, label);

                if (firmware == null)
                {
                    Log.Information("No active Firmware found for articleUid: {ArticleUid} and label: {Label}", articleUid, label);
                    return NotFound();
                }

                Log.Information("Successfully retrieved active Firmware for articleUid: {ArticleUid}, label: {Label}", articleUid, label);
                return Ok(firmware);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing GetFirmwareByArticleUidByLabelActiveOne for articleUid: {ArticleUid}, label: {Label}", articleUid, label);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("update")]
        [ProducesResponseType(200, Type = typeof(Firmware))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
            Summary = "Met à jour un firmware",
            Description = "Ce endpoint met à jour les informations d'un firmware existant.",
            OperationId = "UpdateFirmware"
        )]
        public async Task<IActionResult> UpdateFirmware(Firmware firmware)
        {
            Log.Information("UpdateTolerance called with request: {@Request}", firmware);

            try
            {
                // Use DateTimeOffset from model directly
                var startDate = firmware.StartDate;
                var today = DateTimeOffset.Now.Date;

                if (startDate.Date > today)
                {
                    Log.Information("Anticipated Mode");
                    var firm = await _firmwareRepository.IsDoubletProductAndVersion(firmware);

                    if (firm != null)
                    {
                        return Ok("Le couple Produit/fichier Firmware ou le couple Produit/Version existe déjà.");
                    }

                    var updatedFirmwareAnticipated = await _firmwareRepository.UpdateFirmwareAnticipatedMode(firmware);
                }
                else if (startDate.Date == today)
                {
                    Log.Information("Nominal Mode");

                    var firm = await _firmwareRepository.IsDoubletProductAndVersion(firmware);

                    if (firm != null)
                    {
                        return Ok("Le couple Produit/fichier Firmware ou le couple Produit/Version existe déjà.");
                    }

                    var updatedFirmwareNominal = await _firmwareRepository.UpdateFirmwareNominalMode(firmware);
                }
                else
                {
                    return Ok("La date de début du nouveau firmware est inférieure à la date d'aujourd'hui.");
                }

                return Ok("SUCESS UPDATED");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateTolerance failed with exception: {Exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("link-firmware-file")]
        [ProducesResponseType(200, Type = typeof(Firmware))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
            Summary = "Liaison d'un fichier firmware",
            Description = "Ce endpoint lie un fichier firmware à un article.",
            OperationId = "LinkFirmwareFile"
        )]
        public async Task<IActionResult> LinkFirmwareFile(Firmware firmware)
        {
            try
            {
                var updateFirmwareFile = await _firmwareRepository.LinkFirmwareFileUpdate(firmware);
                return Ok("SUCCESS LINKED");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "LinkFirmwareFile failed with exception: {Exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("suspend")]
        [ProducesResponseType(200, Type = typeof(Firmware))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
            Summary = "Suspend un firmware",
            Description = "Ce endpoint suspend temporairement un firmware.",
            OperationId = "SuspendFirmware"
        )]
        public async Task<IActionResult> SuspendFirmware(Firmware firmware)
        {
            try
            {
                var updateFirmwareFile = await _firmwareRepository.SuspendFirmwareTemporary(firmware);
                return Ok("SUCCESS SUSPENDED");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "LinkFirmwareFile failed with exception: {Exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("unsuspend")]
        [ProducesResponseType(200, Type = typeof(Firmware))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        [SwaggerOperation(
            Summary = "Annule la suspension d'un firmware",
            Description = "Ce endpoint annule la suspension temporaire d'un firmware.",
            OperationId = "UnsuspendFirmware"
        )]
        public async Task<IActionResult> UnsuspendFirmware(Firmware firmware)
        {
            try
            {
                var updateFirmwareFile = await _firmwareRepository.UnsuspendFirmwareTemporary(firmware);
                return Ok("SUCCESS UNSUSPENDED");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "LinkFirmwareFile failed with exception: {Exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
