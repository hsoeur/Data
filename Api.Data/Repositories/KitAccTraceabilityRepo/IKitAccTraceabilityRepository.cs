using DataLib;

namespace Api.Data.Repositories.KitAccTraceabilityRepo
{
    public interface IKitAccTraceabilityRepository
    {
        Task CreateKitAccessory(KitAccTraceability kitAcc);
        Task<KitAccTraceability?> GetKitAccessoryByProductUidByOrderLineUid(string productUid, string orderLineUid); 
    }
}
