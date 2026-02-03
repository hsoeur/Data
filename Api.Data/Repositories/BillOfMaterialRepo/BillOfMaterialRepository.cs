using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.BillOfMaterialRepo
{
    public class BillOfMaterialRepository : IBillOfMaterialRepository
    {
        private readonly AppDbContext _context;

        public BillOfMaterialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<BillOfMaterial>> GetBillOfMaterialsByManufacturingOrderUid(int manufacturingOrderUid)
        {
            Log.Information("GetNomenclatureByOFUidAsync called with uidOF: {UidOF}", manufacturingOrderUid);

            try
            {
                var uidProduit = await _context.ManufacturingOrders
                    .Where(of => of.ManufacturingOrderUid == manufacturingOrderUid)
                    .Select(of => of.ProductUid)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(uidProduit))
                {
                    Log.Warning("No UidProduit found for uidOF: {UidOF}", manufacturingOrderUid);
                    return new List<BillOfMaterial>();
                }

                var nomenclatures = await _context.BillOfMaterials
                    .Where(row => row.Product == uidProduit)
                    .ToListAsync();

                Log.Information("Nomenclature retrieved for uidOF: {UidOF}. Count: {Count}", manufacturingOrderUid, nomenclatures.Count);

                return nomenclatures;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve nomenclature for uidOF: {UidOF}. Exception: {ExceptionMessage}", manufacturingOrderUid, ex.Message);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<ICollection<BillOfMaterial>> GetBillOfMaterialsWithoutLabelsByManufacturingOrderUid(int manufacturingOrderUid)
        {
            Log.Information("GetNomenclatureByOFUidWithoutLabelsAsync called with uidOF: {UidOF}", manufacturingOrderUid);

            try
            {
                var uidProduit = await _context.ManufacturingOrders
                    .Where(of => of.ManufacturingOrderUid == manufacturingOrderUid)
                    .Select(of => of.ProductUid)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(uidProduit))
                {
                    Log.Warning("No UidProduit found for uidOF: {UidOF}", manufacturingOrderUid);
                    return new List<BillOfMaterial>();
                }

                var nomenclatures = await _context.BillOfMaterials
                    .Where(row => row.Product == uidProduit)
                    .Where(row => !row.Component.Contains("05EE"))
                    .Where(row => !row.Component.Contains("05EC"))
                    .ToListAsync();

                Log.Information("Nomenclature without labels retrieved for uidOF: {UidOF}. Count: {Count}", manufacturingOrderUid, nomenclatures.Count);

                return nomenclatures;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve nomenclature without labels for uidOF: {UidOF}. Exception: {ExceptionMessage}", manufacturingOrderUid, ex.Message);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<ICollection<BillOfMaterial>> GetBillOfMaterialsLabelsByArticleUid(string produit)
        {

            Log.Information("GetNomenclatureByProduit called with produit: {produit}", produit);

            try
            {
                var nomenclatures = await _context.BillOfMaterials
                   .Where(row => row.Product == produit)
                   .Where(row => row.Component.Contains("05EE"))
                   .ToListAsync();

                return nomenclatures;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve nomenclature without labels for uidOF: {UidOF}. Exception: {ExceptionMessage}", produit, ex.Message);
                throw; // Rethrow the exception to be handled upstream
            }

        }

        public async Task<ICollection<Article>> GetQuantitiesForArticlesInBillOfMaterialsByArticleUids()
        {
            // Liste des UIDs d'articles en dur pour le test
            var articleUids = new List<string>
            {
                "71PT00572", "71PT00573", "71PT00574", "71PT00575", "71PT00593", "71PT0095", "61DM00033", "61DM00034", "61DM00035", "61DM00036", "61DM00037", "61DM00038"
            };

            // Ajouter la plage 71PT00602 à 71PT00629
            articleUids.AddRange(Enumerable.Range(602, 28).Select(i => $"71PT00{i}"));

            // Ajouter la plage 02VK00001 à 02VK00019
            articleUids.AddRange(Enumerable.Range(1, 19).Select(i => $"02VK{i.ToString("D5")}"));


            // Liste de toutes les nomenclatures pour les articles donnés
            var nomenclatures = await _context.BillOfMaterials
                                              .Where(row => articleUids.Contains(row.Product))
                                              .ToListAsync();

            // Extraire la liste des UIDs de composants depuis les nomenclatures
            var componentUids = nomenclatures.Select(n => n.Component).Distinct().ToList();

            // Récupérer les articles correspondant aux UIDs de composants sans doublon par ArticleUid
            var articles = await _context.Articles
                                         .Where(article => componentUids.Contains(article.ArticleUid))
                                         .GroupBy(article => article.ArticleUid)
                                         .Select(group => group.First()) // Prendre le premier élément de chaque groupe
                                         .ToListAsync();

            return articles;
        }

        public async Task<BillOfMaterial> GetBillOfMaterialByArticleUidAndbyComponentUid(string articleUid, string componentUid)
        {
            Log.Information("GetBillOfMaterialByArticleUidAndbyComponentUid called with articleUid: {articleUid} and {componentUid}", articleUid, componentUid);

            try
            {
                var nomenclature = await _context.BillOfMaterials
                   .Where(row => row.Product == articleUid)
                   .Where(row => row.Component == componentUid)
                   .FirstOrDefaultAsync();

                return nomenclature;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve nomenclature without labels for articleUid: {articleUid}. Exception: {ExceptionMessage}", articleUid, ex.Message);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<bool> IsArticleInBillOfMaterials(string article, string product)
        {
            var bom = await _context.BillOfMaterials
                .Where(row => row.Product == product)
                .Where(row => row.Component == article)
                .FirstOrDefaultAsync();

            return bom != null;
        }
    }
}

