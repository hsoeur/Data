using System.Globalization;
using Api.Data.Dtos;
using Api.Data.Repositories.ManufacturingOrderRepo;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Controllers
{
    [Route("api/ManufacturingOrders")]
    [ApiController]
    [Produces("application/json")]
    public sealed class ManufacturingOrderValidationController : ControllerBase
    {
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;

        public ManufacturingOrderValidationController(IManufacturingOrderRepository manufacturingOrderRepository)
        {
            _manufacturingOrderRepository = manufacturingOrderRepository;
        }

        [HttpPost("validate")]
        [SwaggerOperation(
            Summary = "Valide un ordre de fabrication",
            Description = "Vérifie que le suffixe produit correspond au produit de l'OF et que le numéro de série appartient à sa plage.",
            OperationId = "ValidateManufacturingOrder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ManufacturingOrderValidationResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<ManufacturingOrderValidationResponse>> ValidateManufacturingOrder(
            [FromBody] ManufacturingOrderValidationRequest request)
        {
            try
            {
                // [ApiController] garantit les formats avant l'entrée dans l'action.
                var manufacturingOrderId = int.Parse(request.ManufacturingOrderId, CultureInfo.InvariantCulture);
                var serialNumber = int.Parse(request.SerialNumber, CultureInfo.InvariantCulture);

                Log.Information(
                    "ValidateManufacturingOrder called for OF {ManufacturingOrderId}, suffix {ProductSuffix} and serial {SerialNumber}",
                    manufacturingOrderId,
                    request.ProductSuffix,
                    serialNumber);

                var order = await _manufacturingOrderRepository.GetManufacturingOrderForValidation(manufacturingOrderId);
                if (order is null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Ordre de fabrication introuvable",
                        Detail = $"Aucun ordre de fabrication ne correspond à l'identifiant {request.ManufacturingOrderId}."
                    });
                }

                // Le suffixe du code-barres est sur 6 caractères et peut donc être
                // complété par des espaces à droite (par exemple "0640A ").
                var productSuffix = request.ProductSuffix.TrimEnd(' ');
                var productMatches = productSuffix.Length > 0
                    && order.ProductUid.EndsWith(
                        productSuffix,
                        StringComparison.OrdinalIgnoreCase);

                var lastSerialAsLong = (long)order.FirstSerialNumber + order.Quantity - 1L;
                var hasValidRange = order.Quantity > 0 && lastSerialAsLong <= int.MaxValue;
                var serialNumberInRange = hasValidRange
                    && serialNumber >= order.FirstSerialNumber
                    && serialNumber <= lastSerialAsLong;
                var lastSerialNumber = hasValidRange ? (int)lastSerialAsLong : order.FirstSerialNumber;

                var message = (productMatches, serialNumberInRange) switch
                {
                    (true, true) => "L'ordre de fabrication est valide pour ce produit et ce numéro de série.",
                    (false, true) => "Le suffixe produit ne correspond pas au produit de l'OF.",
                    (true, false) => "Le numéro de série n'appartient pas à la plage de l'OF.",
                    _ => "Le suffixe produit ne correspond pas au produit de l'OF et le numéro de série n'appartient pas à sa plage."
                };

                return Ok(new ManufacturingOrderValidationResponse(
                    productMatches && serialNumberInRange,
                    productMatches,
                    serialNumberInRange,
                    order.ManufacturingOrderUid,
                    order.ProductUid,
                    order.FirstSerialNumber,
                    lastSerialNumber,
                    message));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while validating manufacturing order {ManufacturingOrderId}", request.ManufacturingOrderId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Erreur interne",
                    Detail = "Une erreur est survenue pendant la validation de l'ordre de fabrication."
                });
            }
        }
    }
}
