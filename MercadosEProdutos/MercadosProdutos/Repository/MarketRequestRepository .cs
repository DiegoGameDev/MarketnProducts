using DBContext;
using DBModel;
using Microsoft.EntityFrameworkCore;
using Results;
using Enums;

namespace Repository;

public class MarketRequestRepository : IMarketRequestRepository
{
    private readonly MarketDBContext _context;

    public MarketRequestRepository(MarketDBContext context)
    {
        _context = context;
    }

    public async Task<ResultOperation> CreateRequest(MarketRequestCreation request)
    {
        request.Status = Enums.MarketStatus.Pending;
        request.CreatedAt = DateTime.UtcNow;

        await _context.MarketRequestCreations.AddAsync(request);
        await _context.SaveChangesAsync();

        return ResultOperation.Ok("Solicitação enviada com sucesso");
    }

    public async Task<ResultOperation<IEnumerable<MarketRequestCreation>>> GetPendingRequests()
    {
        var requests = await _context.MarketRequestCreations
            .Where(x => x.Status == MarketStatus.Pending)
            .Include(x => x.Market)
            .AsNoTracking()
            .ToListAsync();

        return ResultOperation<IEnumerable<MarketRequestCreation>>.Ok(requests);
    }

    public async Task<ResultOperation<IEnumerable<Market>>> GetRejectRequests()
    {
        var requests = await _context.MarketRequestCreations
            .Where(x => x.Status == MarketStatus.Rejected)
            .Select(x => x.Market)
            .AsNoTracking()
            .ToListAsync();

        return ResultOperation<IEnumerable<Market>>.Ok(requests);
    }

    public async Task<ResultOperation> ApproveRequest(Guid requestId)
    {
        var request = await _context.MarketRequestCreations.Where(x => x.MarketId == requestId)
            .Include(x => x.Market)
            .FirstOrDefaultAsync();

        if (request == null)
            return ResultOperation.Fail("Solicitação não encontrada");

        if (request.Status != MarketStatus.Pending)
            return ResultOperation.Fail("Solicitação já processada");

        request.Status = MarketStatus.Approved;
        request.UpdateDate = DateTime.UtcNow;

        if (request.Market != null)
        {
            request.Market.marketReviewStatus = MarketStatus.Approved;
        }

        _context.MarketList.Update(request.Market);
        _context.MarketRequestCreations.Update(request);
        await _context.SaveChangesAsync();

        return ResultOperation.Ok("Solicitação aprovada");
    }

    public async Task<ResultOperation> RejectRequest(Guid requestId, string reason)
    {
        var request = await _context.MarketRequestCreations.Where(x => x.MarketId == requestId)
            .Include(x => x.Market)
            .FirstOrDefaultAsync();

        if (request == null)
            return ResultOperation.Fail("Solicitação não encontrada");

        if (request.Status != MarketStatus.Pending)
            return ResultOperation.Fail("Solicitação já processada");

        request.Status = MarketStatus.Rejected;
        request.RejectionReason = reason;

        _context.MarketRequestCreations.Update(request);
        _context.MarketList.Update(request.Market);
        await _context.SaveChangesAsync();

        return ResultOperation.Ok("Solicitação rejeitada");
    }

    public async Task<ResultOperation<MarketRequestCreation>> GetMarketRequestByMarketID(Guid id)
    {
        var request = await _context.MarketRequestCreations.Where(x=>x.MarketId == id)
                        .Include(x => x.Market).FirstOrDefaultAsync();

        return ResultOperation<MarketRequestCreation>.Ok(request);
    }
}