using DBModel;
using Results;

namespace Repository;

public interface IMarketRequestRepository
{
    Task<ResultOperation> CreateRequest(MarketRequestCreation request);

    Task<ResultOperation<IEnumerable<MarketRequestCreation>>> GetPendingRequests();

    Task<ResultOperation> ApproveRequest(int requestId);

    Task<ResultOperation> RejectRequest(int requestId, string reason);
}