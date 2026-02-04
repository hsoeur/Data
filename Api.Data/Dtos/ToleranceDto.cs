using DataLib;

namespace Api.Data.Dtos
{
    public class ToleranceDto
    {
        public required string ProductUid { get; set; }
        public required int OperationNumber { get; set; }
        public required string MachineUid { get; set; }
        public required string Label { get; set; }
        public required Mode Mode { get; set; }
        public required string Val1 { get; set; }
        public required string Val2 { get; set; }
        public required Unit Unit { get; set; }
        public required string UserUid { get; set; }
        public required DateTimeOffset StartDate { get; set; }
        public required DateTimeOffset EndDate { get; set; }
    }
}
