using DataLib;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repositories.LabelRepo
{
    public interface ILabelRepository
    {
        public Task<ICollection<Label>> GetCollectionLabelsByManufacturingOrderUid(int manufacturingOrderUid);
        public Task<ICollection<Label>> GetCollectionLabelsMostRecentByManufacturingOrderUid(int manufacturingOrderUid);
        public Task<Label> GetLabelByAuthentiUid(string authentiUid);
        public Task CreateLabel(Label label);
        Task CreateLabels(IReadOnlyCollection<Label> labels);
        Task<bool> LabelExists(string authentiUid);
        Task<bool> LabelPrinted(int manufacturingOrderUid, string usageUid);
        Task<ICollection<Label>> GetCollectionLabelsByKeywordSearch(string keyword, int manufacturingOrderUid, string usageUid);
        Task<List<Label>> GetCollectionLabelsByManufacturingOrderUidAndUsageUid(int manufacturingOrderUid, string usageUid);
        Task<Label> GetLabelLastSerialNumberPrintedByManufacturingOrderUid(int manufacturingOrderUid);
    }
}
