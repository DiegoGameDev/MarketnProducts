using Results;
using DBModel;
using System.Linq.Expressions;

namespace Repository
{
    public interface IMarketRepository
    {
        Task<ResultOperation<Market>> AddAsync(Market market);
        Task<ResultOperation<Market>> UpdateAsync(Market market);
        Task<ResultOperation<bool>> DeleteAsync(Guid id);
        Task<ResultOperation<Market>> GetByName(string slug);

        Task<ResultOperation<Market>> GetByIdAsync(Guid id);
        Task<ResultOperation<IEnumerable<Market>>> GetAllAsync();
        Task<ResultOperation<List<Product>>> GetProductsFromMarket(Market market);
    }
}
