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
    private readonly INotificationService _notificationService;

    public ReviewMarketService(IMarketRepository marketRepository, IMarketAssociatedRepository marketAssociatedRepository, IEmailSender emailSender, IWebHostEnvironment environment
    , IMarketRequestRepository marketRequestRepository,
    INotificationService notificationService)
    {
        _emailSender = emailSender;
        _marketRepository = marketRepository;
        _marketAssociatedRepository = marketAssociatedRepository;
        _environment = environment;
        _marketRequestRepository = marketRequestRepository;
        _notificationService = notificationService;
    }

    public async Task<ResultOperation> ApproveMarket(Guid marketId)
    {
        var result = await _marketRequestRepository.ApproveRequest(marketId);

        var ownerMarket = await _marketAssociatedRepository.GetMarketAssociatedByMarketID(marketId);

        if (ownerMarket.Success)
        {
            await ApprovedEmailToMarketOwner(ownerMarket.Data.Market, ownerMarket.Data.User);

            Alert notificationObject = new Alert
            {
                UserID = "System",
                TargetID = ownerMarket.Data.UserID,
                Title = "Mercado Aprovado",
                Message = $"O seu mercado {ownerMarket.Data.Market.marketName} foi aprovado! Você já pode começar a cadastrar seus produtos e gerenciar seu mercado.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            await _notificationService.NotifyUser(notificationObject);
        }

        return result;
    }
    public async Task<ResultOperation> RejectMarket(Guid marketId, string reason)
    {
        var result = await _marketRequestRepository.RejectRequest(marketId, reason);
        Console.WriteLine(result.Message);

        var ownerMarket = await _marketAssociatedRepository.GetMarketAssociatedByMarketID(marketId);

        if (ownerMarket.Success && result.Success)
        {
            await RejectedEmailToMarketOwner(ownerMarket.Data.Market, ownerMarket.Data.User, reason);

            Alert notificationObject = new Alert
            {
                UserID = "System",
                TargetID = ownerMarket.Data.UserID,
                Title = "Mercado Rejeitado",
                Message = $"O seu mercado {ownerMarket.Data.Market.marketName} foi rejeitado. Motivo: {reason}",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            await _notificationService.NotifyUser(notificationObject);
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
        var result = await _marketRequestRepository.GetRejectRequests();

        return result;
    }


    public async Task<ResultOperation> RemoveMarket(Guid marketId, string reason)
    {
        var ownerMarket = await _marketAssociatedRepository.GetMarketAssociatedByMarketID(marketId);
        var result = await _marketRepository.DeleteAsync(marketId);


        if (ownerMarket.Success)
        {
            await RemoveEmailToMarketOwner(ownerMarket.Data.Market, ownerMarket.Data.User, reason);

            Alert notificationObject = new Alert
            {
                UserID = "System",
                TargetID = ownerMarket.Data.UserID,
                Title = "Mercado Removido",
                Message = $"O seu mercado {ownerMarket.Data.Market.marketName} foi removido. Motivo: {reason}",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            await _notificationService.NotifyUser(notificationObject);
        }

        return result;
    }

    public async Task<ResultOperation> ApprovedEmailToMarketOwner(Market market, User user)
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

    public async Task<ResultOperation> RejectedEmailToMarketOwner(Market market, User user, string reason)
    {
        string path = Path.Combine(_environment.ContentRootPath, "Email templates", "MarketRejected.html");

        string html = File.ReadAllText(path);
        html = html.Replace("{{userName}}", user.UserName).
            Replace("{{marketName}}", market.marketName).
            Replace("{{reason}}", reason);

        await _emailSender.SendEmailAsync(user.Email, "Seu mercado foi rejeitado!", html);

        return ResultOperation.Ok();
    }

    public async Task<ResultOperation> RemoveEmailToMarketOwner(Market market, User user, string reason)
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