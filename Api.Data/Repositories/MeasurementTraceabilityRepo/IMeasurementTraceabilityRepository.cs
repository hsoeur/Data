using Api.Data.Dtos;
using DataLib;

namespace Api.Data.Repositories.MeasurementTraceabilityRepo
{
    public interface IMeasurementTraceabilityRepository
    {
        Task<MeasurementTraceability> CreateMeasurementTraceability(MeasurementTraceability measurementTraceability);
        Task<List<MeasurementTraceability>> CreateMeasurementTraceabilitiesAtomic(IEnumerable<MeasurementTraceability> measurementTraceabilities);
        Task<MeasurementTraceabilityBatchResultDto> CreateMeasurementTraceabilitiesBestEffort(IEnumerable<(int Index, MeasurementTraceability MeasurementTraceability)> measurementTraceabilities);
        Task<List<MeasurementTraceability>> GetCollectionByProductUidAndOperationNumber(string productUid, int operationNumber);
    }
}
