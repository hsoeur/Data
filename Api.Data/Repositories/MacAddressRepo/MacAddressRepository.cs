using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.MacAddressRepo
{
    public class MacAddressRepository : IMacAddressRepository
    {
        private readonly AppDbContext _context;

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
    }
}
