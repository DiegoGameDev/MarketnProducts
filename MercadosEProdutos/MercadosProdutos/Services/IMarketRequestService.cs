using DBModel;
using Results;

namespace Services;

public interface IMarketRequestService
{
    Task<ResultOperation> CreateMarketWithRequest(Market market, User user);
}