using System;
using System.Linq;
using System.Threading.Tasks;
using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.KitAccTraceabilityRepo
{
    public class KitAccTraceabilityRepository : IKitAccTraceabilityRepository
    {
        private readonly AppDbContext _context;

        public KitAccTraceabilityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateKitAccessory(KitAccTraceability kitAcc)
        {
            Log.Information("Starting the creation of a new KitAccTraceability with OrderLineUid: {OrderLineUid}, Product: {Product}", kitAcc.OrderLineUid, kitAcc.Product);

            try
            {
                await _context.KitAccTraceabilities.AddAsync(kitAcc);
                await _context.SaveChangesAsync();

                Log.Information("Successfully created KitAccTraceability with UID: {Uid}", kitAcc.KitAccTraceabilityUid);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while creating KitAccTraceability with OrderLineUid: {OrderLineUid}, Product: {Product}", kitAcc.OrderLineUid, kitAcc.Product);
                throw;
            }
        }

        public async Task<KitAccTraceability?> GetKitAccessoryByProductUidByOrderLineUid(string productUid, string orderLineUid)
        {
            Log.Information("Attempting to retrieve KitAccTraceability with ProductUid: {ProductUid} and OrderLineUid: {OrderLineUid}", productUid, orderLineUid);

            try
            {
                var kitAcc = await _context.KitAccTraceabilities
                    .Where(kit => kit.Product == productUid && kit.OrderLineUid == orderLineUid)
                    .FirstOrDefaultAsync();

                if (kitAcc == null)
                {
                    Log.Warning("No KitAccTraceability found with ProductUid: {ProductUid} and OrderLineUid: {OrderLineUid}", productUid, orderLineUid);
                }
                else
                {
                    Log.Information("Successfully retrieved KitAccTraceability with UID: {Uid}", kitAcc.KitAccTraceabilityUid);
                }

                return kitAcc;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving KitAccTraceability with ProductUid: {ProductUid} and OrderLineUid: {OrderLineUid}", productUid, orderLineUid);
                throw;
            }
        }
    }
}
