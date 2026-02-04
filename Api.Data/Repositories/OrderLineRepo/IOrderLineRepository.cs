using System.Collections.Generic;
using System.Threading.Tasks;
using DataLib;

namespace Api.Data.Repositories.OrderLineRepo
{
    public interface IOrderLineRepository
    {
        Task<List<OrderLine>> GetCollectionOrderLinesByHorizonOrder(string horizonOrder);
    }
}
