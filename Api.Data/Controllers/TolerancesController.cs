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
    public class TolerancesController : ControllerBase
    {
        private readonly IToleranceRepository _toleranceRepository;

        public TolerancesController(IToleranceRepository toleranceRepository)
        {
            _toleranceRepository = toleranceRepository;
        }

        [SwaggerOperation(
            Summary = "Copie un lot de tolérances vers un autre produit",
            Description = "Crée atomiquement de nouvelles tolérances à partir d'une sélection appartenant au produit source.",
            OperationId = "CopyToleranceBatch"
        )]
        [HttpPost("batch-copy")]
        [ProducesResponseType(typeof(CopyToleranceBatchResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CopyToleranceBatchResponse>> CopyBatch(
            [FromBody] CopyToleranceBatchRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SourceProductUid))
                ModelState.AddModelError(nameof(request.SourceProductUid), "Le produit source est obligatoire.");

            if (string.IsNullOrWhiteSpace(request.TargetProductUid))
                ModelState.AddModelError(nameof(request.TargetProductUid), "Le produit cible est obligatoire.");

            if (request.ToleranceUids is null || request.ToleranceUids.Count == 0)
                ModelState.AddModelError(nameof(request.ToleranceUids), "Au moins une tolérance doit être sélectionnée.");

            if (string.IsNullOrWhiteSpace(request.UserUid))
                ModelState.AddModelError(nameof(request.UserUid), "L'utilisateur est obligatoire.");

            if (request.EndDate <= request.StartDate)
                ModelState.AddModelError(nameof(request.EndDate), "La date de fin doit être postérieure à la date de début.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var result = await _toleranceRepository.CopyBatchAsync(request, cancellationToken);
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (SourceProductNotFoundException)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Produit source introuvable",
                    detail: $"Le produit '{request.SourceProductUid}' n'existe pas.");
            }
            catch (TargetProductAlreadyExistsException)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Produit cible déjà existant",
                    detail: $"Le produit '{request.TargetProductUid}' possède déjà des tolérances.");
            }
            catch (InvalidToleranceSelectionException exception)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Sélection de tolérances invalide",
                    detail: exception.Message);
            }
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
                    UnitUid = request.UnitUid,
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
        [HttpGet("latest-by-product-operation-and-machine")]
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

        [SwaggerOperation(
            Summary = "Met à jour une tolérance existante",
            Description = "Ce endpoint permet de mettre à jour une tolérance existante identifiée par son ToleranceUid.",
            OperationId = "UpdateTolerance"
        )]
        [HttpPut("update/{toleranceUid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Tolerance))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Tolerance>> UpdateTolerance([FromRoute] string toleranceUid, [FromBody] ToleranceDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(toleranceUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'toleranceUid' est requis." });
            }

            if (request == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }

            if (string.IsNullOrWhiteSpace(request.ProductUid))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'ProductUid' est requis." });
            }

            Log.Information("UpdateTolerance called with toleranceUid: {ToleranceUid}, request: {@Request}", toleranceUid, request);

            try
            {
                var tolerance = new Tolerance
                {
                    ToleranceUid = toleranceUid,
                    ProductUid = request.ProductUid,
                    OperationNumber = request.OperationNumber,
                    Label = request.Label,
                    MachineUid = request.MachineUid,
                    UnitUid = request.UnitUid,
                    Val1 = request.Val1,
                    Val2 = request.Val2,
                    Mode = request.Mode,
                    UserUid = request.UserUid,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                };

                var updatedTolerance = await _toleranceRepository.UpdateTolerance(toleranceUid, tolerance);

                if (updatedTolerance == null)
                {
                    Log.Warning("UpdateTolerance: Tolerance with uid {ToleranceUid} not found", toleranceUid);
                    return NotFound(new ProblemDetails { Title = "Tolérance introuvable", Detail = $"Aucune tolérance avec l'uid {toleranceUid} n'a été trouvée." });
                }

                Log.Information("UpdateTolerance successfully updated tolerance uid: {ToleranceUid}", toleranceUid);
                return Ok(updatedTolerance);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "UpdateTolerance failed with InvalidOperationException");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateTolerance failed with exception");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
         
        [SwaggerOperation(
            Summary = "Récupère toutes les tolérances actives",
            Description = "Ce endpoint retourne toutes les tolérances dont la date du jour est comprise entre StartDate et EndDate.",
            OperationId = "GetActiveTolerances"
        )]
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Tolerance>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Tolerance>>> GetActiveTolerances()
        {
            try
            {
                Log.Information("GetActiveTolerances called");
                var tolerances = await _toleranceRepository.GetActiveTolerances();
                if (tolerances == null || !tolerances.Any())
                {
                    Log.Information("GetActiveTolerances returned no content");
                    return NoContent();
                }
                Log.Information("GetActiveTolerances successfully returned {Count} active tolerances", tolerances.Count);
                return Ok(tolerances);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetActiveTolerances");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

