using DataLib;

namespace Api.Data.Repositories.DeviceIdentityRepo
{
    public interface IDeviceIdentityRepository
    {
        Task<IEnumerable<DeviceIdentity>> GetAllAsync();
        Task<DeviceIdentity?> GetByIdAsync(int id);
        Task<DeviceIdentity?> GetByEquipSNAsync(string equipSN);
        Task<DeviceIdentity?> GetByMacAddressValueAsync(string macAddressValue);
        Task<DeviceIdentity> CreateAsync(DeviceIdentity deviceIdentity);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByEquipSNAsync(string equipSN);
    }
}
