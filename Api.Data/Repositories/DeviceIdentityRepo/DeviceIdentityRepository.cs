using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.DeviceIdentityRepo
{
    public class DeviceIdentityRepository : IDeviceIdentityRepository
    {
        private readonly AppDbContext _context;

        public DeviceIdentityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeviceIdentity>> GetAllAsync()
        {
            Log.Information("DeviceIdentityRepository:GetAllAsync - start");
            try
            {
                var list = await _context.DeviceIdentities.AsNoTracking().ToListAsync();
                Log.Information("DeviceIdentityRepository:GetAllAsync - success, count: {Count}", list.Count);
                return list;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "DeviceIdentityRepository:GetAllAsync - database update exception");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeviceIdentityRepository:GetAllAsync - unexpected error");
                throw;
            }
        }

        public async Task<DeviceIdentity?> GetByIdAsync(int id)
        {
            Log.Information("DeviceIdentityRepository:GetByIdAsync - start id: {Id}", id);
            try
            {
                var entity = await _context.DeviceIdentities.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
                Log.Information("DeviceIdentityRepository:GetByIdAsync - {Result}", entity == null ? "not found" : "found");
                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "DeviceIdentityRepository:GetByIdAsync - database update exception for id {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeviceIdentityRepository:GetByIdAsync - unexpected error for id {Id}", id);
                throw;
            }
        }

        public async Task<DeviceIdentity?> GetByEquipSNAsync(string equipSN)
        {
            Log.Information("DeviceIdentityRepository:GetByEquipSNAsync - start EquipSN: {EquipSN}", equipSN);
            try
            {
                var entity = await _context.DeviceIdentities
            .AsNoTracking()
                     .FirstOrDefaultAsync(d => d.EquipSN == equipSN);

                Log.Information("DeviceIdentityRepository:GetByEquipSNAsync - {Result}", entity == null ? "not found" : "found");
                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "DeviceIdentityRepository:GetByEquipSNAsync - database update exception for EquipSN {EquipSN}", equipSN);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeviceIdentityRepository:GetByEquipSNAsync - unexpected error for EquipSN {EquipSN}", equipSN);
                throw;
            }
        }

        public async Task<DeviceIdentity?> GetByMacAddressValueAsync(string macAddressValue)
        {
            Log.Information("DeviceIdentityRepository:GetByMacAddressValueAsync - start MacAddressValue: {MacAddressValue}", macAddressValue);
            try
            {
                var entity = await _context.DeviceIdentities
             .AsNoTracking()
           .FirstOrDefaultAsync(d => d.MacAddressValue == macAddressValue);

                Log.Information("DeviceIdentityRepository:GetByMacAddressValueAsync - {Result}", entity == null ? "not found" : "found");
                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "DeviceIdentityRepository:GetByMacAddressValueAsync - database update exception for MacAddressValue {MacAddressValue}", macAddressValue);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeviceIdentityRepository:GetByMacAddressValueAsync - unexpected error for MacAddressValue {MacAddressValue}", macAddressValue);
                throw;
            }
        }

        public async Task<DeviceIdentity> CreateAsync(DeviceIdentity deviceIdentity)
        {
            Log.Information("DeviceIdentityRepository:CreateAsync - start EquipSN: {EquipSN}, MacAddressValue: {MacAddressValue}",
       deviceIdentity?.EquipSN, deviceIdentity?.MacAddressValue);
            try
            {
                _context.DeviceIdentities.Add(deviceIdentity);
                await _context.SaveChangesAsync();
                Log.Information("DeviceIdentityRepository:CreateAsync - created id: {Id}", deviceIdentity.Id);
                return deviceIdentity;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                Log.Error(concEx, "DeviceIdentityRepository:CreateAsync - concurrency error for EquipSN {EquipSN}", deviceIdentity?.EquipSN);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "DeviceIdentityRepository:CreateAsync - database update exception for EquipSN {EquipSN}", deviceIdentity?.EquipSN);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeviceIdentityRepository:CreateAsync - unexpected error for EquipSN {EquipSN}", deviceIdentity?.EquipSN);
                throw;
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            Log.Information("DeviceIdentityRepository:DeleteAsync - start id: {Id}", id);
            try
            {
                var entity = await _context.DeviceIdentities.FirstOrDefaultAsync(d => d.Id == id);
                if (entity == null)
                {
                    Log.Information("DeviceIdentityRepository:DeleteAsync - id {Id} not found", id);
                    return false;
                }

                _context.DeviceIdentities.Remove(entity);
                await _context.SaveChangesAsync();
                Log.Information("DeviceIdentityRepository:DeleteAsync - deleted id: {Id}", id);
                return true;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                Log.Error(concEx, "DeviceIdentityRepository:DeleteAsync - concurrency error for id {Id}", id);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "DeviceIdentityRepository:DeleteAsync - database update exception for id {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeviceIdentityRepository:DeleteAsync - unexpected error for id {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteByEquipSNAsync(string equipSN)
        {
            Log.Information("DeviceIdentityRepository:DeleteByEquipSNAsync - start EquipSN: {EquipSN}", equipSN);
            try
            {
                var entity = await _context.DeviceIdentities.FirstOrDefaultAsync(d => d.EquipSN == equipSN);
                if (entity == null)
                {
                    Log.Information("DeviceIdentityRepository:DeleteByEquipSNAsync - EquipSN {EquipSN} not found", equipSN);
                    return false;
                }

                _context.DeviceIdentities.Remove(entity);
                await _context.SaveChangesAsync();
                Log.Information("DeviceIdentityRepository:DeleteByEquipSNAsync - deleted EquipSN: {EquipSN}", equipSN);
                return true;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                Log.Error(concEx, "DeviceIdentityRepository:DeleteByEquipSNAsync - concurrency error for EquipSN {EquipSN}", equipSN);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "DeviceIdentityRepository:DeleteByEquipSNAsync - database update exception for EquipSN {EquipSN}", equipSN);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeviceIdentityRepository:DeleteByEquipSNAsync - unexpected error for EquipSN {EquipSN}", equipSN);
                throw;
            }
        }
    }
}
