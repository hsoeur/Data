using Api.Data.Repositories.LabelRepo;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
 [Route("api/[controller]")]
 [ApiController]
 [Produces("application/json")]
 public class LabelsController : ControllerBase
 {
 private readonly ILabelRepository _labelRepository;

 public LabelsController(ILabelRepository labelRepository)
 {
 _labelRepository = labelRepository;
 }

 // Empty (vierge) controller scaffolding—endpoints to be added later.
 [SwaggerOperation(
 Summary = "Controller vierge pour Label",
 Description = "Points d'API à implémenter.",
 OperationId = "Labels_Vierge")]
 [HttpGet("health")] 
 public IActionResult Health() => Ok("Labels controller is ready.");
 }
}
