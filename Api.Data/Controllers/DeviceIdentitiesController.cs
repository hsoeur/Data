using Api.Data.Repositories.DeviceIdentityRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DeviceIdentitiesController : ControllerBase
    {
        private readonly IDeviceIdentityRepository _repository;

        public DeviceIdentitiesController(IDeviceIdentityRepository repository)
        {
            _repository = repository;
        }

        [SwaggerOperation(Summary = "Récupère toutes les associations appareil (EquipSN <-> MAC)", OperationId = "GetAllDeviceIdentities")]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<DeviceIdentity>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<DeviceIdentity>>> GetAllDeviceIdentities()
        {
            try
            {
                Log.Information("GetAllDeviceIdentities called");
                var items = await _repository.GetAllAsync();
                if (items == null || !items.Any())
                {
                    Log.Information("GetAllDeviceIdentities returned no content");
                    return NoContent();
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetAllDeviceIdentities failed");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération des associations appareil: {ex.Message}");
            }
        }



        [SwaggerOperation(Summary = "Récupère une association appareil par numéro de série (EquipSN)", OperationId = "GetDeviceIdentityByEquipSN")]
        [HttpGet("by-equipsn")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeviceIdentity))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DeviceIdentity>> GetDeviceIdentityByEquipSN([FromQuery] string equipSN)
        {
            if (string.IsNullOrWhiteSpace(equipSN))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'equipSN' est requis." });
            }

            try
            {
                Log.Information("GetDeviceIdentityByEquipSN called with equipSN: {EquipSN}", equipSN);
                var item = await _repository.GetByEquipSNAsync(equipSN);
                if (item == null)
                {
                    Log.Information("GetDeviceIdentityByEquipSN not found for equipSN: {EquipSN}", equipSN);
                    return NotFound($"Association appareil avec EquipSN '{equipSN}' introuvable.");
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetDeviceIdentityByEquipSN failed for EquipSN {EquipSN}", equipSN);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération de l'association appareil pour EquipSN '{equipSN}': {ex.Message}");
            }
        }

        [SwaggerOperation(Summary = "Récupère une association appareil par adresse MAC", OperationId = "GetDeviceIdentityByMacAddress")]
        [HttpGet("by-mac")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeviceIdentity))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DeviceIdentity>> GetDeviceIdentityByMacAddress([FromQuery] string macAddress)
        {
            if (string.IsNullOrWhiteSpace(macAddress))
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le paramètre 'macAddress' est requis." });
            }

            try
            {
                Log.Information("GetDeviceIdentityByMacAddress called with macAddress: {MacAddress}", macAddress);
                var item = await _repository.GetByMacAddressValueAsync(macAddress);
                if (item == null)
                {
                    Log.Information("GetDeviceIdentityByMacAddress not found for macAddress: {MacAddress}", macAddress);
                    return NotFound($"Association appareil avec MAC '{macAddress}' introuvable.");
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetDeviceIdentityByMacAddress failed for MacAddress {MacAddress}", macAddress);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération de l'association appareil pour MAC '{macAddress}': {ex.Message}");
            }
        }

        [SwaggerOperation(Summary = "Crée une nouvelle association appareil (EquipSN <-> MAC)", OperationId = "CreateDeviceIdentity")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DeviceIdentity))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DeviceIdentity>> CreateDeviceIdentity([FromBody] DeviceIdentity deviceIdentity)
        {
            if (deviceIdentity == null)
            {
                return BadRequest(new ProblemDetails { Title = "Paramètre invalide", Detail = "Le corps de la requête est requis." });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Vérifier si l'EquipSN existe déjà
                var existingByEquipSN = await _repository.GetByEquipSNAsync(deviceIdentity.EquipSN);
                if (existingByEquipSN != null)
                {
                    return Conflict(new ProblemDetails
                    {
                        Title = "Conflit de duplication",
                        Detail = $"Un appareil avec le numéro de série '{deviceIdentity.EquipSN}' existe déjà."
                    });
                }

                // Vérifier si la MAC existe déjà
                var existingByMac = await _repository.GetByMacAddressValueAsync(deviceIdentity.MacAddressValue);
                if (existingByMac != null)
                {
                    return Conflict(new ProblemDetails
                    {
                        Title = "Conflit de duplication",
                        Detail = $"Un appareil avec l'adresse MAC '{deviceIdentity.MacAddressValue}' existe déjà."
                    });
                }

                var created = await _repository.CreateAsync(deviceIdentity);
                Log.Information("CreateDeviceIdentity created association with id: {Id}", created.Id);
                return CreatedAtAction(nameof(GetDeviceIdentityByEquipSN), new { equipSN = created.EquipSN }, created);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CreateDeviceIdentity failed for EquipSN {EquipSN}", deviceIdentity?.EquipSN);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la création de l'association appareil: {ex.Message}");
            }
        }

        

       
    }
}
