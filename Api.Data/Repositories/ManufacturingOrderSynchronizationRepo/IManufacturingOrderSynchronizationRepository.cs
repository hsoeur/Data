using DataLib;

namespace Api.Data.Repositories.ManufacturingOrderSynchronizationRepo
{
    public interface IManufacturingOrderSynchronizationRepository
    {
        Task<IReadOnlyCollection<DatabaseNotification>> GetDesynchronizationsAsync(int manufacturingOrderUid);
    }
}
