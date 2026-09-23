namespace Api.Data.Dtos
{
    public sealed record ManufacturingOrderValidationResponse(
        bool IsValid,
        bool ProductMatches,
        bool SerialNumberInRange,
        int ManufacturingOrderId,
        string ProductUid,
        int FirstSerialNumber,
        int LastSerialNumber,
        string Message);
}
