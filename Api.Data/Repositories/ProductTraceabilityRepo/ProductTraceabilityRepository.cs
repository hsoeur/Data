using Api.Data.Dtos;
using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;

namespace Api.Data.Repositories.ProductTraceabilityRepo
{
    public class ProductTraceabilityRepository : IProductTraceabilityRepository
    {
        private readonly AppDbContext _context;

        public ProductTraceabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public void CreateProductTraceability(ProductTraceability tracabiliteProduit)
        {
            Log.Information("CreateProductTraceability");

            try
            {
                _context.ProductTraceabilities.Add(tracabiliteProduit);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException, "Error in CreateProductTraceability");
                throw;
            }

        }

        public async Task<List<ProductTraceability>> GetCollectionProductTraceability(string manufacturingOrderUid, string articleUid)
        {
            Log.Information("GetCollectionProductTraceability called with manufacturingOrderUid : {manufacturingOrderUid} and articleUid : {articleUid}", manufacturingOrderUid, articleUid);
            try
            {
                if (!int.TryParse(manufacturingOrderUid, out var moId))
                {
                    Log.Warning("GetCollectionProductTraceability: manufacturingOrderUid is not a valid integer: {manufacturingOrderUid}", manufacturingOrderUid);
                    return new List<ProductTraceability>();
                }

                var traceabilityList = await _context.ProductTraceabilities
                    .Where(pt => pt.ManufacturingOrderUid == moId && pt.ProductUid == articleUid)
                    .ToListAsync();

                return traceabilityList;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetCollectionProductTraceability");
                throw;
            }
        }

        public async Task<ProductTraceability> GetLatestProductTraceabilityWithArticleUid(string articleUid)
        {
            Log.Information("GetLatestProductTraceabilityWithArticleUid called with article uid : {articleuid}", articleUid);
            try
            {
                var pt = await _context.ProductTraceabilities
                    .Where(pt => pt.ProductUid == articleUid)
                    .OrderByDescending(pt => pt.ProductSerialNumber)
                    .FirstOrDefaultAsync();

                if (pt == null)
                {
                    ProductTraceability ptNull = new ProductTraceability()
                    {
                        ProductUid = articleUid,
                        ProductSerialNumber =100000,

                    };
                    return ptNull;
                }

                return pt;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetLatestProductTraceabilityWithArticleUid");
                throw;
            }
        }

        public async Task<ProductTraceability?> GetProductTraceabilityByProductBySerialNumberByOperationLatestOne(string product, string serialNumber, string operationUid)
        {
            ProductTraceability? latestProductTraceability = null;

            try
            {
                Log.Information("Début de l'exécution de GetLatestProductTraceabilityByProductBySerialNumberByOperation.");
                Log.Information("Paramètres reçus : Product = {Product}, SerialNumber = {SerialNumber}, OperationUid = {OperationUid}", product, serialNumber, operationUid);

                if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(serialNumber) || string.IsNullOrWhiteSpace(operationUid))
                {
                    Log.Warning("Les paramètres de la fonction sont invalides : l'un des UIDs est vide ou nul.");
                    return null;
                }

                if (!int.TryParse(serialNumber, out var serialNum))
                {
                    Log.Warning("SerialNumber n'est pas un entier valide: {SerialNumber}", serialNumber);
                    return null;
                }
                if (!int.TryParse(operationUid, out var operationNumber))
                {
                    Log.Warning("OperationUid n'est pas un entier valide: {OperationUid}", operationUid);
                    return null;
                }

                latestProductTraceability = await _context.ProductTraceabilities
                    .Where(p => p.ProductUid == product && p.ProductSerialNumber == serialNum && p.OperationNumber == operationNumber)
                    .OrderByDescending(p => p.CreationDate)
                    .FirstOrDefaultAsync();

                if (latestProductTraceability != null)
                {
                    Log.Information("Enregistrement trouvé : ProductTraceabilityUid = {Uid}", latestProductTraceability.ProductTraceabilityUid);
                }
                else
                {
                    Log.Information("Aucun enregistrement correspondant trouvé.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur survenue dans GetLatestProductTraceabilityByProductBySerialNumberByOperation.");
                throw;
            }

            Log.Information("Fin de l'exécution de GetLatestProductTraceabilityByProductBySerialNumberByOperation.");
            return latestProductTraceability;
        }


        public async Task<List<ProductTraceability>> GetCollectionProductTraceabilityByManufacturingOrderUidByOperationUid(string manufacturingOrderUid, string operationUid)
        {
            List<ProductTraceability> productTraceabilities = new List<ProductTraceability>();

            try
            {
                Log.Information("Début de l'exécution de GetLatestProductTraceabilityByManufacturingOrder.");
                Log.Information("Paramètres reçus : ManufacturingOrderUid = {manufacturingOrderUid}, manufacturingOrderUid = {manufacturingOrderUid}", manufacturingOrderUid);
                Log.Information("Paramètres reçus : OperationUid = {ManufacturingOrderUid}, operationUid = {operationUid}", operationUid);

                // Validation des paramètres
                if (string.IsNullOrWhiteSpace(manufacturingOrderUid))
                {
                    Log.Warning("Les paramètres de la fonction sont invalides : l'un des UIDs est vide ou nul.");
                    return productTraceabilities;
                }

                if (!int.TryParse(manufacturingOrderUid, out var moId))
                {
                    Log.Warning("ManufacturingOrderUid n'est pas un entier valide: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return productTraceabilities;
                }
                if (!int.TryParse(operationUid, out var operationNumber))
                {
                    Log.Warning("OperationUid n'est pas un entier valide: {OperationUid}", operationUid);
                    return productTraceabilities;
                }

                productTraceabilities = await _context.ProductTraceabilities
                    .Where(p => p.ManufacturingOrderUid == moId)
                    .Where(p => p.OperationNumber == operationNumber)
                    .OrderBy(p => p.ProductSerialNumber)
                    .ToListAsync();

                Log.Information("Nombre d'enregistrements trouvés : {Count}", productTraceabilities.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur survenue dans GetLatestProductTraceabilityByManufacturingOrder.");
                throw;
            }

            Log.Information("Fin de l'exécution de GetLatestProductTraceabilityByManufacturingOrder.");
            return productTraceabilities;
        }

        public async Task<List<ProductTraceability>> GetCollectionProductTraceabilityByManufacturingOrderUidByOperationLatestOne(string manufacturingOrderUid, string operationUid)
        {
            List<ProductTraceability> productTraceabilities = new List<ProductTraceability>();

            try
            {
                Log.Information("Début de l'exécution de GetLatestProductTraceabilityByManufacturingOrder.");
                Log.Information("Paramètres reçus : ManufacturingOrderUid = {ManufacturingOrderUid}", manufacturingOrderUid);
                Log.Information("Paramètres reçus : OperationUid = {operationUid}", operationUid);

                // Validation des paramètres
                if (string.IsNullOrWhiteSpace(manufacturingOrderUid))
                {
                    Log.Warning("Les paramètres de la fonction sont invalides : ManufacturingOrderUid est vide ou nul.");
                    return productTraceabilities;
                }

                if (!int.TryParse(manufacturingOrderUid, out var moId))
                {
                    Log.Warning("ManufacturingOrderUid n'est pas un entier valide: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return productTraceabilities;
                }
                if (!int.TryParse(operationUid, out var operationNumber))
                {
                    Log.Warning("OperationUid n'est pas un entier valide: {OperationUid}", operationUid);
                    return productTraceabilities;
                }

                // Récupération des données brutes
                var rawData = await _context.ProductTraceabilities
                    .Where(p => p.ManufacturingOrderUid == moId)
                    .Where(P => P.OperationNumber == operationNumber)
                    .ToListAsync();

                // Groupement et tri côté client
                productTraceabilities = rawData
                    .GroupBy(p => new { p.ManufacturingOrderUid, p.ProductSerialNumber })
                    .Select(g => g.OrderByDescending(p => p.CreationDate)
                                   .FirstOrDefault())
                    .ToList();

                Log.Information("Nombre d'enregistrements trouvés : {Count}", productTraceabilities.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur survenue dans GetLatestProductTraceabilityByManufacturingOrder.");
                throw;
            }

            Log.Information("Fin de l'exécution de GetLatestProductTraceabilityByManufacturingOrder.");
            return productTraceabilities;
        }

        public async Task<ProductTraceabilityStatsDto> GetTraceabilityStatsByManufacturingOrderUidAndOperationUid(string manufacturingOrderUid, string operationUid)
        {
            var stats = new ProductTraceabilityStatsDto();

            try
            {
                Log.Information("Début de GetTraceabilityStatsByManufacturingOrderUidAndOperationUid.");
                Log.Information("Paramètres reçus : ManufacturingOrderUid = {ManufacturingOrderUid}, OperationUid = {OperationUid}", manufacturingOrderUid, operationUid);

                if (string.IsNullOrWhiteSpace(manufacturingOrderUid) || string.IsNullOrWhiteSpace(operationUid))
                {
                    Log.Warning("Paramètres invalides.");
                    return stats;
                }

                if (!int.TryParse(manufacturingOrderUid, out var moId))
                {
                    Log.Warning("ManufacturingOrderUid n'est pas un entier valide: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return stats;
                }
                if (!int.TryParse(operationUid, out var operationNumber))
                {
                    Log.Warning("OperationUid n'est pas un entier valide: {OperationUid}", operationUid);
                    return stats;
                }

                stats.OK = await _context.ProductTraceabilities
                    .CountAsync(p => p.ManufacturingOrderUid == moId &&
                                     p.OperationNumber == operationNumber &&
                                     p.ProductStatusUid == "OK");

                stats.NOK = await _context.ProductTraceabilities
                    .CountAsync(p => p.ManufacturingOrderUid == moId &&
                                     p.OperationNumber == operationNumber &&
                                     p.ProductStatusUid == "NOK");

                stats.Total = stats.OK + stats.NOK;
                stats.PercentageOK = stats.Total >0 ? Math.Round((double)stats.OK / stats.Total *100,2) :0.0;

                Log.Information("Stats : OK = {OK}, NOK = {NOK}, Total = {Total}, %OK = {Percentage}%", stats.OK, stats.NOK, stats.Total, stats.PercentageOK);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur dans GetTraceabilityStatsByManufacturingOrderUidAndOperationUid");
                throw;
            }

            return stats;
        }

        public async Task<ProductTraceabilityStatsDto> GetTraceabilityStatsByManufacturingOrderUidAndOperationUidToday(string manufacturingOrderUid, string operationUid)
        {
            var stats = new ProductTraceabilityStatsDto();

            try
            {
                Log.Information("Début de GetTraceabilityStatsByManufacturingOrderUidAndOperationUidToday.");
                Log.Information("Paramètres reçus : ManufacturingOrderUid = {ManufacturingOrderUid}, OperationUid = {OperationUid}", manufacturingOrderUid, operationUid);

                if (string.IsNullOrWhiteSpace(manufacturingOrderUid) || string.IsNullOrWhiteSpace(operationUid))
                {
                    Log.Warning("Paramètres invalides.");
                    return stats;
                }

                if (!int.TryParse(manufacturingOrderUid, out var moId))
                {
                    Log.Warning("ManufacturingOrderUid n'est pas un entier valide: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return stats;
                }
                if (!int.TryParse(operationUid, out var operationNumber))
                {
                    Log.Warning("OperationUid n'est pas un entier valide: {OperationUid}", operationUid);
                    return stats;
                }

                var today = DateTime.Today;

                var listToday = (await _context.ProductTraceabilities
                     .Where(p => p.ManufacturingOrderUid == moId &&
                     p.OperationNumber == operationNumber)
                     .ToListAsync())
                     .Where(p => p.CreationDate.Date == today)
                     .ToList();

                stats.OK = listToday.Count(p => p.ProductStatusUid == "OK");
                stats.NOK = listToday.Count(p => p.ProductStatusUid == "NOK");
                stats.Total = stats.OK + stats.NOK;
                stats.PercentageOK = stats.Total >0 ? Math.Round((double)stats.OK / stats.Total *100,2) :0.0;

                Log.Information("Stats (Today) : OK = {OK}, NOK = {NOK}, Total = {Total}, %OK = {Percentage}%", stats.OK, stats.NOK, stats.Total, stats.PercentageOK);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur dans GetTraceabilityStatsByManufacturingOrderUidAndOperationUidToday");
                throw;
            }

            return stats;
        }


        public async Task<ProductTraceability?> GetProductTraceabilityByManufacturingOrderProductAndSerialNumber(
            string manufacturingOrderUid,
            string productUid,
            string serialNumber)
        {
            try
            {
                Log.Information("Début de GetProductTraceabilityByManufacturingOrderProductAndSerialNumber.");
                Log.Information("Paramètres reçus : ManufacturingOrderUid = {ManufacturingOrderUid}, ProductUid = {ProductUid}, SerialNumber = {SerialNumber}",
                    manufacturingOrderUid, productUid, serialNumber);

                if (string.IsNullOrWhiteSpace(manufacturingOrderUid) ||
                    string.IsNullOrWhiteSpace(productUid) ||
                    string.IsNullOrWhiteSpace(serialNumber))
                {
                    Log.Warning("Les paramètres sont invalides (vide ou null).");
                    return null;
                }

                if (!int.TryParse(manufacturingOrderUid, out var moId))
                {
                    Log.Warning("ManufacturingOrderUid n'est pas un entier valide: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return null;
                }
                if (!int.TryParse(serialNumber, out var serialNum))
                {
                    Log.Warning("SerialNumber n'est pas un entier valide: {SerialNumber}", serialNumber);
                    return null;
                }

                var productTraceability = await _context.ProductTraceabilities
                    .FirstOrDefaultAsync(p =>
                        p.ManufacturingOrderUid == moId &&
                        p.ProductUid == productUid &&
                        p.ProductSerialNumber == serialNum);

                if (productTraceability != null)
                {
                    Log.Information("Enregistrement trouvé : ProductTraceabilityUid = {Uid}", productTraceability.ProductTraceabilityUid);
                }
                else
                {
                    Log.Information("Aucun enregistrement correspondant trouvé.");
                }

                return productTraceability;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur dans GetProductTraceabilityByManufacturingOrderProductAndSerialNumber.");
                throw;
            }
            finally
            {
                Log.Information("Fin de GetProductTraceabilityByManufacturingOrderProductAndSerialNumber.");
            }
        }

        public async Task<ProductTraceability?> GetProductTraceabilityWithHighestSerialNumberByManufacturingOrderUid(string manufacturingOrderUid)
        {
            try
            {
                Log.Information("Début de GetProductTraceabilityWithHighestSerialNumberByManufacturingOrderUid.");
                Log.Information("Paramètre reçu : ManufacturingOrderUid = {ManufacturingOrderUid}", manufacturingOrderUid);

                if (string.IsNullOrWhiteSpace(manufacturingOrderUid))
                {
                    Log.Warning("Le ManufacturingOrderUid est vide ou nul.");
                    return null;
                }

                if (!int.TryParse(manufacturingOrderUid, out var moId))
                {
                    Log.Warning("ManufacturingOrderUid n'est pas un entier valide: {ManufacturingOrderUid}", manufacturingOrderUid);
                    return null;
                }

                var record = await _context.ProductTraceabilities
                    .Where(p => p.ManufacturingOrderUid == moId)
                    .OrderByDescending(p => p.ProductSerialNumber)
                    .FirstOrDefaultAsync();

                if (record != null)
                {
                    Log.Information("Enregistrement trouvé : ProductTraceabilityUid = {Uid}, SerialNumber = {SerialNumber}",
                        record.ProductTraceabilityUid, record.ProductSerialNumber);
                }
                else
                {
                    Log.Information("Aucun enregistrement trouvé pour ManufacturingOrderUid = {ManufacturingOrderUid}", manufacturingOrderUid);
                }

                return record;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur dans GetProductTraceabilityWithHighestSerialNumberByManufacturingOrderUid.");
                throw;
            }
        }
    }
}
