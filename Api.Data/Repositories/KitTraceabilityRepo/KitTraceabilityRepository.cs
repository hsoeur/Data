using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.KitTraceabilityRepo
{
    public class KitTraceabilityRepository : IKitTraceabilityRepository
    {
        private readonly AppDbContext _context;

        public KitTraceabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AlreadyProcessed(int uidOf)
        {
            Log.Information("AlreadyProcessed called with uidOf: {UidOf}", uidOf);

            try
            {
                var ordreFabrication = await _context.ManufacturingOrders
                    .Where(of => of.ManufacturingOrderUid == uidOf)
                    .FirstOrDefaultAsync();

                if (ordreFabrication == null)
                {
                    Log.Warning("No OrdreFabrication found with uidOf: {UidOf}", uidOf);
                    return false;
                }

                bool exists = await _context.KitTraceabilities
                    .AnyAsync(kit => kit.ClientOrder == ordreFabrication.ExternalOrderNumber
                                     && kit.ManufacturingOrderUid == ordreFabrication.ManufacturingOrderUid
                                     && kit.Product == ordreFabrication.ProductUid);

                Log.Information("AlreadyProcessed result for uidOf: {UidOf} is {Exists}", uidOf, exists);

                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to determine if kit has already been processed for uidOf: {UidOf}", uidOf);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task CreateKit(KitTraceability kit)
        {
            Log.Information("CreateKit called with kit: {Kit}", kit);

            try
            {
                _context.KitTraceabilities.Add(kit);
                await _context.SaveChangesAsync();

                Log.Information("Kit created successfully: {Kit}", kit);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create kit: {Kit}", kit);
                throw; // Rethrow the exception to be handled upstream
            }
        }
    }
}
