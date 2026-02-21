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
        request.Status = Enums.MarketReviewStatus.Pending;
        request.CreatedAt = DateTime.UtcNow;

        await _context.MarketRequestCreations.AddAsync(request);
        await _context.SaveChangesAsync();

        return ResultOperation.Ok("Solicitação enviada com sucesso");
    }

    public async Task<ResultOperation<IEnumerable<MarketRequestCreation>>> GetPendingRequests()
    {
        var requests = await _context.MarketRequestCreations
            .Where(x => x.Status == MarketReviewStatus.Pending)
            .Include(x => x.Market)
            .AsNoTracking()
            .ToListAsync();

        return ResultOperation<IEnumerable<MarketRequestCreation>>.Ok(requests);
    }

    public async Task<ResultOperation> ApproveRequest(int requestId)
    {
        var request = await _context.MarketRequestCreations
            .Include(x => x.Market)
            .FirstOrDefaultAsync(x => x.ID == requestId);

        if (request == null)
            return ResultOperation.Fail("Solicitação não encontrada");

        if (request.Status != MarketReviewStatus.Pending)
            return ResultOperation.Fail("Solicitação já processada");

        request.Status = MarketReviewStatus.Approved;

        // Aqui você pode ativar o mercado
        if (request.Market != null)
        {
            request.Market.marketReviewStatus = MarketReviewStatus.Approved;
        }

        await _context.SaveChangesAsync();

        return ResultOperation.Ok("Solicitação aprovada");
    }

    public async Task<ResultOperation> RejectRequest(int requestId, string reason)
    {
        var request = await _context.MarketRequestCreations
            .FirstOrDefaultAsync(x => x.ID == requestId);

        if (request == null)
            return ResultOperation.Fail("Solicitação não encontrada");

        if (request.Status != MarketReviewStatus.Pending)
            return ResultOperation.Fail("Solicitação já processada");

        request.Status = MarketReviewStatus.Rejected;
        request.RejectionReason = reason;

        await _context.SaveChangesAsync();

        return ResultOperation.Ok("Solicitação rejeitada");
    }
}