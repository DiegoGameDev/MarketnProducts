using Results;
using DBModel;
using System.Linq.Expressions;

namespace Repository
{
    public interface IMarketRepository
    {
        Task<ResultOperation<Market>> AddAsync(Market market);
        Task<ResultOperation> UpdateAsync(Market market);
        Task<ResultOperation> DeleteAsync(Guid id);
        Task<ResultOperation<Market>> GetByName(string slug);

        Task<ResultOperation<Market>> GetByIdAsync(Guid id);
        Task<ResultOperation<IEnumerable<Market>>> GetAllAsync();
        Task<ResultOperation<IEnumerable<Market>>> GetApprovedMarketListAsync();
        Task<ResultOperation<IEnumerable<Market>>> GetRejectMarketListAsync();
        Task<ResultOperation<List<Product>>> GetProductsFromMarket(Market market);

        //reviewers area
        Task<ResultOperation> ApproveMarket(Guid marketId);
        Task<ResultOperation> RejectMarket(Guid marketId);
    }
}
