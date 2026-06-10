using Api.Data.Repositories.LabelTypeRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LabelTypesController : ControllerBase
    {
        private readonly ILabelTypeRepository _LabelTypeRepository;

        public LabelTypesController(ILabelTypeRepository LabelTypeRepository)
        {
            _LabelTypeRepository = LabelTypeRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère toutes les utilisations",
            Description = "Ce endpoint retourne toutes les utilisations disponibles dans la base de données.",
            OperationId = "GetCollectionLabelTypes"
        )]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<LabelType>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<LabelType>>> GetCollectionLabelTypes()
        {
            try
            {
                Log.Information("GetCollectionLabelTypes called");
                var LabelTypes = await _LabelTypeRepository.GetCollectionLabelTypes();
                if (LabelTypes == null || !LabelTypes.Any())
                {
                    Log.Information("GetCollectionLabelTypes returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionLabelTypes successfully returned {Count} LabelTypes", LabelTypes.Count());
                return Ok(LabelTypes.ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionLabelTypes");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Crée une nouvelle utilisation",
            Description = "Ce endpoint permet de créer une nouvelle utilisation dans la base de données.",
            OperationId = "CreateLabelType"
        )]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LabelType))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LabelType>> CreateLabelType([FromBody] LabelType LabelType)
        {
            if (LabelType == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }

            Log.Information("CreateLabelType called with LabelType: {@LabelType}", LabelType);

            try
            {
                await _LabelTypeRepository.CreateLabelType(LabelType);
                Log.Information("CreateLabelType successfully created LabelType with Id: {Id}", LabelType.LabelTypeUid);
                return Ok(LabelType);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateLabelType with LabelType: {@LabelType}", LabelType);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
