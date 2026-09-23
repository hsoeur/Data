using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Api.Data.Repositories.ManufacturingOrderNotificationRepo;

namespace Api.Data.Repositories.ManufacturingOrderRepo
{
    public class ManufacturingOrderRepository : IManufacturingOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IManufacturingOrderNotificationRepository _notificationRepository;

        public ManufacturingOrderRepository(AppDbContext context, IManufacturingOrderNotificationRepository notificationRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
        }

        public async Task<ICollection<ManufacturingOrder>> GetCollectionManufacturingOrders()
        {
            Log.Information("GetCollectionManufacturingOrders called.");

            try
            {
                var ordreFabrications = await _context.ManufacturingOrders.AsNoTracking().ToListAsync();
                Log.Information("Retrieved {Count} ordreFabrications.", ordreFabrications.Count);
                return ordreFabrications;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to retrieve all ordreFabrications. Exception: {Exception}", ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public Task<ManufacturingOrder?> GetManufacturingOrderForValidation(int manufacturingOrderUid)
        {
            return _context.ManufacturingOrders
                .AsNoTracking()
                .SingleOrDefaultAsync(order => order.ManufacturingOrderUid == manufacturingOrderUid);
        }

        public async Task<ManufacturingOrder?> GetManufacturingOrderByManufacturingOrderUid(string manufacturingOrderUid)
        {
            Log.Information("GetManufacturingOrderByManufacturingOrderUid called with uidOF: {UidOF}", manufacturingOrderUid);

            try
            {
                if (!int.TryParse(manufacturingOrderUid, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uid))
                {
                    Log.Warning("Invalid ManufacturingOrderUid format: {UidOF}", manufacturingOrderUid);
                    return null;
                }

                var ordreFabrication = await _context.ManufacturingOrders
                    .FirstOrDefaultAsync(o => o.ManufacturingOrderUid == uid);

                if (ordreFabrication == null)
                {
                    Log.Warning("No ordreFabrication found with uidOF: {UidOF}", manufacturingOrderUid);
                }
                else
                {
                    // Update tracked entity directly to avoid multiple tracked instances
                    ordreFabrication.Status = "ACTIVE";
                    await _context.SaveChangesAsync();

                    Log.Information("Updated ordreFabrication to ACTIVE with uidOF: {UidOF}", manufacturingOrderUid);
                }

                return ordreFabrication;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to retrieve/update ordreFabrication with uidOF: {UidOF}. Exception: {Exception}", manufacturingOrderUid, ex);
                throw;
            }
        }


        public async Task<ICollection<ManufacturingOrder>> GetCollectionManufacturingOrderByArticleUid(string articleUid)
        {
            Log.Information("GetCollectionManufacturingOrderByArticleUid called with uidProduit: {UidProduit}", articleUid);

            try
            {
                if (string.IsNullOrWhiteSpace(articleUid))
                {
                    Log.Warning("Invalid ProductUid format: {UidProduit}", articleUid);
                    return Array.Empty<ManufacturingOrder>();
                }

                var ordreFabrications = await _context.ManufacturingOrders
                    .Where(o => o.ProductUid == articleUid)
                    .ToListAsync();

                Log.Information("Retrieved {Count} ordreFabrications for uidProduit: {UidProduit}", ordreFabrications.Count, articleUid);
                return ordreFabrications;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to retrieve ordreFabrications for uidProduit: {UidProduit}. Exception: {Exception}", articleUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<ManufacturingOrder> GetManufacturingOrderByManufacturingOrderUidKeepAlive(string manufacturingOrderUid)
        {

            try
            {
                if (!int.TryParse(manufacturingOrderUid, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uid))
                {
                    Log.Warning("Invalid ManufacturingOrderUid format for KeepAlive: {UidOF}", manufacturingOrderUid);
                    return null!; // preserve signature; caller should handle null
                }

                var ordreFabrication = await _context.ManufacturingOrders
                    .AsNoTracking()
                    .Where(o => o.ManufacturingOrderUid == uid)
                    .FirstOrDefaultAsync();

                return ordreFabrication!;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to KeepAlive API DATA ordreFabrication with uidOF: {UidOF}. Exception: {Exception}", manufacturingOrderUid, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<ICollection<ManufacturingOrder>> GetCollectionManufacturingOrderByManufacturingOrderAmphitechOrder(string amphitechOrder)
        {
            Log.Information("GetCollectionManufacturingOrderByManufacturingOrderAmphitechOrder called with cdeAmphitech: {CdeAmphitech}", amphitechOrder);

            try
            {
                var ordreFabrications = await _context.ManufacturingOrders
                    .Where(o => o.ExternalOrderNumber == amphitechOrder)
                    .ToListAsync();

                Log.Information("Retrieved {Count} ordreFabrications for cdeAmphitech: {CdeAmphitech}", ordreFabrications.Count, amphitechOrder);

                return ordreFabrications;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to retrieve ordreFabrications for cdeAmphitech: {CdeAmphitech}. Exception: {Exception}", amphitechOrder, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<List<ManufacturingOrder>> GetCollectionManufacturingOrdersByHorizonOrder(string horizonOrder)
        {
            if (string.IsNullOrWhiteSpace(horizonOrder))
            {
                Log.Warning("GetCollectionManufacturingOrdersByHorizonOrder called with an empty or null horizonOrder.");
                return new List<ManufacturingOrder>();
            }

            try
            {

                Log.Information("Fetching ManufacturingOrders for HorizonOrder: {HorizonOrder}", horizonOrder);

                var list = await _context.ManufacturingOrders
                                          .Where(of => of.InternalOrderNumber == horizonOrder)
                                          .ToListAsync();

                Log.Information("{Count} ManufacturingOrders found for HorizonOrder: {HorizonOrder}", list.Count, horizonOrder);

                return list;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching ManufacturingOrders for HorizonOrder: {HorizonOrder}", horizonOrder);
                throw; // Rethrow the exception to ensure it propagates up the call stack.
            }
        }

        public async Task<bool> UpdateManufacturingOrderQuantityAndFirstSerial(string manufacturingOrderUid, string quantity, string firstSerialNumber, string operatorUid)
        {
            Log.Information("UpdateManufacturingOrderQuantityAndFirstSerial called with uidOF: {UidOF}, quantity: {Quantity}, firstSerialNumber: {FirstSerialNumber}",
                manufacturingOrderUid, quantity, firstSerialNumber);

            try
            {
                if (!int.TryParse(manufacturingOrderUid, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uid))
                {
                    Log.Warning("Invalid ManufacturingOrderUid format: {UidOF}", manufacturingOrderUid);
                    return false;
                }

                var manufacturingOrder = await _context.ManufacturingOrders
                    .FirstOrDefaultAsync(o => o.ManufacturingOrderUid == uid);

                if (manufacturingOrder == null)
                {
                    Log.Warning("No manufacturing order found with uidOF: {UidOF}", manufacturingOrderUid);
                    return false;
                }

                if (!int.TryParse(quantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out var qty) ||
                    !int.TryParse(firstSerialNumber, NumberStyles.Integer, CultureInfo.InvariantCulture, out int newStart))
                {
                    Log.Warning("Invalid input: Quantity or FirstSerialNumber not valid formats. Quantity: {Quantity}, FirstSerialNumber: {FirstSerialNumber}",
                        quantity, firstSerialNumber);
                    return false;
                }

                int newEnd = newStart + qty -1;

                var productUid = manufacturingOrder.ProductUid;

                if (await IsSerialNumberRangeOverlapping(productUid, uid, newStart, newEnd))
                {
                    Log.Warning("Update aborted due to serial number overlap for ProductUid: {ProductUid}", productUid);
                    throw new Exception("Serial number overlapping");
                }

                bool quantityChanged = manufacturingOrder.Quantity != qty;
                bool serialChanged = manufacturingOrder.FirstSerialNumber != newStart;

                if (quantityChanged)
                {
                    // Update tracked entity property via Entry API (immutable record)
                    _context.Entry(manufacturingOrder).Property(nameof(ManufacturingOrder.Quantity)).CurrentValue = qty;
                    // Placeholder: notify external system if needed
                    // await _notificationRepository.UpdateQuantityAsync(manufacturingOrder.ManufacturingOrderUid + "_Quantity", quantity, operatorUid);
                }

                if (serialChanged)
                {
                    // Update tracked entity property via Entry API (immutable record)
                    _context.Entry(manufacturingOrder).Property(nameof(ManufacturingOrder.FirstSerialNumber)).CurrentValue = newStart;
                    // Placeholder: notify external system if needed
                    // await _notificationRepository.UpdateSerialNumberAsync(manufacturingOrder.ManufacturingOrderUid + "_SerialNumber", firstSerialNumber, operatorUid);
                }

                await _context.SaveChangesAsync();

                Log.Information("Manufacturing order {UidOF} updated successfully.", manufacturingOrderUid);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Exception while updating manufacturing order {UidOF}: {Exception}", manufacturingOrderUid, ex);
                throw;
            }
        }


        private async Task<bool> IsSerialNumberRangeOverlapping(string productUid, int excludedManufacturingOrderUid, int updateStart, int updateEnd)
        {
            var existingOrders = await _context.ManufacturingOrders
                .Where(o => o.ProductUid == productUid && o.ManufacturingOrderUid != excludedManufacturingOrderUid)
                .ToListAsync();

            foreach (var order in existingOrders)
            {
                int existingStart = order.FirstSerialNumber;
                int existingQty = order.Quantity;
                int existingEnd = existingStart + existingQty -1;

                if (updateStart <= existingEnd && existingStart <= updateEnd)
                {
                    Log.Warning("Overlap found between update range [{UpdateStart}, {UpdateEnd}] and existing range [{ExistingStart}, {ExistingEnd}] (OF: {Uid})",
                        updateStart, updateEnd, existingStart, existingEnd, order.ManufacturingOrderUid);
                    return true;
                }
            }

            return false;
        }
    }
}
