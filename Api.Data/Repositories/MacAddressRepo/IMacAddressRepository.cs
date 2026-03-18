using DataLib;

namespace Api.Data.Repositories.MacAddressRepo
{
    public interface IMacAddressRepository
    {
        Task<IEnumerable<MacAddress>> GetAllAsync();
        Task<MacAddress?> GetByIdAsync(int id);
        Task<MacAddress?> GetNextAvailableMacAddressAsync();
        Task<MacAddress> CreateAsync(MacAddress macAddress);
        Task<bool> UpdateAsync(MacAddress macAddress);
        Task<bool> DeleteAsync(int id);
    }
}
