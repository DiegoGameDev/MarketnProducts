using DBModel;
using Repository;
using Results;
using Enums;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Services;

public class ReviewMarketService : IReviewMarketService
{
    private readonly IMarketRepository _marketRepository;
    private readonly IMarketAssociatedRepository _marketAssociatedRepository;
    private readonly IMarketRequestRepository _marketRequestRepository;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _environment;

    public ReviewMarketService(IMarketRepository marketRepository, IMarketAssociatedRepository marketAssociatedRepository, IEmailSender emailSender, IWebHostEnvironment environment
    , IMarketRequestRepository marketRequestRepository)
    {
        _emailSender = emailSender;
        _marketRepository = marketRepository;
        _marketAssociatedRepository = marketAssociatedRepository;
        _environment = environment;
        _marketRequestRepository = marketRequestRepository;
    }

    public async Task<ResultOperation> ApproveMarket(Guid marketId)
    {
        var result = await _marketRequestRepository.ApproveRequest(marketId);

        var ownerMarket = await _marketAssociatedRepository.GetMarketAssociatedByMarketID(marketId);

        if (ownerMarket.Success)
        {
            await ApprovedEmailToMarketOwner(marketId, ownerMarket.Data.Market, ownerMarket.Data.User);
        }

        return result;
    }
    public async Task<ResultOperation> RejectMarket(Guid marketId, string reason)
    {
        var result = await _marketRequestRepository.RejectRequest(marketId, reason);

        var ownerMarket = await _marketAssociatedRepository.GetMarketAssociatedByMarketID(marketId);

        if (ownerMarket.Success)
        {
                await RejectedEmailToMarketOwner(marketId, ownerMarket.Data.Market, ownerMarket.Data.User, reason);
            }

        return result;
    }

    public async Task<ResultOperation<IEnumerable<Market>>> GetApprovedMarketListAsync()
    {
        var result = await _marketRepository.GetApprovedMarketListAsync();

        return result;
    }

    public async Task<ResultOperation<IEnumerable<Market>>> GetRejectMarketListAsync()
    {
        var result = await _marketRepository.GetRejectMarketListAsync();

        return result;
    }


    public async Task<ResultOperation> RemoveMarket(Guid marketId, string reason)
    {
        var result = await _marketRepository.DeleteAsync(marketId);
        await _marketRequestRepository.DeleteAsyncMarket(marketId);

        var ownerMarket = await _marketAssociatedRepository.GetMarketAssociatedByMarketID(marketId);

        if (ownerMarket.Success)
        {
            await RemoveEmailToMarketOwner(marketId, ownerMarket.Data.Market, ownerMarket.Data.User, reason);
        }

        return result;
    }

    public async Task<ResultOperation> ApprovedEmailToMarketOwner(Guid marketId, Market market, User user)
    {
        string path = Path.Combine(_environment.ContentRootPath, "Email templates", "MarketApproved.html");

        string html = File.ReadAllText(path);
        html = html.Replace("{{userName}}", user.UserName).
            Replace("{{marketName}}", market.marketName).
            Replace("{{marketLocation}}", market.marketLocal).
            Replace("{{marketDescription}}", market.description);

        await _emailSender.SendEmailAsync(user.Email, "Seu mercado foi aprovado!", html);

        return ResultOperation.Ok();
    }

    public async Task<ResultOperation> RejectedEmailToMarketOwner(Guid marketId, Market market, User user, string reason)
    {
        string path = Path.Combine(_environment.ContentRootPath, "Email templates", "MarketRejected.html");

        string html = File.ReadAllText(path);
        html = html.Replace("{{userName}}", user.UserName).
            Replace("{{marketName}}", market.marketName).
            Replace("{{reason}}", reason);

        await _emailSender.SendEmailAsync(user.Email, "Seu mercado foi rejeitado!", html);

        return ResultOperation.Ok();
    }

    public async Task<ResultOperation> RemoveEmailToMarketOwner(Guid marketId, Market market, User user, string reason)
    {
        string path = Path.Combine(_environment.ContentRootPath, "Email templates", "MarketRemoved.html");

        string html = File.ReadAllText(path);
        html = html.Replace("{{userName}}", user.UserName).
            Replace("{{marketName}}", market.marketName).
            Replace("{{reason}}", reason);

        await _emailSender.SendEmailAsync(user.Email, "Seu mercado foi removido!", html);

        return ResultOperation.Ok();
    }

    public async Task<ResultOperation<IEnumerable<MarketRequestCreation>>> GetPendingMarketListAsync()
    {
        var result = await _marketRequestRepository.GetPendingRequests();

        return result;
    }

    public async Task<ResultOperation<Market>> GetMarketByID(Guid id)
    {
        var result = await _marketRepository.GetByIdAsync(id);

        return result;
    }

    public async Task<ResultOperation<MarketRequestCreation>> GetMarketRequestByID(Guid id)
    {
        var result = await _marketRequestRepository.GetMarketRequestByMarketID(id);

        return result;
    }
}