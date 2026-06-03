using DataLib;

namespace Api.Data.Dtos
{
    public class MeasurementTraceabilityDto
    {
        public string ProductTraceabilityUid { get; set; } = string.Empty;
        public string ToleranceUid { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Classification { get; set; } = string.Empty;
    }

    public class MeasurementTraceabilityBatchResultDto
    {
        public List<MeasurementTraceability> Created { get; set; } = new();
        public List<MeasurementTraceabilityBatchErrorDto> Errors { get; set; } = new();
    }

    public class MeasurementTraceabilityBatchErrorDto
    {
        public int Index { get; set; }
        public MeasurementTraceabilityDto Item { get; set; } = new();
        public string Error { get; set; } = string.Empty;
    }
}
