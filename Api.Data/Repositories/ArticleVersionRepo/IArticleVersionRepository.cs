using DataLib;

namespace Api.Data.Repositories.ArticleVersionRepo
{
    public interface IArticleVersionRepository
    {
        Task<ArticleVersion?> GetArticleVersionByArticleUid(string articleUid);
    }
}
