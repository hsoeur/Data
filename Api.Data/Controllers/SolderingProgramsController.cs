using Api.Data.Dtos;
using Api.Data.Repositories.SolderingProgramRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SolderingProgramController : ControllerBase
    {
        private readonly ISolderingProgramRepository _solderingProgramsRepository;

        public SolderingProgramController(ISolderingProgramRepository solderingProgramsRepository)
        {
            _solderingProgramsRepository = solderingProgramsRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère toutes les programmes de soudure",
            Description = "Retourne l'ensemble des programmes de soudure.",
            OperationId = "GetCollectionSolderingPrograms"
        )]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<SolderingProgram>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<SolderingProgram>>> GetCollectionSolderingPrograms()
        {
            try
            {
                Log.Information("GetCollectionSolderingPrograms called");
                var programs = await _solderingProgramsRepository.GetCollectionSolderingPrograms();
                if (programs == null || !programs.Any())
                {
                    Log.Information("GetCollectionSolderingPrograms returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionSolderingPrograms successfully returned {Count} soldering programs", programs.Count());
                return Ok(programs.ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while retrieving soldering programs");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Recherche des programmes de soudure par mot-clé",
            Description = "Retourne les programmes de soudure correspondant au mot-clé fourni.",
            OperationId = "GetCollectionSolderingProgramsByKeyword"
        )]
        [HttpGet("search-by-keyword")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<SolderingProgram>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<SolderingProgram>>> GetCollectionSolderingProgramsByKeyword([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'keyword' est requis." });
            }

            try
            {
                Log.Information("GetCollectionSolderingProgramsByKeyword called with keyword: {Keyword}", keyword);
                var programs = await _solderingProgramsRepository.GetCollectionSolderingProgramsByKeywordSearch(keyword);
                if (programs == null || !programs.Any())
                {
                    Log.Information("GetCollectionSolderingProgramsByKeyword returned no content for keyword: {Keyword}", keyword);
                    return NoContent();
                }
                Log.Information("GetCollectionSolderingProgramsByKeyword successfully returned {Count} soldering programs for keyword: {Keyword}", programs.Count(), keyword);
                return Ok(programs.ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while searching soldering programs with keyword: {Keyword}", keyword);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les programmes de soudure actifs",
            Description = "Retourne la collection des programmes de soudure actifs.",
            OperationId = "GetCollectionSolderingProgramsActiveOnes"
        )]
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<SolderingProgram>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<SolderingProgram>>> GetCollectionSolderingProgramsActiveOnes()
        {
            try
            {
                Log.Information("GetCollectionSolderingProgramsActiveOnes called");
                var activePrograms = await _solderingProgramsRepository.GetCollectionSolderingProgramsActiveOnes();
                if (activePrograms == null || !activePrograms.Any())
                {
                    Log.Information("GetCollectionSolderingProgramsActiveOnes returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionSolderingProgramsActiveOnes successfully returned {Count} active soldering programs", activePrograms.Count);
                return Ok(activePrograms);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while retrieving active soldering programs");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Recherche des programmes de soudure actifs par mot-clé",
            Description = "Retourne les programmes de soudure actifs correspondant au mot-clé fourni.",
            OperationId = "GetCollectionSolderingProgramsByKeywordActiveOnes"
        )]
        [HttpGet("active-search-by-keyword")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<SolderingProgram>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<SolderingProgram>>> GetCollectionSolderingProgramsByKeywordActiveOnes([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'keyword' est requis." });
            }

            try
            {
                Log.Information("GetCollectionSolderingProgramsByKeywordActiveOnes called with keyword: {Keyword}", keyword);
                var activePrograms = await _solderingProgramsRepository.GetCollectionSolderingProgramsByKeywordActiveOnes(keyword);
                if (activePrograms == null || !activePrograms.Any())
                {
                    Log.Information("GetCollectionSolderingProgramsByKeywordActiveOnes returned no content for keyword: {Keyword}", keyword);
                    return NoContent();
                }
                Log.Information("GetCollectionSolderingProgramsByKeywordActiveOnes successfully returned {Count} active soldering programs for keyword: {Keyword}", activePrograms.Count, keyword);
                return Ok(activePrograms);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while retrieving active soldering programs by keyword: {Keyword}", keyword);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Met à jour un programme de soudure",
            Description = "Met à jour un programme de soudure existant.",
            OperationId = "UpdateSolderingProgram"
        )]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SolderingProgram))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SolderingProgram>> UpdateSolderingProgram([FromBody] SolderingProgram solderingProgram)
        {
            if (solderingProgram == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }
            if (string.IsNullOrWhiteSpace(solderingProgram.SolderingProgramUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'SolderingProgramUid' est requis." });
            }

            try
            {
                Log.Information("UpdateSolderingProgram called for uid: {SolderingProgramUid}", solderingProgram.SolderingProgramUid);
                var updatedProgram = await _solderingProgramsRepository.UpdateSolderingProgram(solderingProgram);
                if (updatedProgram == null)
                {
                    Log.Information("UpdateSolderingProgram could not find program uid: {SolderingProgramUid}", solderingProgram.SolderingProgramUid);
                    return NotFound($"Soldering program with UID {solderingProgram.SolderingProgramUid} not found.");
                }
                Log.Information("UpdateSolderingProgram successfully updated uid: {SolderingProgramUid}", solderingProgram.SolderingProgramUid);
                return Ok(updatedProgram);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "Error occurred while updating the soldering program");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while updating the soldering program");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Crée un programme de soudure",
            Description = "Crée un nouveau programme de soudure.",
            OperationId = "CreateSolderingProgram"
        )]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SolderingProgram))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SolderingProgram>> CreateSolderingProgram([FromBody] SolderingProgramDto request)
        {
            if (request == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }
            if (string.IsNullOrWhiteSpace(request.Program))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'Program' est requis." });
            }
            if (!int.TryParse(request.Program, out var programValue))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'Program' doit être un entier." });
            }

            try
            {
                Log.Information("CreateSolderingProgram called with Program: {Program}", request.Program);
                var now = DateTimeOffset.Now;
                var solderingProgram = new SolderingProgram
                {
                    SolderingProgramUid = Guid.NewGuid().ToString(),
                    Program = programValue,
                    StartDate = now,
                    EndDate = DateTimeOffset.Parse("9999-12-31T23:59:59Z"),
                    UserUid = request.UserUid,
                    ProductUid = request.ProductUid,
                    Side = request.Side,
                };
                var createdProgram = await _solderingProgramsRepository.CreateSolderingProgram(solderingProgram);
                Log.Information("CreateSolderingProgram successfully created with uid: {SolderingProgramUid}", createdProgram.SolderingProgramUid);
                return CreatedAtAction(nameof(CreateSolderingProgram), new { id = createdProgram.SolderingProgramUid }, createdProgram);
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning(ex, "Invalid input provided for creating a soldering program");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while creating the soldering program");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère le dernier programme de soudure par OF",
            Description = "Retourne le dernier programme de soudure pour un ordre de fabrication.",
            OperationId = "GetSolderingProgramByManufacturingOrderUidLatestOne"
        )]
        [HttpGet("latest-by-manufacturing-order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SolderingProgram))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SolderingProgram>> GetSolderingProgramByManufacturingOrderUidLatestOne([FromQuery] int manufacturingOfUid)
        {
            if (manufacturingOfUid <=0)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'manufacturingOfUid' est requis et doit être >0." });
            }

            try
            {
                Log.Information("GetSolderingProgramByManufacturingOrderUidLatestOne called with manufacturingOfUid={ManufacturingOfUid}", manufacturingOfUid);
                var solderingProgram = await _solderingProgramsRepository.GetSolderingProgramByManufacturingOrderUid(manufacturingOfUid);
                if (solderingProgram == null)
                {
                    Log.Information("No soldering program found for manufacturingOfUid={ManufacturingOfUid}", manufacturingOfUid);
                    return NotFound($"No soldering program found for manufacturing order UID: {manufacturingOfUid}");
                }
                Log.Information("GetSolderingProgramByManufacturingOrderUidLatestOne successfully returned a program for manufacturingOfUid={ManufacturingOfUid}", manufacturingOfUid);
                return Ok(solderingProgram);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred while fetching the soldering program for manufacturingOfUid={ManufacturingOfUid}", manufacturingOfUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
