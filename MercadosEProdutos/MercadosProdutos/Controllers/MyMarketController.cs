using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Filter;
using Helper;
using Repository;
using DBModel;
using AspNetCoreGeneratedDocument;

namespace MercadosProdutos.Controllers;

[Authorize(Roles = "Associated")]
public class MyMarketController : Controller
{
    private readonly IMarketSession _session;
    private readonly IMarketAssociatedRepository _context;
    private readonly IMarketRepository _marketContext;

    public MyMarketController(IMarketSession session, IMarketAssociatedRepository context, IMarketRepository marketContext)
    {
        _session = session;
        _context = context;
        _marketContext = marketContext;
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
    public IActionResult Create(Market modelForm)
    {
        return View(modelForm);
    }

    [HttpPost]
    public async Task<IActionResult> SendResquestMarketCreation(Market modelForm)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", modelForm);
        }

        var marketResultCreation = await _marketContext.AddAsync(modelForm);
        var user = _session.GetSession();

        //preparar o email

        return View("Create", modelForm);
    }
}