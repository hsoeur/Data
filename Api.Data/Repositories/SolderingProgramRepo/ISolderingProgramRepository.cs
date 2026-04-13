using DataLib;

namespace Api.Data.Repositories.SolderingProgramRepo
{
    public interface ISolderingProgramRepository
    {
        Task<List<SolderingProgram>> GetCollectionSolderingPrograms();
        Task<List<SolderingProgram>> GetCollectionSolderingProgramsByKeywordSearch(string keyword);
        Task<List<SolderingProgram>> GetCollectionSolderingProgramsActiveOnes();
        Task<List<SolderingProgram>> GetCollectionSolderingProgramsByKeywordActiveOnes(string keyword);
        Task<SolderingProgram> UpdateSolderingProgram(SolderingProgram solderingProgram);
        Task<SolderingProgram> CreateSolderingProgram(SolderingProgram solderingProgram);
        Task<List<SolderingProgram>> GetSolderingProgramByManufacturingOrderUid(int manufacturingOrderUid);
    }
}
