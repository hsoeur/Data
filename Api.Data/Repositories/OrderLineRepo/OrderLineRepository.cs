using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataLib;
using Serilog;

namespace Api.Data.Repositories.OrderLineRepo
{
    public class OrderLineRepository : IOrderLineRepository
    {
        private readonly AppDbContext _context;

        public OrderLineRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderLine>> GetCollectionOrderLinesByHorizonOrder(string horizonOrder)
        {
            if (string.IsNullOrWhiteSpace(horizonOrder))
            {
                Log.Warning("Attempted to fetch order lines with an empty or null Horizon Order identifier.");
                return new List<OrderLine>();
            }

            try
            {
                Log.Information("Fetching order lines for Horizon Order: {HorizonOrder}", horizonOrder);

                var list = await _context.OrderLines
                                         .Where(ol => ol.UidOrderLine.Contains(horizonOrder.Substring(0, 4)))
                                         .ToListAsync();

                Log.Information("Successfully fetched {Count} order lines for Horizon Order: {HorizonOrder}", list.Count, horizonOrder);
                return list;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching order lines for Horizon Order: {HorizonOrder}", horizonOrder);
                throw; // Rethrow exception to ensure it propagates
            }
        }

    }
}
