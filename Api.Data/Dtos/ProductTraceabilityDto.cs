namespace Api.Data.Dtos
{
    public class ProductTraceabilityDto
    {
        public string ProductUid { get; set; } = string.Empty;
        public int ProductSerialNumber { get; set; } 
        public string ProductStatusUid { get; set; } = string.Empty;
        public DateTimeOffset CreationDate { get; set; } 
        public int ManufacturingOrderUid { get; set; }
        public string UserUid { get; set; } = string.Empty;
        public decimal CycleTime { get; set; } 
        public int OperationNumber { get; set; }
        public string MachineUid { get; set; } = string.Empty;
    }
}
