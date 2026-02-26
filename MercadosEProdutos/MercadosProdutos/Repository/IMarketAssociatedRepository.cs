using DBModel;
using Results;

namespace Repository;

public interface IMarketAssociatedRepository
{
    Task<ResultOperation<IEnumerable<User>>> GetUserListByMarket(Market market);
    Task<ResultOperation<MarketAssociated>> GetMarketAssociatedByMarketID(Guid marketId);
    Task<ResultOperation<IEnumerable<Market>>> GetMarketListByUserId(string id);

    Task<ResultOperation> AddUserInMarket(User user, Market market);
}