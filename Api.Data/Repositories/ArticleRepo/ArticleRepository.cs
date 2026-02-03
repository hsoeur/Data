using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.ArticleRepo
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly AppDbContext _context;

        public ArticleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Article?> GetArticleByArticleSupplierReference(string supplierReference)
        {
            Log.Information("GetArticleByArticleSupplierReference called with refFournisseur: {RefFournisseur}", supplierReference);

            try
            {
                var article = await _context.Articles
                    .FirstOrDefaultAsync(item => item.SupplierReference == supplierReference);

                if (article == null)
                {
                    Log.Warning("No article found with ReferenceFournisseur: {RefFournisseur}", supplierReference);
                }
                else
                {
                    Log.Information("Article found with ReferenceFournisseur: {RefFournisseur}", supplierReference);
                }

                return article;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get article by ReferenceFournisseur: {RefFournisseur}. Exception: {Exception}", supplierReference, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<Article?> GetArticleByArticleUid(string articleUid)
        {
            Log.Information("GetArticleByUid called with uid: {Uid}", articleUid);

            try
            {
                var article = await _context.Articles
                    .FirstOrDefaultAsync(a => a.ArticleUid == articleUid);

                if (article == null)
                {
                    Log.Warning("No article found with UidArticle: {Uid}", articleUid);
                }
                else
                {
                    Log.Information("Article found with UidArticle: {Uid}", articleUid);
                }

                return article;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get article by UidArticle: {Uid}. Exception: {Exception}", articleUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<ICollection<Article>> GetArticleCollectionByManufacturingOrderUid(int manufacturingOrderUid)
        {
            Log.Information("GetArticleCollectionByManufacturingOrderUid called with uidOF: {UidOF}", manufacturingOrderUid);

            try
            {
                var uidArticle = await _context.ManufacturingOrders
                    .Where(of => of.ManufacturingOrderUid == manufacturingOrderUid)
                    .Select(of => of.ProductUid)
                    .FirstOrDefaultAsync();

                if (uidArticle == null)
                {
                    Log.Warning("No product found for OrdreFabrication with uidOF: {UidOF}", manufacturingOrderUid);
                    return new List<Article>(); // Return an empty list if no product is found
                }

                var components = await _context.BillOfMaterials
                    .Where(n => n.Product == uidArticle)
                    .Select(n => n.Component)
                    .ToListAsync();

                var articles = await _context.Articles
                    .Where(a => components.Contains(a.ArticleUid))
                    .ToListAsync();

                Log.Information("Found {Count} articles for OrdreFabrication with uidOF: {UidOF}", articles.Count, manufacturingOrderUid);

                return articles;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get articles by OrdreFabrication uidOF: {UidOF}. Exception: {Exception}", manufacturingOrderUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<string> GetArticleStoreByArticleUid(string articleUid)
        {
            Log.Information("GetMagasinByUidArticle called with uidArticle: {UidArticle}", articleUid);

            try
            {
                var magasin = await _context.Articles
                    .Where(art => art.ArticleUid == articleUid)
                    .Select(art => art.Store)
                    .FirstOrDefaultAsync();

                if (magasin == null)
                {
                    Log.Warning("No magasin found for Article with UidArticle: {UidArticle}", articleUid);
                }
                else
                {
                    Log.Information("Magasin found for Article with UidArticle: {UidArticle}", articleUid);
                }

                return magasin;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get magasin by UidArticle: {UidArticle}. Exception: {Exception}", articleUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }
        public async Task<ICollection<Article>> GetAllArticles()
        {
            Log.Information("GetAllArticles called");

            try
            {
                var articles = await _context.Articles.ToListAsync();

                if (articles == null || !articles.Any())
                {
                    Log.Warning("No articles found in the database.");
                    return new List<Article>(); // Return an empty list if no articles are found
                }

                Log.Information("Found {Count} articles in the database.", articles.Count);

                return articles;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get all articles. Exception: {Exception}", ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<Article?> GetArticleByArticleSupplierReferenceKitBornes(string supplierReference, string productReference)
        {
            Log.Information("GetArticleByArticleSupplierReference called with refFournisseur: {RefFournisseur}", supplierReference);

            try
            {
                // Récupération de la liste des articles correspondant au supplierReference
                var articleList = await _context.Articles
                    .Where(item => item.SupplierReference == supplierReference)
                    .ToListAsync();

                // Récupération de la Bill Of Materials (bom) correspondant au productReference
                var bom = await _context.BillOfMaterials
                    .Where(row => row.Product == productReference)
                    .ToListAsync();

                // Recherche d'un article de articleList contenu dans bom
                var matchingArticle = articleList
                    .FirstOrDefault(article => bom.Any(b => b.Component == article.ArticleUid)); // Modifier ComponentCode et ArticleCode selon votre modèle

                if (matchingArticle == null)
                {
                    Log.Warning("No matching article found in BOM for ReferenceFournisseur: {RefFournisseur}", supplierReference);
                }
                else
                {
                    Log.Information("Matching article found in BOM for ReferenceFournisseur: {RefFournisseur}", supplierReference);
                }

                return matchingArticle;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get article by ReferenceFournisseur: {RefFournisseur}. Exception: {Exception}", supplierReference, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

    }
}
