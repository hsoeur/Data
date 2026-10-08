using System.Globalization;
using Api.Data.Dtos;
using Api.Data.Repositories.ManufacturingOrderRepo;
using Api.Data.Repositories.ManufacturingOrderSynchronizationRepo;
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
        private readonly IManufacturingOrderSynchronizationRepository _synchronizationRepository;

        public ManufacturingOrderValidationController(
            IManufacturingOrderRepository manufacturingOrderRepository,
            IManufacturingOrderSynchronizationRepository synchronizationRepository)
        {
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _synchronizationRepository = synchronizationRepository;
        }

        [HttpGet("{manufacturingOrderId:int}/synchronization")]
        [SwaggerOperation(
            Summary = "Vérifie la synchronisation d'un ordre de fabrication",
            Description = "Recherche les notifications non traitées concernant la quantité ou le premier numéro de série de l'OF. OldValue correspond à la base de production et NewValue à la GPAO.",
            OperationId = "ValidateManufacturingOrderSynchronization")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ManufacturingOrderSynchronizationResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<ManufacturingOrderSynchronizationResponse>> ValidateManufacturingOrderSynchronization(
            [FromRoute] int manufacturingOrderId)
        {
            try
            {
                Log.Information(
                    "ValidateManufacturingOrderSynchronization called for OF {ManufacturingOrderId}",
                    manufacturingOrderId);

                var order = await _manufacturingOrderRepository.GetManufacturingOrderForValidation(manufacturingOrderId);
                if (order is null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Ordre de fabrication introuvable",
                        Detail = $"Aucun ordre de fabrication ne correspond à l'identifiant {manufacturingOrderId}."
                    });
                }

                var notifications = await _synchronizationRepository.GetDesynchronizationsAsync(manufacturingOrderId);
                var quantityNotification = notifications
                    .Where(notification => notification.PropertyName.Equals("Quantity", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(notification => notification.ModifiedAt)
                    .FirstOrDefault();
                var serialNumberNotification = notifications
                    .Where(notification =>
                        notification.PropertyName.Equals("SerialNumber", StringComparison.OrdinalIgnoreCase) ||
                        notification.PropertyName.Equals("FirstSerialNumber", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(notification => notification.ModifiedAt)
                    .FirstOrDefault();

                var hasQuantityDesynchronization = quantityNotification is not null;
                var hasSerialNumberDesynchronization = serialNumberNotification is not null;
                var isSynchronized = !hasQuantityDesynchronization && !hasSerialNumberDesynchronization;

                var quantity = quantityNotification is null
                    ? null
                    : new ManufacturingOrderSynchronizationValue(
                        quantityNotification.OldValue,
                        quantityNotification.NewValue);
                var serialNumber = serialNumberNotification is null
                    ? null
                    : new ManufacturingOrderSynchronizationValue(
                        serialNumberNotification.OldValue,
                        serialNumberNotification.NewValue);

                var message = (hasQuantityDesynchronization, hasSerialNumberDesynchronization) switch
                {
                    (false, false) => "L'ordre de fabrication est synchronisé.",
                    (true, false) => "L'ordre de fabrication est désynchronisé au niveau de la quantité.",
                    (false, true) => "L'ordre de fabrication est désynchronisé au niveau du numéro de série.",
                    _ => "L'ordre de fabrication est désynchronisé au niveau de la quantité et du numéro de série."
                };

                return Ok(new ManufacturingOrderSynchronizationResponse(
                    manufacturingOrderId,
                    isSynchronized,
                    hasQuantityDesynchronization,
                    hasSerialNumberDesynchronization,
                    quantity,
                    serialNumber,
                    message));
            }
            catch (Exception ex)
            {
                Log.Error(
                    ex,
                    "Exception occurred while validating synchronization for manufacturing order {ManufacturingOrderId}",
                    manufacturingOrderId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Erreur interne",
                    Detail = "Une erreur est survenue pendant la vérification de la synchronisation de l'ordre de fabrication."
                });
            }
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
