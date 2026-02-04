using DataLib;

namespace Api.Data.Repositories.LabelTypeRepo
{
    public interface ILabelTypeRepository
    {
        Task<IEnumerable<LabelType>> GetCollectionLabelTypes();
        Task CreateLabelType(LabelType LabelType);
    }
}
