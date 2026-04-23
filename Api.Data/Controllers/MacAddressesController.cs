using Api.Data.Dtos;
using Api.Data.Repositories.MacAddressRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class MacAddressesController : ControllerBase
    {
        private readonly IMacAddressRepository _repository;

        public MacAddressesController(IMacAddressRepository repository)
        {
            _repository = repository;
        }

        [SwaggerOperation(Summary = "Récupère la prochaine adresse MAC disponible", OperationId = "GetNextMac")]
        [HttpGet("next-mac")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NextMacAddressDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<NextMacAddressDto>> GetNextMac()
        {
            try
            {
                Log.Information("GetNextMac called");
                var macAddress = await _repository.GenerateAndReserveNextMacAddressAsync();

                Log.Information("MAC generated and reserved -> Id={Id} Address={MacAddress}", macAddress.Id, macAddress.MacAddressValue);

                var result = new NextMacAddressDto
                {
                    MacId = macAddress.Id,
                    MacAddress = macAddress.MacAddressValue
                };

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "GetNextMac failed - MAC address overflow");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Dépassement de la plage d'adresses MAC",
                    Detail = ex.Message
                });
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "GetNextMac failed");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Erreur lors de la génération de l'adresse MAC",
                    Detail = $"Erreur lors de la récupération de la prochaine adresse MAC: {ex.Message}"
                });
            }
        }

        [SwaggerOperation(Summary = "R�cup�re toutes les adresses MAC", OperationId = "GetAllMacAddresses")]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<MacAddress>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<MacAddress>>> GetAllMacAddresses()
        {
            try
            {
                Log.Information("GetAllMacAddresses called");
                var items = await _repository.GetAllAsync();
                if (items == null || !items.Any())
                {
                    Log.Information("GetAllMacAddresses returned no content");
                    return NoContent();
                }
                return Ok(items);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "GetAllMacAddresses failed");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la r�cup�ration des adresses MAC: {ex.Message}");
            }
        }

        [SwaggerOperation(Summary = "R�cup�re une adresse MAC par ID", OperationId = "GetMacAddressById")]
        [HttpGet("by-id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MacAddress))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MacAddress>> GetMacAddressById([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails { Title = "Param�tre invalide", Detail = "Le param�tre 'id' doit �tre sup�rieur � 0." });
            }

            try
            {
                Log.Information("GetMacAddressById called with id: {Id}", id);
                var item = await _repository.GetByIdAsync(id);
                if (item == null)
                {
                    Log.Information("GetMacAddressById not found for id: {Id}", id);
                    return NotFound($"Adresse MAC avec ID {id} introuvable.");
                }
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "GetMacAddressById failed for ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la r�cup�ration de l'adresse MAC {id}: {ex.Message}");
            }
        }

        [SwaggerOperation(Summary = "Cr�e une adresse MAC", OperationId = "CreateMacAddress")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MacAddress))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MacAddress>> CreateMacAddress([FromBody] MacAddress macAddress)
        {
            if (macAddress == null)
            {
                return BadRequest(new ProblemDetails { Title = "Param�tre invalide", Detail = "Le corps de la requ�te est requis." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _repository.CreateAsync(macAddress);
                Log.Information("CreateMacAddress created MAC with id: {Id}", created.Id);
                return CreatedAtAction(nameof(GetMacAddressById), new { id = created.Id }, created);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "CreateMacAddress failed for MacAddress {MacAddress}", macAddress?.MacAddressValue);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la cr�ation de l'adresse MAC: {ex.Message}");
            }
        }

        [SwaggerOperation(Summary = "Met � jour une adresse MAC", OperationId = "UpdateMacAddress")]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMacAddress([FromBody] MacAddress macAddress)
        {
            if (macAddress == null)
            {
                return BadRequest(new ProblemDetails { Title = "Param�tre invalide", Detail = "Le corps de la requ�te est requis." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updated = await _repository.UpdateAsync(macAddress);
                if (!updated)
                {
                    Log.Information("UpdateMacAddress not found for id: {Id}", macAddress.Id);
                    return NotFound($"Adresse MAC avec ID {macAddress.Id} introuvable.");
                }
                Log.Information("UpdateMacAddress updated MAC with id: {Id}", macAddress.Id);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "UpdateMacAddress failed for ID {Id}", macAddress?.Id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la mise � jour de l'adresse MAC: {ex.Message}");
            }
        }

        [SwaggerOperation(Summary = "Supprime une adresse MAC", OperationId = "DeleteMacAddress")]
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMacAddress([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ProblemDetails { Title = "Param�tre invalide", Detail = "Le param�tre 'id' doit �tre sup�rieur � 0." });
            }

            try
            {
                Log.Information("DeleteMacAddress called with id: {Id}", id);
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                    Log.Information("DeleteMacAddress not found for id: {Id}", id);
                    return NotFound($"Adresse MAC avec ID {id} introuvable.");
                }
                Log.Information("DeleteMacAddress deleted MAC with id: {Id}", id);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "DeleteMacAddress failed for ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la suppression de l'adresse MAC: {ex.Message}");
            }
        }
    }

}
