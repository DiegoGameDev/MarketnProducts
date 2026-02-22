using DBModel;
using Results;

namespace Services;

public interface IMarketAppService
{
    Task<ResultOperation<IEnumerable<Market>>> GetMarketsByUser(string userId);
    Task<ResultOperation<Market>> GetMarketById(Guid id);
    Task<ResultOperation<ICollection<Product>>> GetProductsInMarket(Guid market);
    Task<ResultOperation> CreateMarketWithRequest(Market market, User userId);
    Task<ResultOperation> UpdateMarket(Market market);
}