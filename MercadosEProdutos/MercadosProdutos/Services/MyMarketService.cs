using System.Reflection.Metadata;
using DBContext;
using DBModel;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Repository;
using Results;
using ViewModels;

namespace Services;

public class MyMarketService : IMyMarketService
{
    private MarketDBContext _context;
    private readonly IMarketRepository _marketRepository;
    private readonly IMarketAssociatedRepository _marketAssociatedRepository;
    private readonly IMarketRequestRepository _marketRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;

    private readonly IWebHostEnvironment _env;
    private readonly INotificationService _notificationService;

    public MyMarketService(
        IMarketRepository marketRepository,
        IMarketAssociatedRepository associatedRepository,
        IMarketRequestRepository requestRepository,
        IUserRepository userRepository,
        IEmailSender emailSender,
        MarketDBContext context,
        IWebHostEnvironment env,
        INotificationService notificationService)
    {
        _marketRepository = marketRepository;
        _marketAssociatedRepository = associatedRepository;
        _marketRequestRepository = requestRepository;
        _userRepository = userRepository;
        _emailSender = emailSender;
        _context = context;
        _notificationService = notificationService;
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

            #region Validação e upload de imagem
            if (market.image == null || market.image.Length == 0)
                return ResultOperation.Fail("Imagem obrigatória");

            var extension = Path.GetExtension(market.image.FileName).ToLower();
            var allowed = new[] { ".jpg", ".jpeg", ".png" };
            if (!allowed.Contains(extension))
                return ResultOperation.Fail("Formato inválido");
            
            var fileName = Guid.NewGuid().ToString() + extension;
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Resources/Market");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(directory, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await market.image.CopyToAsync(stream);
            }
            market.imagePath = "/Resources/Market/" + fileName;
            #endregion

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

            #region Validação e upload de imagem

            if (market.image != null && market.image.Length > 0)
            {
                var extension = Path.GetExtension(market.image.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" };
                if (!allowed.Contains(extension))
                    return ResultOperation.Fail("Formato inválido");
                
                var fileName = Guid.NewGuid().ToString() + extension;
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Resources/Market");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var fullPath = Path.Combine(directory, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await market.image.CopyToAsync(stream);
                }
                result.Data.imagePath = "/Resources/Market/" + fileName;
            }

            #endregion

            // update fields
            result.Data.marketName = market.marketName;
            result.Data.marketLocal = market.marketLocal;
            result.Data.description = market.description;
            result.Data.marketReviewStatus = Enums.MarketStatus.Approved;

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

    public async Task<ResultOperation> CreateProduct(Product model)
    {
        if (model == null)
            return ResultOperation.Fail("Produto inválido");
        
        try
        {
            if (model.image == null || model.image.Length == 0)
                return ResultOperation.Fail("Imagem obrigatória");

            var extension = Path.GetExtension(model.image.FileName).ToLower();
            var allowed = new[] { ".jpg", ".jpeg", ".png" };

            if (!allowed.Contains(extension))
                return ResultOperation.Fail("Formato inválido");

            var fileName = Guid.NewGuid().ToString() + extension;

            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Resources");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(directory, fileName);

            // salva imagem primeiro
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await model.image.CopyToAsync(stream);
            }

            model.imagePath = "/Resources/" + fileName;

            await _context.ProductList.AddAsync(model);
            await _context.SaveChangesAsync();

            return ResultOperation.Ok("Produto criado com sucesso");
        }
        catch (DbUpdateException ex)
        {
            return ResultOperation.Fail(ex.InnerException.Message);
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

            if (!string.IsNullOrEmpty(existing.imagePath)) // sobrescreve imagem existente
                using (var stream = new FileStream(existing.imagePath, FileMode.OpenOrCreate))
                {
                    await product.image.CopyToAsync(stream);
                }
            else // Cria nova imagem caso não tenha uma existente
            {
                if (product.image == null || product.image.Length == 0)
                    return ResultOperation.Fail("Imagem obrigatória");

                var extension = Path.GetExtension(product.image.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowed.Contains(extension))
                    return ResultOperation.Fail("Formato inválido");

                var fileName = Guid.NewGuid().ToString() + extension;

                var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Resources");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var fullPath = Path.Combine(directory, fileName);

                // salva imagem primeiro
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await product.image.CopyToAsync(stream);
                }

                existing.imagePath = "/Resources/" + fileName;
            }

            // update fields
            existing.productName = product.productName;
            existing.description = product.description;
            existing.productPrice = product.productPrice;

            await _context.SaveChangesAsync();

            return ResultOperation.Ok("Produto atualizado com sucesso");
        }
        catch (DbUpdateException ex)
        {
            return ResultOperation.Fail("Produto não foi atualizado: " + ex.InnerException.Message);
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

    public async Task<ResultOperation> SendRequestToDelete(Guid id, User user, string reason)
    {
        if (id == Guid.Empty)
            return ResultOperation.Fail("ID de mercado inválido");

        var market = await _marketRepository.GetByIdAsync(id);

        string path = Path.Combine(_env.ContentRootPath, "Email Templates", "MarketDeleteRequest.html");
        var html = await File.ReadAllTextAsync(path);
        html = html.Replace("{{marketName}}", market.Data.marketName).
            Replace("{{userName}}", user.UserName).
            Replace("{{reason}}", reason);

        var reviewers = await _userRepository.GetByUserType(Enums.UserType.Reviewer);

        try
        {
            if (reviewers.Success)
                foreach(var i in reviewers.Data)
                {
                    if (!string.IsNullOrEmpty(i.Email))
                    await _emailSender.SendEmailAsync(i.Email, "Solicitação para deletar mercado", htmlMessage: html);
                }

            
        Notification notification = new Notification()
        {
            UserID = user.Id,
            Title = "Solicatação de exclusão",
            Content = $"O usuario: {user.UserName} deseja excluir o mercado: {market.Data.marketName}. Motivo: {reason}",
            CreatedAt = DateTime.UtcNow
        };
        var resultNotifications = await _notificationService.NotifyReviewers(notification: notification);
        //Console.WriteLine(resultNotifications.Message);
        }
        catch (System.Exception ex)
        {
            return ResultOperation.Fail(ex.Message);
        }

        return ResultOperation.Ok("Solicitação enviada para os analisadores");
    }
}