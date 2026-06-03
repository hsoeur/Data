using Api.Data.Dtos;
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

        public async Task<MeasurementTraceability> CreateMeasurementTraceability(MeasurementTraceability measurementTraceability)
        {
            try
            {
                _context.MeasurementTraceabilities.Add(measurementTraceability);
                await _context.SaveChangesAsync();
                return measurementTraceability;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in CreateMeasurementTraceability for item {@MeasurementTraceability}", measurementTraceability);
                throw;
            }
        }

        public async Task<List<MeasurementTraceability>> CreateMeasurementTraceabilitiesAtomic(IEnumerable<MeasurementTraceability> measurementTraceabilities)
        {
            var items = measurementTraceabilities.ToList();
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    _context.MeasurementTraceabilities.AddRange(items);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return items;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error(ex, "Atomic batch failed. No measurement traceability record was saved.");
                    throw;
                }
            });
        }

        public async Task<MeasurementTraceabilityBatchResultDto> CreateMeasurementTraceabilitiesBestEffort(IEnumerable<(int Index, MeasurementTraceability MeasurementTraceability)> measurementTraceabilities)
        {
            var result = new MeasurementTraceabilityBatchResultDto();
            var items = measurementTraceabilities.ToList();

            for (var i = 0; i < items.Count; i++)
            {
                var originalIndex = items[i].Index;
                var item = items[i].MeasurementTraceability;

                try
                {
                    _context.MeasurementTraceabilities.Add(item);
                    await _context.SaveChangesAsync();
                    result.Created.Add(item);
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();
                    result.Errors.Add(new MeasurementTraceabilityBatchErrorDto
                    {
                        Index = originalIndex,
                        Item = new MeasurementTraceabilityDto
                        {
                            ProductTraceabilityUid = item.ProductTraceabilityUid,
                            ToleranceUid = item.ToleranceUid,
                            Value = item.Value,
                            Classification = item.Classification
                        },
                        Error = ex.InnerException?.Message ?? ex.Message
                    });

                    Log.Error(ex, "Best-effort batch failed for measurement traceability at index {Index}: {@MeasurementTraceability}", originalIndex, item);
                }
            }

            return result;
        }

        public async Task<List<MeasurementTraceability>> GetCollectionByProductUidAndOperationNumber(string productUid, int operationNumber)
        {
            try
            {
                return await _context.MeasurementTraceabilities
                    .AsNoTracking()
                    .Include(m => m.ProductTraceability)
                    .Include(m => m.Tolerance)
                    .Where(m => m.ProductTraceability != null &&
                                m.ProductTraceability.ProductUid == productUid &&
                                m.ProductTraceability.OperationNumber == operationNumber)
                    .OrderBy(m => m.ProductTraceabilityUid)
                    .ThenBy(m => m.ToleranceUid)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetCollectionByProductUidAndOperationNumber for ProductUid {ProductUid} and OperationNumber {OperationNumber}", productUid, operationNumber);
                throw;
            }
        }
    }
}
