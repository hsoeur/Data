using Api.Data.Repositories.MachineRepo;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     [Produces("application/json")]
     public class MachinesController : ControllerBase
     {
         private readonly IMachineRepository _machineRepository;

         public MachinesController(IMachineRepository machineRepository)
         {
             _machineRepository = machineRepository;
         }

         [SwaggerOperation(
             Summary = "Récupère toutes les machines",
             Description = "Ce endpoint retourne la liste complète des machines.",
             OperationId = "GetAllMachines"
         )]
         [HttpGet("all")]
         [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<Machine>))]
         [ProducesResponseType(StatusCodes.Status204NoContent)]
         [ProducesResponseType(StatusCodes.Status500InternalServerError)]
         public async Task<ActionResult<ICollection<Machine>>> GetAllMachines()
         {
             try
             {
                 Log.Information("GetAllMachines called");
                 var machines = await _machineRepository.GetAllMachines();
                 if (machines == null || machines.Count ==0)
                 {
                     Log.Information("GetAllMachines returned no content");
                     return NoContent();
                 }
                 Log.Information("GetAllMachines successfully returned {Count} machines", machines.Count);
                 return Ok(machines);
             }
             catch (System.Exception ex)
             {
                 Log.Error(ex, "Exception occurred while processing GetAllMachines");
                 return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
             }
         }

         [SwaggerOperation(
             Summary = "Récupère une machine par UID",
             Description = "Ce endpoint retourne les informations d'une machine spécifique basée sur son identifiant unique.",
             OperationId = "GetMachineByMachineUid"
         )]
         [HttpGet("by-machine-uid")]
         [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Machine))]
         [ProducesResponseType(StatusCodes.Status400BadRequest)]
         [ProducesResponseType(StatusCodes.Status404NotFound)]
         [ProducesResponseType(StatusCodes.Status500InternalServerError)]
         public async Task<ActionResult<Machine>> GetMachineByMachineUid([FromQuery] string machineUid)
         {
             if (string.IsNullOrWhiteSpace(machineUid))
             {
                 return BadRequest(new ProblemDetails
                 {
                     Title = "Paramètre invalide",
                     Detail = "Le paramètre 'machineUid' est requis."
                 });
             }

             try
             {
                 Log.Information("GetMachineByMachineUid called with machineUid: {MachineUid}", machineUid);
                 var machine = await _machineRepository.GetMachineByMachineUid(machineUid);
                 if (machine == null)
                 {
                     Log.Information("GetMachineByMachineUid did not find a machine for machineUid: {MachineUid}", machineUid);
                     return NotFound();
                 }
                 Log.Information("GetMachineByMachineUid successfully returned a machine with machineUid: {MachineUid}", machineUid);
                 return Ok(machine);
             }
             catch (System.Exception ex)
             {
                 Log.Error(ex, "Exception occurred while processing GetMachineByMachineUid with machineUid: {MachineUid}", machineUid);
                 return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
             }
         }
     }
}
