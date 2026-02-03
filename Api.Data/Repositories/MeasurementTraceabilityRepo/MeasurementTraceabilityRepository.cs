using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.MeasurementTraceabilityRepo
{
    public class MeasurementTraceabilityRepository : IMeasurementTraceabilityRepository
    {
        private readonly AppDbContext _context;

        public MeasurementTraceabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public void CreateMeasurementTraceability(MeasurementTraceability tracabiliteMesure)
        {
            _context.MeasurementTraceabilities.Add(tracabiliteMesure);
            _context.SaveChanges();
        }

        public async Task<List<MeasurementTraceability>> GetCollectionMeasurementTraceabilityByArticleUidByOperationUid(string articleUid, int operationNumber)
        {
            List<MeasurementTraceability> measurementTraceabilities = new List<MeasurementTraceability>();

            Log.Information("Starting execution of GetCollectionMeasurementTraceabilityByArticleUidByOperationUid. ArticleUid: {ArticleUid}, OperationNumber: {OperationNumber}", articleUid, operationNumber);

            try
            {
                measurementTraceabilities = await _context.MeasurementTraceabilities
                    .Where(p => p.ProductUid == articleUid)
                    .Where(p => p.OperationNumber == operationNumber)
                    .ToListAsync();

                Log.Information("Successfully retrieved data for ArticleUid: {ArticleUid}, OperationNumber: {OperationNumber}. Number of results: {Count}",
                    articleUid, operationNumber, measurementTraceabilities.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred in GetCollectionMeasurementTraceabilityByArticleUidByOperationUid for ArticleUid: {ArticleUid}, OperationNumber: {OperationNumber}",
                    articleUid, operationNumber);
                throw;
            }

            Log.Information("Finished execution of GetCollectionMeasurementTraceabilityByArticleUidByOperationUid. ArticleUid: {ArticleUid}, OperationNumber: {OperationNumber}",
                articleUid, operationNumber);

            return measurementTraceabilities;
        }

    }
}
