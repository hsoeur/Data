using DataLib;

namespace Api.Data.Repositories.LabelTemplateRepo
{
    public interface ILabelTemplateRepository
    {
        Task<LabelTemplate> GetLabelTemplateByArticleUidAndUsageUid(string articleUid, string usageUid);
        Task<List<LabelTemplate>> GetCollectionLabelTemplateByArticleUid(string articleUid);
        Task CreateLabelTemplate(LabelTemplate usageCorrespondanceProduit);
        Task<List<LabelTemplate>> GetCollectionLabelTemplates();
        public Task<ICollection<LabelTemplate>> GetCollectionLabelTemplateByKeywordSearch(string keyword);
        public Task UpdateLabelTemplate(LabelTemplate usageCorrespondanceProduit);
        Task<LabelTemplate> GetLabelTemplateByTemplateUid(string uIDUsageCorrespondanceProduit);
        public Task<List<LabelTemplate>> GetCollectionLabelTemplateActiveOnes();
        public Task<List<LabelTemplate>> GetCollectionLabelTemplateByKeywordSearchOnAcitveOnes(string keyword);
    }
}
