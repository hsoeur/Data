using Api.Data.Repositories.ToleranceRepo;
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
    public class ToleranceController : ControllerBase
    {
        private readonly IToleranceRepository _toleranceRepository;

        public ToleranceController(IToleranceRepository toleranceRepository)
        {
            _toleranceRepository = toleranceRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère toutes les tolérances",
            Description = "Ce endpoint retourne toutes les tolérances disponibles.",
            OperationId = "GetCollectionTolerances"
        )]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Tolerance>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Tolerance>>> GetCollectionTolerances()
        {
            try
            {
                Log.Information("GetCollectionTolerances called");
                var tolerances = await _toleranceRepository.GetCollectionTolerances();
                if (tolerances == null || !tolerances.Any())
                {
                    Log.Information("GetCollectionTolerances returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionTolerances successfully returned {Count} tolerances", tolerances.Count());
                return Ok(tolerances.ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionTolerances");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Crée une nouvelle tolérance",
            Description = "Ce endpoint permet de créer une nouvelle tolérance avec les informations fournies.",
            OperationId = "CreateTolerance"
        )]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Tolerance))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Tolerance>> CreateTolerance([FromBody] ToleranceDto request)
        {
            if (request == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }
            if (string.IsNullOrWhiteSpace(request.ProductUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'ProductUid' est requis." });
            }

            Log.Information("CreateTolerance called with request: {@Request}", request);

            try
            {
                var tolerance = new Tolerance
                {
                    ToleranceUid = Guid.NewGuid().ToString(),
                    ProductUid = request.ProductUid,
                    OperationNumber = request.OperationNumber,
                    Label = request.Label,
                    MachineUid = request.MachineUid,
                    Unit = request.Unit,
                    Val1 = request.Val1,
                    Val2 = request.Val2,
                    Mode = request.Mode,
                    UserUid = request.UserUid,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                };
                await _toleranceRepository.CreateTolerance(tolerance);
                Log.Information("CreateTolerance successfully created tolerance uid: {ToleranceUid}", tolerance.ToleranceUid);
                return Ok(tolerance);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "CreateTolerance failed with InvalidOperationException");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CreateTolerance failed with exception");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les tolérances les plus récentes pour un produit, une opération et une machine",
            Description = "Ce endpoint retourne les tolérances les plus récentes en fonction des critères fournis (ProductUid, OperationNumber, Machine).",
            OperationId = "GetCollectionTolerancesByProductUidByOperationUidByMachineUidLatestOne"
        )]
        [HttpGet("latest-by-product-operation-machine")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Tolerance>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Tolerance>>> GetCollectionTolerancesByProductUidByOperationUidByMachineUidLatestOne([FromQuery] string productUid, [FromQuery] int operationNumber, [FromQuery] string machine)
        {
            if (string.IsNullOrWhiteSpace(productUid) || string.IsNullOrWhiteSpace(machine))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Les paramètres 'productUid' et 'machine' sont requis." });
            }

            try
            {
                Log.Information("GetCollectionTolerancesByProductUidByOperationUidByMachineUidLatestOne called with productUid: {ProductUid}, operationNumber: {OperationNumber}, machine: {Machine}", productUid, operationNumber, machine);
                var result = await _toleranceRepository.GetCollectionTolerancesByProductUidByOperationByMachineLatestOne(productUid, operationNumber, machine);
                if (result == null || !result.Any())
                {
                    Log.Information("GetCollectionTolerancesByProductUidByOperationUidByMachineUidLatestOne returned no content for given criteria");
                    return NoContent();
                }
                Log.Information("GetCollectionTolerancesByProductUidByOperationUidByMachineUidLatestOne successfully returned {Count} tolerances", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching tolerances");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
