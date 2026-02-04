using Api.Data.Dtos;
using DataLib;

namespace Api.Data.Repositories.ProductTraceabilityRepo
{
    public interface IProductTraceabilityRepository
    {
        void CreateProductTraceability(ProductTraceability productTraceability);

        Task<ProductTraceability> GetLatestProductTraceabilityWithArticleUid(string articleUid);
        Task<List<ProductTraceability>> GetCollectionProductTraceability(string manufacturingOrderUid, string articleUid);
        Task<ProductTraceability?> GetProductTraceabilityByProductBySerialNumberByOperationLatestOne(string product, string serialNumber, string operationUid);
        Task<List<ProductTraceability>> GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUid(string manufacturingOrderUid, string operationUid);
        Task<List<ProductTraceability>> GetCollectionProductTraceabilityByManufacturingOrderUidByOperationLatestOne(string manufacturingOrderUid, string operationUid);
        Task<ProductTraceabilityStatsDto> GetTraceabilityStatsByManufacturingOrderUidAndOperationUid(string manufacturingOrderUid, string operationUid);
        Task<ProductTraceabilityStatsDto> GetTraceabilityStatsByManufacturingOrderUidAndOperationUidToday(string manufacturingOrderUid, string operationUid);
        Task<ProductTraceability?> GetProductTraceabilityByManufacturingOrderProductAndSerialNumber(
            string manufacturingOrderUid,
            string productUid,
            string serialNumber);

        Task<ProductTraceability?> GetProductTraceabilityWithHighestSerialNumberByManufacturingOrderUid(string manufacturingOrderUid);
    }
}
