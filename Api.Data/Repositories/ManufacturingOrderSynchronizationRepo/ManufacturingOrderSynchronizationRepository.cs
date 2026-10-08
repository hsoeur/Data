using System.Globalization;
using DataLib;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repositories.ManufacturingOrderSynchronizationRepo
{
    public sealed class ManufacturingOrderSynchronizationRepository : IManufacturingOrderSynchronizationRepository
    {
        private readonly AppDbContext _context;

        public ManufacturingOrderSynchronizationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<DatabaseNotification>> GetDesynchronizationsAsync(
            int manufacturingOrderUid)
        {
            var entityUid = manufacturingOrderUid.ToString(CultureInfo.InvariantCulture);

            return await _context.DatabaseNotifications
                .AsNoTracking()
                .Where(notification =>
                    notification.EntityType == "ManufacturingOrder" &&
                    notification.EntityUId == entityUid &&
                    notification.Processed == "NOT_PROCESSED" &&
                    (notification.PropertyName == "Quantity" ||
                     notification.PropertyName == "SerialNumber" ||
                     notification.PropertyName == "FirstSerialNumber"))
                .OrderByDescending(notification => notification.ModifiedAt)
                .ToListAsync();
        }
    }
}
