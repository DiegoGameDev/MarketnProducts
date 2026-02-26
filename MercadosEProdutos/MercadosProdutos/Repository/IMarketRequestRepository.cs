using DBModel;
using Results;

namespace Repository;

public interface IMarketRequestRepository
{
    Task<ResultOperation> CreateRequest(MarketRequestCreation request);

    Task<ResultOperation<IEnumerable<MarketRequestCreation>>> GetPendingRequests();

    Task<ResultOperation> ApproveRequest(Guid requestId);

    Task<ResultOperation> RejectRequest(Guid requestId, string reason);
    Task<ResultOperation> DeleteAsyncMarket(Guid id);
    Task<ResultOperation<MarketRequestCreation>> GetMarketRequestByMarketID(Guid id);
}