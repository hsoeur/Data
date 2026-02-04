using DataLib;

namespace Api.Data.Repositories.SimCardRepo
{
    public interface ISimCardRepository
    {
        Task<IEnumerable<SimCard>> GetAllAsync();
        Task<SimCard?> GetByIdAsync(string uid);
        Task<SimCard> CreateAsync(SimCard simCard);
        Task<bool> UpdateAsync(SimCard simCard);
        Task<bool> DeleteAsync(string uid);
    }
}
