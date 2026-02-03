using Api.Data.Dtos;
using Api.Data.Repositories.UserRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OperatorController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public OperatorController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère tous les opérateurs",
            Description = "Ce endpoint retourne la liste complète des opérateurs disponibles dans le système.",
            OperationId = "GetCollectionUsers"
        )]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<User>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<User>>> GetCollectionUsers()
        {
            try
            {
                Log.Information("GetCollectionUsers called");
                var operateurs = await _userRepository.GetCollectionUsers();
                if (operateurs == null || operateurs.Count == 0)
                {
                    Log.Information("GetCollectionUsers returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionUsers successfully returned {Count} operateurs", operateurs.Count);
                return Ok(operateurs);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionUsers");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les opérateurs actifs",
            Description = "Ce endpoint retourne la liste des opérateurs actuellement actifs dans le système.",
            OperationId = "GetCollectionCurrentOperator"
        )]
        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<User>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<User>>> GetCollectionCurrentOperator()
        {
            try
            {
                Log.Information("GetCollectionCurrentOperator called");
                var operateurs = await _userRepository.GetCollectionCurrentUsers();
                if (operateurs == null || operateurs.Count == 0)
                {
                    Log.Information("GetCollectionCurrentOperator returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionCurrentOperator successfully returned {Count} operateurs", operateurs.Count);
                return Ok(operateurs);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionCurrentOperator");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère un opérateur par UID",
            Description = "Ce endpoint retourne les informations d'un opérateur spécifique basé sur son identifiant unique.",
            OperationId = "GetOperatorByOperatorUid"
        )]
        [HttpGet("by-operator-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> GetOperatorByOperatorUid([FromQuery] string operatorUid)
        {
            if (string.IsNullOrWhiteSpace(operatorUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'operatorUid' est requis."
                });
            }

            try
            {
                Log.Information("GetOperatorByOperatorUid called with operatorUid: {OperatorUid}", operatorUid);
                var operateur = await _userRepository.GetOperatorByOperatorUid(operatorUid);
                if (operateur == null)
                {
                    Log.Information("GetOperatorByOperatorUid did not find an operator for operatorUid: {OperatorUid}", operatorUid);
                    return NotFound();
                }
                Log.Information("GetOperatorByOperatorUid successfully returned an operator with operatorUid: {OperatorUid}", operatorUid);
                return Ok(operateur);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetOperatorByOperatorUid with operatorUid: {OperatorUid}", operatorUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
