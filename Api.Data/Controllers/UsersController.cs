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
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [SwaggerOperation(
            Summary = "Récupère tous les utilisateurs",
            Description = "Ce endpoint retourne la liste complète des utilisateurs disponibles dans le système.",
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
                var utilisateurs = await _userRepository.GetCollectionUsers();
                if (utilisateurs == null || utilisateurs.Count == 0)
                {
                    Log.Information("GetCollectionUsers returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionUsers successfully returned {Count} utilisateurs", utilisateurs.Count);
                return Ok(utilisateurs);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionUsers");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère les utilisateurs actifs",
            Description = "Ce endpoint retourne la liste des utilisateurs actuellement actifs dans le système.",
            OperationId = "GetCollectionCurrentUsers"
        )]
        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<User>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<User>>> GetCollectionCurrentUsers()
        {
            try
            {
                Log.Information("GetCollectionCurrentUsers called");
                var utilisateurs = await _userRepository.GetCollectionCurrentUsers();
                if (utilisateurs == null || utilisateurs.Count == 0)
                {
                    Log.Information("GetCollectionCurrentUsers returned no content");
                    return NoContent();
                }
                Log.Information("GetCollectionCurrentUsers successfully returned {Count} utilisateurs", utilisateurs.Count);
                return Ok(utilisateurs);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetCollectionCurrentUsers");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère un utilisateur par UID",
            Description = "Ce endpoint retourne les informations d'un utilisateur spécifique basé sur son identifiant unique.",
            OperationId = "GetUserByUserUid"
        )]
        [HttpGet("by-user-uid")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> GetUserByUserUid([FromQuery] string userUid)
        {
            if (string.IsNullOrWhiteSpace(userUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'userUid' est requis."
                });
            }

            try
            {
                Log.Information("GetUserByUserUid called with userUid: {UserUid}", userUid);
                // Le dépôt utilise encore le nom 'GetOperatorByOperatorUid' côté interface.
                var utilisateur = await _userRepository.GetOperatorByOperatorUid(userUid);
                if (utilisateur == null)
                {
                    Log.Information("GetUserByUserUid did not find a user for userUid: {UserUid}", userUid);
                    return NotFound();
                }
                Log.Information("GetUserByUserUid successfully returned a user with userUid: {UserUid}", userUid);
                return Ok(utilisateur);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetUserByUserUid with userUid: {UserUid}", userUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
