namespace Api.Data.Dtos
{
    public class MeasurementTraceabilityDto
    {
        public string ProductUid { get; set; } = string.Empty;
        public int SerialNumber { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Classification { get; set; } = string.Empty;
        public DateTimeOffset CreationDate { get; set; }
        public int OperationNumber { get; set; }
        public string UnitUid { get; set; } = string.Empty;
        public string UserUid { get; set; } = string.Empty;
        public string MachineUid { get; set; } = string.Empty;
    }
}
