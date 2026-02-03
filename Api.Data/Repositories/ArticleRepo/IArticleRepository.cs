using DataLib;

namespace Api.Data.Repositories.ArticleRepo
{
    public interface IArticleRepository
    {
        public Task<ICollection<Article>> GetArticleCollectionByManufacturingOrderUid(int manufacturingOrderUid);
        public Task<Article?> GetArticleByArticleUid(string articleUid);
        public Task<string> GetArticleStoreByArticleUid(string articleUid);
        public Task<Article?> GetArticleByArticleSupplierReference(string supplierReference);
        public Task<Article?> GetArticleByArticleSupplierReferenceKitBornes(string supplierReference, string productReference);
        public Task<ICollection<Article>> GetAllArticles();
    }
}
