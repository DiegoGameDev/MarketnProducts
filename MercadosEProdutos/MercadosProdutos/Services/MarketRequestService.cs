using DBModel;
using Microsoft.AspNetCore.Identity.UI.Services;
using Repository;
using Results;

namespace Services;

public class MarketRequestService : IMarketRequestService
{
    private readonly IMarketRepository _marketRepository;
    private readonly IMarketRequestRepository _marketRequestRepository;
    private readonly IMarketAssociatedRepository _marketAssociatedRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;

    public MarketRequestService(
        IMarketRepository marketRepository,
        IMarketRequestRepository marketRequestRepository,
        IUserRepository userRepository,
        IEmailSender emailSender,
        IMarketAssociatedRepository marketAssociatedRepository)
    {
        _marketRepository = marketRepository;
        _marketRequestRepository = marketRequestRepository;
        _userRepository = userRepository;
        _emailSender = emailSender;
        _marketAssociatedRepository = marketAssociatedRepository;
    }

    public async Task<ResultOperation> CreateMarketWithRequest(Market market, User user)
    {
        // 1. Criar mercado
        var marketResult = await _marketRepository.AddAsync(market);

        if (!marketResult.Success)
            return ResultOperation.Fail("Erro ao criar mercado");

        // Cria o objeto que liga usuario a uma loja
        await _marketAssociatedRepository.AddUserInMarket(user, marketResult.Data);
        // 2. Criar solicitação de aprovação
        var request = new MarketRequestCreation
        {
            MarketId = marketResult.Data.ID
        };

        var requestResult = await _marketRequestRepository.CreateRequest(request);

        if (!requestResult.Success)
            return ResultOperation.Fail("Erro ao criar solicitação");

        // 3. (Opcional) Buscar reviewers
        var reviewers = await _userRepository.GetByUserType(Enums.UserType.Reviewer);

        // Aqui você pode futuramente:
        var htmlBodyMessage = await File.ReadAllTextAsync(@"C:\Users\woody\OneDrive\Documentos\Projetos Programação\Estudos\MercadosEProdutos\MercadosProdutos\Email templates\MarketRequest.html");
        
        htmlBodyMessage = htmlBodyMessage.Replace("{{marketName}}", market.marketName).
            Replace("{{marketLocation}}", market.marketLocal).
            Replace("{{userName}}", "Cabaço").
            Replace("{{reviewLink}}", "youtube.com").
            Replace("{{description}}", market.description);

        if (reviewers.Success)
            foreach(var i in reviewers.Data)
            {
                await _emailSender.SendEmailAsync(i.Email?.ToLower(), "Nova solicitação", htmlMessage: htmlBodyMessage);
            }

        return ResultOperation.Ok("Mercado enviado para aprovação");
    }
}