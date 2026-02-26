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

    public async Task<ResultOperation<IEnumerable<Market>>> GetMarketsByUserApproved(string userId)
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
            market.marketReviewStatus = Enums.MarketStatus.Pending; 
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
                    await _emailSender.SendEmailAsync(i.Email, "Nova solicitação", htmlMessage: htmlBodyMessage);
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
        if (market == null)
            return ResultOperation.Fail("Mercado inválido");

        try
        {
            var result = await _marketRepository.GetByIdAsync(market.ID);

            if (result == null)
                return ResultOperation.Fail("Mercado não encontrado");

            // update fields
            result.Data.marketName = market.marketName;
            result.Data.marketLocal = market.marketLocal;
            result.Data.description = market.description;
            result.Data.marketReviewStatus = market.marketReviewStatus;

            await _marketRepository.UpdateAsync(result.Data);
            await _context.SaveChangesAsync();

            return ResultOperation.Ok("Mercado atualizado com sucesso");
        }
        catch
        {
            return ResultOperation.Fail("Mercado não foi atualizado");
        }
    }

    public async Task<ResultOperation<ICollection<Product>>> GetProductsInMarket(Guid id)
    {
        var resultProducts = await _context.ProductList.Where(x => x.MarketID == id).AsNoTracking().ToListAsync();
     
        if (resultProducts != null)
            return ResultOperation<ICollection<Product>>.Ok(resultProducts, "Produtos encontrados");
        else
            return ResultOperation<ICollection<Product>>.Fail("Produtos não foram encontrados");
    }

    public async Task<ResultOperation> CreateProduct(Product product)
    {
        if (product == null)
            return ResultOperation.Fail("Produto inválido");

        try
        {
            await _context.ProductList.AddAsync(product);
            await _context.SaveChangesAsync();

            return ResultOperation.Ok("Produto criado com sucesso");
        }
        catch (Exception ex)
        {
            return ResultOperation.Fail(ex.Message);
        }
    }

    public async Task<ResultOperation> EditProduct(Product product)
    {
        if (product == null)
            return ResultOperation.Fail("Produto inválido");

        try
        {
            var existing = await _context.ProductList
                .FirstOrDefaultAsync(p => p.ID == product.ID && p.MarketID == product.MarketID);

            if (existing == null)
                return ResultOperation.Fail("Produto não encontrado");

            // update fields
            existing.productName = product.productName;
            existing.description = product.description;
            existing.productPrice = product.productPrice;

            await _context.SaveChangesAsync();

            return ResultOperation.Ok("Produto atualizado com sucesso");
        }
        catch
        {
            return ResultOperation.Fail("Produto não foi atualizado");
        }
    }

    public async Task<ResultOperation> DeleteProduct(Guid marketId, int productID)
    {
        try
        {
            var existing = await _context.ProductList
                .FirstOrDefaultAsync(p => p.ID == productID && p.MarketID == marketId);

            if (existing == null)
                return ResultOperation.Fail("Produto não encontrado");

            _context.ProductList.Remove(existing);
            await _context.SaveChangesAsync();

            return ResultOperation.Ok("Produto excluído com sucesso");
        }
        catch (Exception ex)
        {
            return ResultOperation.Fail(ex.Message);
        }
    }

    public async Task<ResultOperation<Product>> GetProductById(int id)
    {
        var product = await _context.ProductList.FirstOrDefaultAsync(p => p.ID == id);

        if (product != null)
            return ResultOperation<Product>.Ok(product, "Produto encontrado");
        else
            return ResultOperation<Product>.Fail("Produto não encontrado");
    }
}