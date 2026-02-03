using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.FirmwareRepo
{
    public class FirmwareRepository : IFirmwareRepository
    {
        private readonly AppDbContext _context;
        // Active sentinel must be exactly9999-12-3123:59:59.000000 (UTC)
        private static readonly DateTimeOffset ActiveEndDate = new DateTimeOffset(9999,12,31,23,59,59, TimeSpan.Zero);

        public FirmwareRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Firmware>> GetCollectionFirmwareActiveOnes()
        {
            try
            {
                Log.Information("Fetching active firmwares with EndDate sentinel9999-12-3123:59:59.000000 (UTC)");

                var result = await _context.Firmwares
                    .Where(f => f.EndDate == ActiveEndDate)
                    .ToListAsync();

                Log.Information("Successfully fetched {Count} active firmwares.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching active firmwares.");
                throw;
            }
        }

        public async Task<List<Firmware>> GetCollectionFirmwareByKeyword(string keyword)
        {
            try
            {
                Log.Information("Fetching firmwares with Product or Label containing keyword: {Keyword}", keyword);

                var result = await _context.Firmwares
                    .Where(f => f.Product.Contains(keyword) || f.Label.Contains(keyword))
                    .OrderBy(f => f.Product)
                    .ThenBy(f => f.Label)
                    .ToListAsync();

                Log.Information("Successfully fetched {Count} firmwares matching the keyword: {Keyword}", result.Count, keyword);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching firmwares by keyword: {Keyword}", keyword);
                throw;
            }
        }

        public async Task<List<Firmware>> GetCollectionFirmwareByKeywordActiveOnes(string keyword)
        {
            try
            {
                Log.Information("Fetching active firmwares with Product or Label containing keyword: {Keyword}", keyword);

                var result = await _context.Firmwares
                    .Where(f => (f.Product.Contains(keyword) || f.Label.Contains(keyword)) && f.EndDate == ActiveEndDate)
                    .OrderBy(f => f.Product)
                    .ThenBy(f => f.Label)
                    .ToListAsync();

                Log.Information("Successfully fetched {Count} active firmwares matching the keyword: {Keyword}", result.Count, keyword);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching active firmwares by keyword: {Keyword}", keyword);
                throw;
            }
        }

        public async Task<List<Firmware>> GetCollectionFirmwares()
        {
            try
            {
                Log.Information("Fetching all firmwares from the database.");

                var result = await _context.Firmwares
                    .OrderBy(f => f.Product)
                    .ThenBy(f => f.Label)
                    .ToListAsync();

                Log.Information("Successfully fetched {Count} firmwares.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching all firmwares.");
                throw;
            }
        }

        public async Task<Firmware> GetFirmwareByArticleUidByLabelActiveOne(string articleUid, string label)
        {
            Log.Information("Starting method GetFirmwareByArticleUidByLabel. ArticleUid: {ArticleUid}, Label: {Label}", articleUid, label);

            try
            {
                Log.Debug("Querying database for Firmware with Product: {ArticleUid} and Label: {Label}", articleUid, label);

                var result = await _context.Firmwares
                    .Where(f => f.Product == articleUid && f.Label == label && f.EndDate == ActiveEndDate)
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    Log.Warning("No Firmware found for Product: {ArticleUid} and Label: {Label}", articleUid, label);
                    return null;
                }

                Log.Information("Successfully retrieved Firmware. Firmware details: {Firmware}", result.ToString());

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching Firmware for Product: {ArticleUid} and Label: {Label}", articleUid, label);
                throw;
            }
        }

        public async Task<Firmware> IsDoubletProductAndVersion(Firmware firmware)
        {
            try
            {
                // Return null if the File field is empty or null
                if (string.IsNullOrWhiteSpace(firmware.File))
                    return null;

                var firmExists = await _context.Firmwares
                    .Where(firm => firm.Product == firmware.Product &&
                                  ((firm.File == firmware.File && !string.IsNullOrWhiteSpace(firmware.File)) || firm.Version == firmware.Version))
                    .FirstOrDefaultAsync();

                return firmExists;
            }
            catch (Exception ex)
            {
                Log.Error("Something went wrong with IsDoubletProductAndVersion", ex);
                throw;
            }
        }


        public async Task<Firmware> LinkFirmwareFileUpdate(Firmware firmware)
        {
            try
            {
                var oldFirmware = _context.Firmwares.FirstOrDefault(firm => firm.FirmwareUid == firmware.FirmwareUid);
                if (oldFirmware == null) return null;
                oldFirmware.File = firmware.File;
                _context.Entry(oldFirmware).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return oldFirmware;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new InvalidOperationException("An error occurred while updating the firmware file.", ex);
            }
        }

        public async Task<Firmware> SuspendFirmwareTemporary(Firmware firmware)
        {
            try
            {
                var oldFirmware = _context.Firmwares.FirstOrDefault(firm => firm.FirmwareUid == firmware.FirmwareUid);
                if (oldFirmware == null) return null;
                oldFirmware.IsSuspended = true;
                _context.Entry(oldFirmware).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return oldFirmware;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new InvalidOperationException("An error occurred while updating the firmware file.", ex);
            }
        }

        public async Task<Firmware> UnsuspendFirmwareTemporary(Firmware firmware)
        {
            try
            {
                var oldFirmware = _context.Firmwares.FirstOrDefault(firm => firm.FirmwareUid == firmware.FirmwareUid);
                if (oldFirmware == null) return null;
                oldFirmware.IsSuspended = false;
                _context.Entry(oldFirmware).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return oldFirmware;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new InvalidOperationException("An error occurred while updating the firmware file.", ex);
            }
        }

        public async Task<Firmware> UpdateFirmwareAnticipatedMode(Firmware firmware)
        {
            try
            {
                var oldFirm = _context.Firmwares.FirstOrDefault(firm => firm.FirmwareUid == firmware.FirmwareUid);
                if (oldFirm == null) return null;

                bool hasValue1Changed = oldFirm.File != firmware.File;
                bool hasValue2Changed = oldFirm.Version != firmware.Version;

                if (hasValue1Changed || hasValue2Changed)
                {
                    oldFirm.EndDate = new DateTimeOffset(DateTime.Now.Date.AddDays(0).AddHours(23).AddMinutes(59).AddSeconds(59), DateTimeOffset.Now.Offset);

                    // Mark entity as modified
                    _context.Entry(oldFirm).State = EntityState.Modified;

                    // Create a new record with the new values
                    var newFirmware = new Firmware
                    {
                        FirmwareUid = Guid.NewGuid().ToString(),
                        Product = firmware.Product,
                        Label = firmware.Label,
                        File = firmware.File,
                        Version = firmware.Version,
                        UserUid = firmware.UserUid,
                        StartDate = firmware.StartDate,
                        EndDate = ActiveEndDate,
                        IsSuspended = firmware.IsSuspended,
                    };

                    _context.Firmwares.Add(newFirmware);
                    await _context.SaveChangesAsync();
                    return newFirmware;

                }

                return firmware;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new InvalidOperationException("An error occurred while updating the tolerance.", ex);
            }
        }

        public async Task<Firmware> UpdateFirmwareNominalMode(Firmware firmware)
        {
            try
            {
                var oldFirm = _context.Firmwares.FirstOrDefault(firm => firm.FirmwareUid == firmware.FirmwareUid);
                if (oldFirm == null) return null;

                bool hasValue1Changed = oldFirm.File != firmware.File;
                bool hasValue2Changed = oldFirm.Version != firmware.Version;

                if (hasValue1Changed || hasValue2Changed)
                {
                    oldFirm.EndDate = DateTimeOffset.Now;

                    // Mark entity as modified
                    _context.Entry(oldFirm).State = EntityState.Modified;

                    // Create a new record with the new values
                    var newFirmware = new Firmware
                    {
                        FirmwareUid = Guid.NewGuid().ToString(),
                        Product = firmware.Product,
                        Label = firmware.Label,
                        File = firmware.File,
                        Version = firmware.Version,
                        UserUid = firmware.UserUid,
                        StartDate = DateTimeOffset.Now,
                        EndDate = ActiveEndDate,
                        IsSuspended = firmware.IsSuspended,
                    };

                    _context.Firmwares.Add(newFirmware);
                    await _context.SaveChangesAsync();
                    return newFirmware;

                }

                return firmware;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new InvalidOperationException("An error occurred while updating the tolerance.", ex);
            }
        }
    }
}
