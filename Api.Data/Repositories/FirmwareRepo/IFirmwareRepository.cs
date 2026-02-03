using DataLib;

namespace Api.Data.Repositories.FirmwareRepo
{
    public interface IFirmwareRepository
    {
        Task<List<Firmware>> GetCollectionFirmwares();
        Task<List<Firmware>> GetCollectionFirmwareByKeywordActiveOnes(string keyword);
        Task<List<Firmware>> GetCollectionFirmwareByKeyword(string keyword);
        Task<List<Firmware>> GetCollectionFirmwareActiveOnes();
        Task<Firmware> GetFirmwareByArticleUidByLabelActiveOne(string articleUid, string label);
        Task<Firmware> IsDoubletProductAndVersion(Firmware firmware);
        Task<Firmware> UpdateFirmwareAnticipatedMode(Firmware firmware);
        Task<Firmware> UpdateFirmwareNominalMode(Firmware firmware);
        Task<Firmware> LinkFirmwareFileUpdate(Firmware firmware);
        Task<Firmware> SuspendFirmwareTemporary(Firmware firmware);
        Task<Firmware> UnsuspendFirmwareTemporary(Firmware firmware);
    }
}
