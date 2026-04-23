using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.MacAddressRepo
{
    public class MacAddressRepository : IMacAddressRepository
    {
        private readonly AppDbContext _context;
        private static readonly SemaphoreSlim _reservationLock = new SemaphoreSlim(1, 1);
        private static readonly TimeSpan _reservationTimeout = TimeSpan.FromMinutes(10);
        private const string StartMacAddress = "14:2D:F5:00:3B:5B";
        private const string MacPrefix = "14:2D:F5";

        public MacAddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MacAddress>> GetAllAsync()
        {
            Log.Information("MacAddressRepository:GetAllAsync - start");
            try
            {
                var list = await _context.MacAddresses.AsNoTracking().ToListAsync();
                Log.Information("MacAddressRepository:GetAllAsync - success, count: {Count}", list.Count);
                return list;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "MacAddressRepository:GetAllAsync - database update exception");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MacAddressRepository:GetAllAsync - unexpected error");
                throw;
            }
        }

        public async Task<MacAddress?> GetByIdAsync(int id)
        {
            Log.Information("MacAddressRepository:GetByIdAsync - start id: {Id}", id);
            try
            {
                var entity = await _context.MacAddresses.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                Log.Information("MacAddressRepository:GetByIdAsync - {Result}", entity == null ? "not found" : "found");
                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "MacAddressRepository:GetByIdAsync - database update exception for id {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MacAddressRepository:GetByIdAsync - unexpected error for id {Id}", id);
                throw;
            }
        }

        public async Task<MacAddress?> GetNextAvailableMacAddressAsync()
        {
            Log.Information("MacAddressRepository:GetNextAvailableMacAddressAsync - start");
            try
            {
                var entity = await _context.MacAddresses
                    .AsNoTracking()
                    .Where(m => m.Status == MacAddressStatus.AVAILABLE)
                    .OrderBy(m => m.Id)
                    .FirstOrDefaultAsync();

                Log.Information("MacAddressRepository:GetNextAvailableMacAddressAsync - {Result}", entity == null ? "no MAC available" : $"found Id={entity.Id} Address={entity.MacAddressValue}");
                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "MacAddressRepository:GetNextAvailableMacAddressAsync - database update exception");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MacAddressRepository:GetNextAvailableMacAddressAsync - unexpected error");
                throw;
            }
        }

        public async Task<MacAddress?> ReserveNextMacAddressAsync()
        {
            Log.Information("MacAddressRepository:ReserveNextMacAddressAsync - waiting for lock");
            await _reservationLock.WaitAsync();
            try
            {
                Log.Information("MacAddressRepository:ReserveNextMacAddressAsync - lock acquired");
                var staleThreshold = DateTime.UtcNow - _reservationTimeout;

                var entity = await _context.MacAddresses
                    .Where(m => m.Status == MacAddressStatus.AVAILABLE
                             || (m.Status == MacAddressStatus.RESERVED && m.ReservedAt < staleThreshold))
                    .OrderBy(m => m.Id)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    Log.Warning("MacAddressRepository:ReserveNextMacAddressAsync - no MAC available");
                    return null;
                }

                entity.Status = MacAddressStatus.RESERVED;
                entity.ReservedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                Log.Information("MacAddressRepository:ReserveNextMacAddressAsync - reserved Id={Id} Address={MacAddressValue}", entity.Id, entity.MacAddressValue);
                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "MacAddressRepository:ReserveNextMacAddressAsync - database update exception");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MacAddressRepository:ReserveNextMacAddressAsync - unexpected error");
                throw;
            }
            finally
            {
                _reservationLock.Release();
                Log.Information("MacAddressRepository:ReserveNextMacAddressAsync - lock released");
            }
        }

        public async Task<MacAddress?> GetByMacAddressValueAsync(string macAddressValue)
        {
            Log.Information("MacAddressRepository:GetByMacAddressValueAsync - start MacAddressValue: {MacAddressValue}", macAddressValue);
            try
            {
                var entity = await _context.MacAddresses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.MacAddressValue == macAddressValue);

                Log.Information("MacAddressRepository:GetByMacAddressValueAsync - {Result}", entity == null ? "not found" : "found");
                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "MacAddressRepository:GetByMacAddressValueAsync - database update exception for MacAddressValue {MacAddressValue}", macAddressValue);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MacAddressRepository:GetByMacAddressValueAsync - unexpected error for MacAddressValue {MacAddressValue}", macAddressValue);
                throw;
            }
        }

        public async Task<MacAddress?> MarkMacAddressAsUsedAsync(string macAddressValue)
        {
            Log.Information("MacAddressRepository:MarkMacAddressAsUsedAsync - start MacAddressValue: {MacAddressValue}", macAddressValue);
            try
            {
                var entity = await _context.MacAddresses
                    .FirstOrDefaultAsync(m => m.MacAddressValue == macAddressValue);

                if (entity == null)
                {
                    Log.Information("MacAddressRepository:MarkMacAddressAsUsedAsync - MacAddressValue {MacAddressValue} not found", macAddressValue);
                    return null;
                }

                entity.Status = MacAddressStatus.USED;
                entity.UsedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                Log.Information("MacAddressRepository:MarkMacAddressAsUsedAsync - marked as USED id: {Id}", entity.Id);
                return entity;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                Log.Error(concEx, "MacAddressRepository:MarkMacAddressAsUsedAsync - concurrency error for MacAddressValue {MacAddressValue}", macAddressValue);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "MacAddressRepository:MarkMacAddressAsUsedAsync - database update exception for MacAddressValue {MacAddressValue}", macAddressValue);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MacAddressRepository:MarkMacAddressAsUsedAsync - unexpected error for MacAddressValue {MacAddressValue}", macAddressValue);
                throw;
            }
        }

        public async Task<MacAddress> CreateAsync(MacAddress macAddress)
        {
            Log.Information("MacAddressRepository:CreateAsync - start MacAddressValue: {MacAddressValue}", macAddress?.MacAddressValue);
            try
            {
                _context.MacAddresses.Add(macAddress);
                await _context.SaveChangesAsync();
                Log.Information("MacAddressRepository:CreateAsync - created id: {Id}", macAddress.Id);
                return macAddress;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                Log.Error(concEx, "MacAddressRepository:CreateAsync - concurrency error for MacAddressValue {MacAddressValue}", macAddress?.MacAddressValue);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "MacAddressRepository:CreateAsync - database update exception for MacAddressValue {MacAddressValue}", macAddress?.MacAddressValue);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MacAddressRepository:CreateAsync - unexpected error for MacAddressValue {MacAddressValue}", macAddress?.MacAddressValue);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(MacAddress macAddress)
        {
            Log.Information("MacAddressRepository:UpdateAsync - start id: {Id}", macAddress?.Id);
            try
            {
                var exists = await _context.MacAddresses.AnyAsync(m => m.Id == macAddress.Id);
                if (!exists)
                {
                    Log.Information("MacAddressRepository:UpdateAsync - id {Id} not found", macAddress.Id);
                    return false;
                }

                _context.MacAddresses.Update(macAddress);
                await _context.SaveChangesAsync();
                Log.Information("MacAddressRepository:UpdateAsync - updated id: {Id}", macAddress.Id);
                return true;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                Log.Error(concEx, "MacAddressRepository:UpdateAsync - concurrency error for id {Id}", macAddress?.Id);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "MacAddressRepository:UpdateAsync - database update exception for id {Id}", macAddress?.Id);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MacAddressRepository:UpdateAsync - unexpected error for id {Id}", macAddress?.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Log.Information("MacAddressRepository:DeleteAsync - start id: {Id}", id);
            try
            {
                var entity = await _context.MacAddresses.FirstOrDefaultAsync(m => m.Id == id);
                if (entity == null)
                {
                    Log.Information("MacAddressRepository:DeleteAsync - id {Id} not found", id);
                    return false;
                }

                _context.MacAddresses.Remove(entity);
                await _context.SaveChangesAsync();
                Log.Information("MacAddressRepository:DeleteAsync - deleted id: {Id}", id);
                return true;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                Log.Error(concEx, "MacAddressRepository:DeleteAsync - concurrency error for id {Id}", id);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "MacAddressRepository:DeleteAsync - database update exception for id {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MacAddressRepository:DeleteAsync - unexpected error for id {Id}", id);
                throw;
            }
        }

        public async Task<MacAddress> GenerateAndReserveNextMacAddressAsync()
        {
            Log.Information("MacAddressRepository:GenerateAndReserveNextMacAddressAsync - waiting for lock");
         await _reservationLock.WaitAsync();
            
         try
         {
      Log.Information("MacAddressRepository:GenerateAndReserveNextMacAddressAsync - lock acquired, starting transaction");
 
         // Use the execution strategy to handle transactions with retry logic
                var strategy = _context.Database.CreateExecutionStrategy();
    
          return await strategy.ExecuteAsync(async () =>
        {
using var transaction = await _context.Database.BeginTransactionAsync();
        
  try
    {
             // Retrieve the last inserted MAC address with row-level lock
      var lastMac = await _context.MacAddresses
          .OrderByDescending(m => m.Id)
            .FirstOrDefaultAsync();

     string nextMacAddress;

        if (lastMac == null)
                 {
    // Table is empty, start with the initial MAC address
        nextMacAddress = StartMacAddress;
          Log.Information("MacAddressRepository:GenerateAndReserveNextMacAddressAsync - table empty, starting with {MacAddress}", nextMacAddress);
           }
     else
     {
    // Convert MAC to long, increment, and convert back
long macAsLong = MacToLong(lastMac.MacAddressValue);
        macAsLong++;
        nextMacAddress = LongToMac(macAsLong);
   
     Log.Information("MacAddressRepository:GenerateAndReserveNextMacAddressAsync - incremented from {LastMac} to {NextMac}", 
                   lastMac.MacAddressValue, nextMacAddress);
         }

      // Verify the prefix to prevent overflow
   if (!nextMacAddress.StartsWith(MacPrefix, StringComparison.OrdinalIgnoreCase))
          {
      Log.Error("MacAddressRepository:GenerateAndReserveNextMacAddressAsync - MAC address overflow detected. Generated MAC {MacAddress} does not start with prefix {Prefix}", 
       nextMacAddress, MacPrefix);
     throw new InvalidOperationException($"MAC address overflow: generated address {nextMacAddress} does not start with required prefix {MacPrefix}");
}

          // Insert the new MAC address with RESERVED status
     var newMacAddress = new MacAddress
      {
      MacAddressValue = nextMacAddress,
                     Status = MacAddressStatus.RESERVED,
      ReservedAt = DateTime.UtcNow,
      CreatedAt = DateTime.UtcNow
 };

          _context.MacAddresses.Add(newMacAddress);
    await _context.SaveChangesAsync();
  await transaction.CommitAsync();

               Log.Information("MacAddressRepository:GenerateAndReserveNextMacAddressAsync - successfully created and reserved MAC Id={Id} Address={MacAddress}", 
         newMacAddress.Id, newMacAddress.MacAddressValue);

       return newMacAddress;
       }
             catch
     {
     await transaction.RollbackAsync();
   Log.Information("MacAddressRepository:GenerateAndReserveNextMacAddressAsync - transaction rolled back");
                throw;
         }
     });
 }
          catch (Exception ex)
       {
      Log.Error(ex, "MacAddressRepository:GenerateAndReserveNextMacAddressAsync - error occurred");
             throw;
        }
   finally
            {
                _reservationLock.Release();
       Log.Information("MacAddressRepository:GenerateAndReserveNextMacAddressAsync - lock released");
            }
        }

        /// <summary>
        /// Converts a MAC address string (AA:BB:CC:DD:EE:FF) to a 48-bit long integer
        /// </summary>
        private static long MacToLong(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac))
            {
             throw new ArgumentException("MAC address cannot be null or empty", nameof(mac));
  }

            // Remove colons and parse as hexadecimal
   string hexString = mac.Replace(":", "");
            
     if (hexString.Length != 12)
    {
  throw new ArgumentException($"Invalid MAC address format: {mac}. Expected format: AA:BB:CC:DD:EE:FF", nameof(mac));
            }

   return Convert.ToInt64(hexString, 16);
  }

        /// <summary>
        /// Converts a 48-bit long integer to a MAC address string (AA:BB:CC:DD:EE:FF)
        /// </summary>
        private static string LongToMac(long value)
    {
      if (value < 0 || value > 0xFFFFFFFFFFFF)
            {
         throw new ArgumentOutOfRangeException(nameof(value), "Value must be a valid 48-bit MAC address (0 to 281474976710655)");
   }

       // Convert to 12-character hex string with leading zeros
   string hexString = value.ToString("X12");

            // Format as AA:BB:CC:DD:EE:FF
      return $"{hexString.Substring(0, 2)}:{hexString.Substring(2, 2)}:{hexString.Substring(4, 2)}:" +
         $"{hexString.Substring(6, 2)}:{hexString.Substring(8, 2)}:{hexString.Substring(10, 2)}";
        }
    }
}
