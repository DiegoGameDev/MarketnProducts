using System.Reflection.Metadata;
using DBContext;
using DBModel;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using Results;

namespace Services;

public class MarketAppService : IMarketAppService
{
    private MarketDBContext _context;
    private readonly IMarketRepository _marketRepository;
    private readonly IMarketAssociatedRepository _marketAssociatedRepository;
    private readonly IMarketRequestRepository _marketRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;

    private readonly IWebHostEnvironment _env;

    public MarketAppService(
        IMarketRepository marketRepository,
        IMarketAssociatedRepository associatedRepository,
        IMarketRequestRepository requestRepository,
        IUserRepository userRepository,
        IEmailSender emailSender,
        MarketDBContext context,
        IWebHostEnvironment env)
    {
        _marketRepository = marketRepository;
        _marketAssociatedRepository = associatedRepository;
        _marketRequestRepository = requestRepository;
        _userRepository = userRepository;
        _emailSender = emailSender;
        _context = context;
        _env = env;
    }

    public async Task<ResultOperation<IEnumerable<Market>>> GetMarketsByUser(string userId)
    {
        return await _marketAssociatedRepository.GetMarketListByUserId(userId);
    }

    public async Task<ResultOperation<Market>> GetMarketById(Guid id)
    {
        return await _marketRepository.GetByIdAsync(id);
    }

    public async Task<ResultOperation> CreateMarketWithRequest(Market market, User user)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Criar mercado
            var marketResult = await _marketRepository.AddAsync(market);

            if (!marketResult.Success)
            {
                await transaction.RollbackAsync();
                return ResultOperation.Fail("Erro ao criar mercado");
            }

            // Cria o objeto que liga usuario a uma loja
            await _marketAssociatedRepository.AddUserInMarket(user, marketResult.Data);
            // 2. Criar solicitação de aprovação
            var request = new MarketRequestCreation
            {
                MarketId = marketResult.Data.ID
            };

            var requestResult = await _marketRequestRepository.CreateRequest(request);

            if (!requestResult.Success)
            {await transaction.RollbackAsync(); return ResultOperation.Fail("Erro ao criar solicitação");}

            // 3. (Opcional) Buscar reviewers
            var reviewers = await _userRepository.GetByUserType(Enums.UserType.Reviewer);

            // Aqui você pode futuramente:
            string path = Path.Combine(_env.ContentRootPath, "Email Templates", "MarketRequest.html");
            var htmlBodyMessage = await File.ReadAllTextAsync(path);
            
            htmlBodyMessage = htmlBodyMessage.Replace("{{marketName}}", market.marketName).
                Replace("{{marketLocation}}", market.marketLocal).
                Replace("{{userName}}", user.UserName).
                Replace("{{reviewLink}}", "youtube.com").
                Replace("{{description}}", market.description);
            try
            {
                if (reviewers.Success)
                foreach(var i in reviewers.Data)
                {
                    if (!string.IsNullOrEmpty(i.Email))
                    await _emailSender.SendEmailAsync(i.Email.ToLower(), "Nova solicitação", htmlMessage: htmlBodyMessage);
                }
            }
            catch (System.Exception)
            {
            }
            

            await transaction.CommitAsync();

            return ResultOperation.Ok("Mercado enviado para aprovação");
        }
        catch (System.Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        
    }

    public async Task<ResultOperation> UpdateMarket(Market market)
    {
        return await _marketRepository.UpdateAsync(market);
    }

    public async Task<ResultOperation<ICollection<Product>>> GetProductsInMarket(Guid id)
    {
        var resultProducts = await _context.ProductList.Where(x => x.MarketID == id).AsNoTracking().ToListAsync();
     
        if (resultProducts != null)
            return ResultOperation<ICollection<Product>>.Ok(resultProducts, "Produtos encontrados");
        else
            return ResultOperation<ICollection<Product>>.Fail("Produtos não foram encontrados");
    }
}