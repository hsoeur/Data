using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataLib;

namespace Api.Data.Repositories.ManufacturingOrderRepo
{
    public interface IManufacturingOrderRepository
    {
        Task<ICollection<ManufacturingOrder>> GetCollectionManufacturingOrders();
        Task<ManufacturingOrder?> GetManufacturingOrderByManufacturingOrderUid(string manufacturingOrderUid);
        Task<ICollection<ManufacturingOrder>> GetCollectionManufacturingOrderByArticleUid(string articleUid);
        Task<ManufacturingOrder> GetManufacturingOrderByManufacturingOrderUidKeepAlive(string v);
        Task<ICollection<ManufacturingOrder>> GetCollectionManufacturingOrderByManufacturingOrderAmphitechOrder(string amphitechOrder);
        Task<List<ManufacturingOrder>> GetCollectionManufacturingOrdersByHorizonOrder(string horizonOrder);
        Task<bool> UpdateManufacturingOrderQuantityAndFirstSerial(string manufacturingOrderUid, string quantity, string firstSerialNumber, string operatorUid);
    }
}
