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

        public async Task<bool> AlreadyProcessed(int ManufacturingOrderUid)
        {
            Log.Information("AlreadyProcessed called with ManufacturingOrderUid: {ManufacturingOrderUid}", ManufacturingOrderUid);

            try
            {
                var manufacturingOrder = await _context.ManufacturingOrders
                    .Where(of => of.ManufacturingOrderUid == ManufacturingOrderUid)
                    .FirstOrDefaultAsync();

                if (manufacturingOrder == null)
                {
                    Log.Warning("No ManufacturingOrder found with ManufacturingOrderUid: {ManufacturingOrderUid}", ManufacturingOrderUid);
                    return false;
                }

                bool exists = await _context.KitTraceabilities
                    .AnyAsync(kit => kit.ClientOrder == manufacturingOrder.ExternalOrderNumber
                                     && kit.ManufacturingOrderUid == manufacturingOrder.ManufacturingOrderUid
                                     && kit.Product == manufacturingOrder.ProductUid);

                Log.Information("AlreadyProcessed result for ManufacturingOrderUid: {ManufacturingOrderUid} is {Exists}", ManufacturingOrderUid, exists);

                return exists;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to determine if kit has already been processed for ManufacturingOrderUid: {ManufacturingOrderUid}", ManufacturingOrderUid);
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

        public async Task<KitTraceability?> GetKitByProductAndSerialNumber(string Product, int SerialNumber)
        {
            Log.Information("GetKitByProductAndSerialNumber called with Product: {Product}, SerialNumber: {SerialNumber}", Product, SerialNumber);

            try
            {
                var kit = await _context.KitTraceabilities
                    .Where(k => k.Product == Product && k.SerialNumber == SerialNumber)
                    .FirstOrDefaultAsync();

                if (kit == null)
                {
                    Log.Information("No Kit found with Product: {Product} and SerialNumber: {SerialNumber}", Product, SerialNumber);
                }
                else
                {
                    Log.Information("Kit found: {Kit}", kit);
                }

                return kit;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get kit with Product: {Product} and SerialNumber: {SerialNumber}", Product, SerialNumber);
                throw;
            }
        }
    }
}
