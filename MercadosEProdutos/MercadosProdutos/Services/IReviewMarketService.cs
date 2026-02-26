using DBModel;
using Enums;
using Results;

namespace Services;

public interface IReviewMarketService
{
    Task<ResultOperation> ApproveMarket(Guid marketId);
    Task<ResultOperation> RejectMarket(Guid marketId, string reason);
    Task<ResultOperation> RemoveMarket(Guid marketId, string reason);

    Task<ResultOperation> ApprovedEmailToMarketOwner(Guid marketId, Market market, User user);
    Task<ResultOperation> RejectedEmailToMarketOwner(Guid marketId, Market market, User user, string reason);
    Task<ResultOperation> RemoveEmailToMarketOwner(Guid marketId, Market market, User user, string reason);

    Task<ResultOperation<IEnumerable<DBModel.Market>>> GetApprovedMarketListAsync();
    Task<ResultOperation<IEnumerable<DBModel.Market>>> GetRejectMarketListAsync();
}