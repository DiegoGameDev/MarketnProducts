using DBModel;
using Enums;
using Results;

namespace Services;

public interface IReviewMarketService
{
    Task<ResultOperation> ApproveMarket(Guid marketId);
    Task<ResultOperation> RejectMarket(Guid marketId, string reason);
    Task<ResultOperation> RemoveMarket(Guid marketId, string reason);

    Task<ResultOperation> ApprovedEmailToMarketOwner(Market market, User user);
    Task<ResultOperation> RejectedEmailToMarketOwner( Market market, User user, string reason);
    Task<ResultOperation> RemoveEmailToMarketOwner(Market market, User user, string reason);

    Task<ResultOperation<IEnumerable<DBModel.Market>>> GetApprovedMarketListAsync();
    Task<ResultOperation<IEnumerable<DBModel.Market>>> GetRejectMarketListAsync();
    Task<ResultOperation<IEnumerable<DBModel.MarketRequestCreation>>> GetPendingMarketListAsync();
    Task<ResultOperation<Market>> GetMarketByID(Guid id);
    Task<ResultOperation<MarketRequestCreation>> GetMarketRequestByID(Guid id);
}