using DataLib;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repositories.OperationRepo
{
    public class OperationRepository : IOperationRepository
    {
        private readonly AppDbContext _context;

        public OperationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Operation>> GetCollectionOperationByProductUid(string productUid)
        {
            return await _context.Operations
                .Where(operation => operation.ProductUid == productUid)
                .Where(operation => _context.Sequences
                    .Any(sequence =>
                        sequence.Product == operation.ProductUid &&
                        sequence.OperationNumber == operation.OperationNumber))
                .ToListAsync();
        }

        public async Task<List<Operation>> GetCollectionOperationBasic(string productUid)
        {
            return await _context.Operations
                .Where(operation => operation.ProductUid == productUid)
                .ToListAsync();
        }

        public async Task<Operation?> GetPreviousOperationByProductUid(string productUid, int currentOperationNumber)
        {
            // Materialize operation numbers to memory
            var operationNumbers = await _context.Operations
                .Where(o => o.ProductUid == productUid)
                .Select(o => o.OperationNumber)
                .ToListAsync();

            // Compute previous numeric operation number strictly less than current
            var previousNum = operationNumbers
                .Where(v => v < currentOperationNumber)
                .DefaultIfEmpty(int.MinValue)
                .Max();

            if (previousNum == int.MinValue)
            {
                return null;
            }

            var prevOperation = await _context.Operations
                .Where(o => o.ProductUid == productUid && o.OperationNumber == previousNum)
                .FirstOrDefaultAsync();

            return prevOperation;
        }
    }
}
