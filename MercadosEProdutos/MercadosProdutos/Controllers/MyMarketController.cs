using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Helper;
using Repository;
using DBModel;
using Services;
using marketView = ViewModels.Market; // model market apenas para form

namespace MercadosProdutos.Controllers;

[Authorize(Roles = "Associated")]
public class MyMarketController : Controller
{
    private readonly IMarketSession _session;
    private readonly IMarketAssociatedRepository _context;
    private readonly IMarketRepository _marketContext;
    private readonly IMarketRequestService _service;

    public MyMarketController(IMarketSession session, IMarketAssociatedRepository context, IMarketRepository marketContext,
        IMarketRequestService service)
    {
        _session = session;
        _context = context;
        _marketContext = marketContext;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string marketID)
    {
        var User = _session.GetSession();

        var marketListResult = await _context.GetMarketListByUserId(User.Id);

        IEnumerable<Market> marketList = marketListResult.Data;

        return View(marketList);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var resultSearch = await _marketContext.GetByIdAsync(id);

        return View(resultSearch.Data);
    }
    [HttpGet]
    public IActionResult Create(marketView modelForm)
    {
        return View(modelForm);
    }

    [HttpPost]
    public async Task<IActionResult> SendResquestMarketCreation(marketView modelForm)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErroMSG"] = "Dados invalidos";
            return View("Create", modelForm);
        }

        var user = _session.GetSession();

        await _service.CreateMarketWithRequest(modelForm.ToModel(), user);

        return View("Create", modelForm);
    }
}