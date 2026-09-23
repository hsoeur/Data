using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq;

namespace Api.Data.Repositories.LabelRepo
{

    public class LabelRepository : ILabelRepository
    {
        private readonly AppDbContext _context;

        public LabelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Label>> GetCollectionLabelsByManufacturingOrderUid(int manufacturingOrderUid)
        {
            Log.Information("GetEtiquettesByUidOF called with uidOF: {UidOF}", manufacturingOrderUid);

            try
            {
                var etiquettes = await _context.Labels
                    .Where(e => e.ManufacturingOrderUid == manufacturingOrderUid)
                    .ToListAsync();

                Log.Information("Retrieved {Count} etiquettes for uidOF: {UidOF}", etiquettes.Count, manufacturingOrderUid);

                return etiquettes;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get etiquettes by uidOF: {UidOF}. Exception: {Exception}", manufacturingOrderUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<Label> GetLabelByAuthentiUid(string authentiUid)
        {
            Log.Information("GetEtiquetteByUidAuthenti called with uidAuthenti: {UidAuthenti}", authentiUid);

            try
            {
                var etiquette = await _context.Labels
                    .FirstOrDefaultAsync(e => e.AuthentiUid == authentiUid);

                if (etiquette == null)
                {
                    Log.Warning("No etiquette found with uidAuthenti: {UidAuthenti}", authentiUid);
                }
                else
                {
                    Log.Information("Etiquette found with uidAuthenti: {UidAuthenti}", authentiUid);
                }

                return etiquette;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get etiquette by uidAuthenti: {UidAuthenti}. Exception: {Exception}", authentiUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task CreateLabel(Label label)
        {
            Log.Information("CreateEtiquette called with etiquette: {Etiquette}", label);

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    _context.Labels.Add(label);
                    _context.Database.SetCommandTimeout(600); //10 minutes
                    await _context.SaveChangesAsync();

                    Log.Information("Etiquette created successfully: {Etiquette}", label);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to create etiquette: {Etiquette}. Exception: {Exception}", label, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task CreateLabels(IReadOnlyCollection<Label> labels)
        {
            Log.Information("CreateLabels called with {Count} labels", labels.Count);

            try
            {
                await _context.Labels.AddRangeAsync(labels);
                _context.Database.SetCommandTimeout(600);
                await _context.SaveChangesAsync();

                Log.Information("CreateLabels successfully created {Count} labels", labels.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create {Count} labels", labels.Count);
                throw;
            }
        }

        public async Task<bool> LabelExists(string authentiUid)
        {
            Log.Information("CheckEtiquetteExists called with uidAuthenti: {UidAuthenti}", authentiUid);

            try
            {
                var exists = await _context.Labels.AnyAsync(e => e.AuthentiUid == authentiUid);

                if (exists)
                {
                    Log.Information("Etiquette exists with uidAuthenti: {UidAuthenti}", authentiUid);
                }
                else
                {
                    Log.Warning("No etiquette exists with uidAuthenti: {UidAuthenti}", authentiUid);
                }

                return exists;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to check if etiquette exists with uidAuthenti: {UidAuthenti}. Exception: {Exception}", authentiUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<ICollection<Label>> GetCollectionLabelsMostRecentByManufacturingOrderUid(int manufacturingOrderUid)
        {
            Log.Information("GetMostRecentEtiquettesByUidOF called with uidOF: {UidOF}", manufacturingOrderUid);

            try
            {
                var mostRecentDate = await _context.Labels
                    .Where(e => e.ManufacturingOrderUid == manufacturingOrderUid)
                    .MaxAsync(e => e.CreationDate);

                var mostRecentEtiquettes = await _context.Labels
                    .Where(e => e.ManufacturingOrderUid == manufacturingOrderUid && e.CreationDate == mostRecentDate)
                    .OrderBy(e => e.CurrentSerialNumber)
                    .ToListAsync();

                Log.Information("Retrieved {Count} most recent etiquettes for uidOF: {UidOF} with date: {Date}", mostRecentEtiquettes.Count, manufacturingOrderUid, mostRecentDate);

                return mostRecentEtiquettes;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get most recent etiquettes by uidOF: {UidOF}. Exception: {Exception}", manufacturingOrderUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<bool> LabelPrinted(int manufacturingOrderUid, string usageUid)
        {
            Log.Information("IsPrintedLabel called with uidOf: {UidOf} and uidUsage: {UidUsage}", manufacturingOrderUid, usageUid);
            bool exists = false;
            try
            {
                var labels = await _context.Labels
                    .Where(e => e.ManufacturingOrderUid == manufacturingOrderUid && e.LabelTypeUid == usageUid)
                    .ToListAsync();

                var of = await _context.ManufacturingOrders
                    .Where(of => of.ManufacturingOrderUid == manufacturingOrderUid)
                    .FirstAsync();

                // Quantity is int; compare directly
                int quantity = of.Quantity;
                if (labels.Count == quantity)
                {
                    exists = true;
                }

                return exists;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to check if label is printed with uidOf: {UidOf} and uidUsage: {UidUsage}. Exception: {Exception}", manufacturingOrderUid, usageUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }


        public async Task<ICollection<Label>> GetCollectionLabelsByKeywordSearch(string keyword, int manufacturingOrderUid, string usageUid)
        {
            Log.Information("GetAllEtiquettesByKeyword called with keyword: {Keyword} and of: {Of}", keyword, manufacturingOrderUid);

            try
            {
                var etiquettes = await _context.Labels
                    .Where(eti => eti.ManufacturingOrderUid == manufacturingOrderUid && (eti.CurrentSerialNumber.ToString().Contains(keyword) || eti.AuthentiUid.Contains(keyword)))
                    .Where(eti => eti.LabelTypeUid == usageUid)
                    .OrderBy(eti => eti.CurrentSerialNumber)
                    .ToListAsync();

                Log.Information("Retrieved {Count} etiquettes by keyword: {Keyword} and of: {Of}", etiquettes.Count, keyword, manufacturingOrderUid);

                return etiquettes;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get etiquettes by keyword: {Keyword} and of: {Of}. Exception: {Exception}", keyword, manufacturingOrderUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<List<Label>> GetCollectionLabelsByManufacturingOrderUidAndUsageUid(int manufacturingOrderUid, string usageUid)
        {
            Log.Information("GetCollectionUcpsByOfAndUsage called. With {Of} et {Usage}", manufacturingOrderUid, usageUid);
            try
            {
                var etiquettes = await _context.Labels
                    .Where(eti => eti.LabelTypeUid == usageUid)
                    .Where(eti => eti.ManufacturingOrderUid == manufacturingOrderUid)
                    .OrderBy(eti => eti.CurrentSerialNumber)
                    .ToListAsync();

                return etiquettes;

            }
            catch (Exception ex)
            {
                Log.Error("Failed to get etiquettes by of : {of} et usage : {usage}. Exception: {Exception}", manufacturingOrderUid, usageUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<Label> GetLabelLastSerialNumberPrintedByManufacturingOrderUid(int manufacturingOrderUid)
        {
            Log.Information("Starting retrieval of the last printed label for ManufacturingOrderUid: {uid}", manufacturingOrderUid);

            try
            {
                // Vérification de l'entrée
                // Since int, no need to check string emptiness

                // Récupération et tri des labels
                var label = await _context.Labels
                    .Where(label => label.ManufacturingOrderUid == manufacturingOrderUid)
                    .OrderByDescending(label => label.CurrentSerialNumber)
                    .FirstOrDefaultAsync();

                // Log des résultats
                if (label != null)
                {
                    Log.Information("Successfully retrieved the last printed label with ManufacturingOrderUid: {uid}. CurrentSerialNumber: {serialNumber}",
                        manufacturingOrderUid, label.CurrentSerialNumber);
                }
                else
                {
                    Log.Warning("No labels found for ManufacturingOrderUid: {uid}", manufacturingOrderUid);
                }

                return label;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve the last printed label for ManufacturingOrderUid: {uid}. Exception: {message}",
                    manufacturingOrderUid, ex.Message);
                throw; // Rethrow the exception to be handled upstream
            }
            finally
            {
                Log.Information("Finished retrieval process for ManufacturingOrderUid: {uid}", manufacturingOrderUid);
            }
        }

    }
}
