using System.Collections.Generic;
using System.Threading.Tasks;
using DataLib;

namespace Api.Data.Repositories.ManufacturingOrderNotificationRepo
{
    public interface IManufacturingOrderNotificationRepository
    {
        Task CreateAsync(ManufacturingOrderNotification notification);
        Task<IEnumerable<ManufacturingOrderNotification>> GetAllAsync();
        Task UpdateAsync(ManufacturingOrderNotification notification);
        Task UpdateQuantityAsync(string notificationUid, string newQuantity, string operatorUid);
        Task UpdateSerialNumberAsync(string notificationUid, string newSerialNumber, string operatorUid);

    }
}
