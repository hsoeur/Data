using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.LabelTypeRepo
{

    public class LabelTypeRepository : ILabelTypeRepository
    {
        private readonly AppDbContext _context;

        public LabelTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LabelType>> GetCollectionLabelTypes()
        {
            try
            {
                return await _context.LabelTypes.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve LabelTypes.");
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task CreateLabelType(LabelType usage)
        {
            try
            {
                _context.LabelTypes.Add(usage);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create usage: {@Usage}", usage);
                throw; // Rethrow the exception to be handled upstream
            }
        }
    }
}
