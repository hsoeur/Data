using DataLib;

namespace Api.Data.Repositories.BillOfMaterialRepo
{
    public interface IBillOfMaterialRepository
    {
        Task<ICollection<BillOfMaterial>> GetBillOfMaterialsByManufacturingOrderUid(int manufacturingOrderUid);
        Task<ICollection<BillOfMaterial>> GetBillOfMaterialsWithoutLabelsByManufacturingOrderUid(int manufacturingOrderUid);
        Task<ICollection<BillOfMaterial>> GetBillOfMaterialsLabelsByArticleUid(string articleUid);
        Task<BillOfMaterial> GetBillOfMaterialByArticleUidAndbyComponentUid(string articleUid, string componentUid);
        public Task<ICollection<Article>> GetQuantitiesForArticlesInBillOfMaterialsByArticleUids();
    }
}
