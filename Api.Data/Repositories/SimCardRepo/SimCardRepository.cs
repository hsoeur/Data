using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.SimCardRepo
{
    public class SimCardRepository : ISimCardRepository
    {
        private readonly AppDbContext _context;

        public SimCardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SimCard>> GetAllAsync()
        {
            Log.Information("SimCardRepository:GetAllAsync - start");
            try
            {
                var list = await _context.SimCards.AsNoTracking().ToListAsync();
                Log.Information("SimCardRepository:GetAllAsync - success, count: {Count}", list.Count);
                return list;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "SimCardRepository:GetAllAsync - database update exception");
                throw;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "SimCardRepository:GetAllAsync - unexpected error");
                throw;
            }
        }

        public async Task<SimCard?> GetByIdAsync(string uid)
        {
            Log.Information("SimCardRepository:GetByIdAsync - start uid: {Uid}", uid);
            try
            {
                var entity = await _context.SimCards.AsNoTracking().FirstOrDefaultAsync(s => s.SimCardUid == uid);
                Log.Information("SimCardRepository:GetByIdAsync - {Result}", entity == null ? "not found" : "found");
                return entity;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "SimCardRepository:GetByIdAsync - database update exception for uid {Uid}", uid);
                throw;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "SimCardRepository:GetByIdAsync - unexpected error for uid {Uid}", uid);
                throw;
            }
        }

        public async Task<SimCard> CreateAsync(SimCard simCard)
        {
            Log.Information("SimCardRepository:CreateAsync - start SimCardNumber: {SimCardNumber}", simCard?.SimCardNumber);
            try
            {
                _context.SimCards.Add(simCard);
                await _context.SaveChangesAsync();
                Log.Information("SimCardRepository:CreateAsync - created uid: {Uid}", simCard.SimCardUid);
                return simCard;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                Log.Error(concEx, "SimCardRepository:CreateAsync - concurrency error for SimCardNumber {SimCardNumber}", simCard?.SimCardNumber);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "SimCardRepository:CreateAsync - database update exception for SimCardNumber {SimCardNumber}", simCard?.SimCardNumber);
                throw;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "SimCardRepository:CreateAsync - unexpected error for SimCardNumber {SimCardNumber}", simCard?.SimCardNumber);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(SimCard simCard)
        {
            Log.Information("SimCardRepository:UpdateAsync - start uid: {Uid}", simCard?.SimCardUid);
            try
            {
                var exists = await _context.SimCards.AnyAsync(s => s.SimCardUid == simCard.SimCardUid);
                if (!exists)
                {
                    Log.Information("SimCardRepository:UpdateAsync - uid {Uid} not found", simCard.SimCardUid);
                    return false;
                }

                _context.SimCards.Update(simCard);
                await _context.SaveChangesAsync();
                Log.Information("SimCardRepository:UpdateAsync - updated uid: {Uid}", simCard.SimCardUid);
                return true;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                Log.Error(concEx, "SimCardRepository:UpdateAsync - concurrency error for uid {Uid}", simCard?.SimCardUid);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "SimCardRepository:UpdateAsync - database update exception for uid {Uid}", simCard?.SimCardUid);
                throw;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "SimCardRepository:UpdateAsync - unexpected error for uid {Uid}", simCard?.SimCardUid);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string uid)
        {
            Log.Information("SimCardRepository:DeleteAsync - start uid: {Uid}", uid);
            try
            {
                var entity = await _context.SimCards.FirstOrDefaultAsync(s => s.SimCardUid == uid);
                if (entity == null)
                {
                    Log.Information("SimCardRepository:DeleteAsync - uid {Uid} not found", uid);
                    return false;
                }

                _context.SimCards.Remove(entity);
                await _context.SaveChangesAsync();
                Log.Information("SimCardRepository:DeleteAsync - deleted uid: {Uid}", uid);
                return true;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                Log.Error(concEx, "SimCardRepository:DeleteAsync - concurrency error for uid {Uid}", uid);
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                Log.Error(dbEx, "SimCardRepository:DeleteAsync - database update exception for uid {Uid}", uid);
                throw;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "SimCardRepository:DeleteAsync - unexpected error for uid {Uid}", uid);
                throw;
            }
        }
    }
}
