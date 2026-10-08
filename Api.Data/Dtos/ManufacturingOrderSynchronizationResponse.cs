using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Dtos
{
    public sealed record ManufacturingOrderSynchronizationResponse(
        int ManufacturingOrderId,
        bool IsSynchronized,
        bool HasQuantityDesynchronization,
        bool HasSerialNumberDesynchronization,
        ManufacturingOrderSynchronizationValue? Quantity,
        ManufacturingOrderSynchronizationValue? SerialNumber,
        string Message);

    public sealed record ManufacturingOrderSynchronizationValue(
        [property: SwaggerSchema(Description = "Valeur présente dans la base de production.")]
        string OldValue,
        [property: SwaggerSchema(Description = "Valeur provenant de la GPAO.")]
        string NewValue);
}
