using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization; // if needed in future

namespace Api.Data.Repositories.LabelTemplateRepo
{
    public class LabelTemplateRepository : ILabelTemplateRepository
    {
        private readonly AppDbContext _context;

        public LabelTemplateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<LabelTemplate>> GetCollectionLabelTemplateByKeywordSearch(string keyword)
        {
            Log.Information("GetCollectionUcpsByKeyword called with keyword: {Keyword}", keyword);

            try
            {
                var templates = _context.LabelTemplates
                    .AsEnumerable()
                    .Where(ucp => ucp.Product.Contains(keyword) || ucp.LabelTypeUid.Contains(keyword) || ucp.TemplateFile.Contains(keyword))
                    .OrderBy(u => u.Product)
                    .ThenBy(u => u.LabelTypeUid)
                    .ThenByDescending(tol => tol.EndDate)
                    .ToList();

                return templates;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get etiquettes by keyword: {Keyword}. Exception: {Exception}", keyword, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }


        public async Task CreateLabelTemplate(LabelTemplate usageCorrespondanceProduit)
        {
            Log.Information("CreateUsageCorrespondanceProduitAsync called with usageCorrespondanceProduit: {@UsageCorrespondanceProduit}", usageCorrespondanceProduit);

            try
            {
                var produitExists = await _context.Articles.AnyAsync(art => art.ArticleUid == usageCorrespondanceProduit.Product);
                if (!produitExists)
                {
                    Log.Warning("Produit avec UidArticle {UidArticle} n'existe pas. Création échouée.", usageCorrespondanceProduit.Product);
                    throw new InvalidOperationException($"Produit avec UidArticle {usageCorrespondanceProduit.Product} n'existe pas.");
                }

                var ucpExists = await _context.LabelTemplates.AnyAsync(ucp => ucp.Product == usageCorrespondanceProduit.Product && ucp.LabelTypeUid == usageCorrespondanceProduit.LabelTypeUid);
                if (ucpExists)
                {
                    Log.Warning("Template existe déjà pour produit {UidArticle} et usage {Usage}.", usageCorrespondanceProduit.Product, usageCorrespondanceProduit.LabelTypeUid);
                    throw new InvalidOperationException($"Template avec UidArticle {usageCorrespondanceProduit.Product} et {usageCorrespondanceProduit.LabelTypeUid} existe déjà dans la base, modifier le template existant si vous voulez changer le fichier template associé.");
                }

                _context.LabelTemplates.Add(usageCorrespondanceProduit);
                await _context.SaveChangesAsync();
                Log.Information("Successfully created usage correspondence produit: {@UsageCorrespondanceProduit}", usageCorrespondanceProduit);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create usage correspondence produit for usageCorrespondanceProduit: {@UsageCorrespondanceProduit}. Exception: {Exception}", usageCorrespondanceProduit, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<LabelTemplate?> GetLabelTemplateByArticleUidAndUsageUid(string uidProduit, string usage)
        {
            Log.Information("GetUsageCorrespondanceProduitByUidProduitUidUsageAsync called with uidProduit: {UidProduit} and usage: {Usage}", uidProduit, usage);

            try
            {
                var usageCorrespondanceProduit = await _context.LabelTemplates
                    .FirstOrDefaultAsync(ucp => ucp.Product == uidProduit && ucp.LabelTypeUid == usage);

                if (usageCorrespondanceProduit == null)
                {
                    Log.Warning("No usage correspondence produit found for uidProduit: {UidProduit} and usage: {Usage}", uidProduit, usage);
                }
                else
                {
                    Log.Information("Retrieved usage correspondence produit for uidProduit: {UidProduit} and usage: {Usage}", uidProduit, usage);
                }

                return usageCorrespondanceProduit;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve usage correspondence produit for uidProduit: {UidProduit} and usage: {Usage}. Exception: {Exception}", uidProduit, usage, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<List<LabelTemplate>> GetCollectionLabelTemplateByArticleUid(string uidProduit)
        {
            Log.Information("GetUsageCorrespondanceProduitsListByUidProduitAsync called with uidProduit: {UidProduit}", uidProduit);

            try
            {
                var currentDate = DateTimeOffset.Now;

                var usageCorrespondanceProduits = await _context.LabelTemplates
                    .Where(ucp => ucp.Product == uidProduit)
                    .ToListAsync();

                var filteredTemplates = usageCorrespondanceProduits
                    .Where(t => t.EndDate > currentDate)
                    .ToList();

                Log.Information("Retrieved {Count} valid templates for uidProduit: {UidProduit}", filteredTemplates.Count, uidProduit);

                return filteredTemplates;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve templates for uidProduit: {UidProduit}. Exception: {Exception}", uidProduit, ex);
                throw;
            }
        }


        public async Task<List<LabelTemplate>> GetCollectionLabelTemplates()
        {
            Log.Information("GetCollectionTemplates called");

            try
            {
                var templates = _context.LabelTemplates
                    .AsEnumerable()
                    .OrderBy(u => u.Product)
                    .ThenBy(u => u.LabelTypeUid)
                    .ThenByDescending(tol => tol.EndDate)
                    .ToList();

                return templates;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve usage correspondence produits. Exception: {Exception}", ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }


        public async Task<List<LabelTemplate>> GetCollectionLabelTemplateActiveOnes()
        {
            Log.Information("GetCollectionUcpOnlyActiveOnes called:");

            try
            {
                var today = DateTimeOffset.Now;

                var allUsageCorrespondanceProduits = await _context.LabelTemplates.ToListAsync();

                var usageCorrespondanceProduits = allUsageCorrespondanceProduits
                    .Where(u => u.EndDate > today)
                    .OrderBy(u => u.Product)
                    .ThenBy(u => u.LabelTypeUid)
                    .ThenBy(u => u.TemplateFile)
                    .ToList();

                Log.Information("Retrieved {Count} usage correspondence produits", usageCorrespondanceProduits.Count);
                return usageCorrespondanceProduits;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve usage correspondence produits. Exception: {Exception}", ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task UpdateLabelTemplate(LabelTemplate usageCorrespondanceProduit)
        {
            try
            {
                var oldUcp = _context.LabelTemplates.FirstOrDefault(ucp => ucp.LabelTemplateUid == usageCorrespondanceProduit.LabelTemplateUid);
                if (oldUcp == null)
                {
                    throw new InvalidOperationException("Template introuvable");
                }

                bool hasTemplateChanged = oldUcp.TemplateFile != usageCorrespondanceProduit.TemplateFile;
                bool hasPhotoChanged = oldUcp.Photo != usageCorrespondanceProduit.Photo;

                if (hasTemplateChanged)
                {
                    oldUcp.EndDate = DateTimeOffset.Now;
                    _context.Entry(oldUcp).State = EntityState.Modified;

                    var nowPlusOneSecond = DateTimeOffset.Now.AddSeconds(1);

                    var newLabelTemplate = new LabelTemplate
                    {
                        LabelTemplateUid = Guid.NewGuid().ToString(),
                        Product = usageCorrespondanceProduit.Product,
                        LabelTypeUid = usageCorrespondanceProduit.LabelTypeUid,
                        Photo = usageCorrespondanceProduit.Photo,
                        TemplateFile = usageCorrespondanceProduit.TemplateFile,
                        StartDate = nowPlusOneSecond,
                        EndDate = DateTimeOffset.Parse("9999-12-31T23:59:59Z"),
                        UserUid = usageCorrespondanceProduit.UserUid
                    };

                    _context.LabelTemplates.Add(newLabelTemplate);
                    await _context.SaveChangesAsync();
                }
                if (hasPhotoChanged && !hasTemplateChanged)
                {
                    oldUcp.Photo = usageCorrespondanceProduit.Photo;
                    _context.Entry(oldUcp).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new InvalidOperationException("An error occurred while updating the usage correspondence product.", ex);
            }

        }


        public async Task<LabelTemplate> GetLabelTemplateByTemplateUid(string uIDUsageCorrespondanceProduit)
        {
            try
            {
                var usageCorrespondanceProduit = await _context.LabelTemplates
                    .FirstOrDefaultAsync(u => u.LabelTemplateUid == uIDUsageCorrespondanceProduit);

                return usageCorrespondanceProduit;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching UsageCorrespondanceProduit with ID: {Id}", uIDUsageCorrespondanceProduit);
                throw new InvalidOperationException("An error occurred while fetching the usage correspondence product.", ex);
            }
        }

        public async Task<List<LabelTemplate>> GetCollectionLabelTemplateByKeywordSearchOnAcitveOnes(string keyword)
        {
            Log.Information("GetCollectionUcpsByKeywordActiveOnes called with keyword: {Keyword}", keyword);

            try
            {
                var today = DateTimeOffset.Now;
                var etiquettes = await _context.LabelTemplates
                    .Where(ucp => ucp.Product.Contains(keyword) || ucp.LabelTypeUid.Contains(keyword) || ucp.TemplateFile.Contains(keyword))
                    .OrderBy(u => u.Product)
                    .ThenBy(u => u.LabelTypeUid)
                    .ToListAsync();

                var activesTolerancesKeyword = etiquettes
                    .Where(tol => tol.EndDate > today)
                    .ToList();

                Log.Information("Retrieved {Count} ucps by keyword: {Keyword}", etiquettes.Count, keyword);

                return activesTolerancesKeyword;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get etiquettes by keyword: {Keyword}. Exception: {Exception}", keyword, ex);
                throw;
            }
        }
    }
}
