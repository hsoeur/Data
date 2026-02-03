using DataLib;

namespace Api.Data.Repositories.MeasurementTraceabilityRepo
{
    public interface IMeasurementTraceabilityRepository
    {
        void CreateMeasurementTraceability(MeasurementTraceability tracabiliteMesure);
        Task<List<MeasurementTraceability>> GetCollectionMeasurementTraceabilityByArticleUidByOperationUid(string articleUid, int operationNumber);
    }
}
