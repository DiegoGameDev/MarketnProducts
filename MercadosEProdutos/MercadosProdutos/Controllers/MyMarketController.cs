using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Helper;
using Services;
using marketView = ViewModels.Market; // model market apenas para form

namespace MercadosProdutos.Controllers;

[Authorize(Roles = "Associated")]
public class MyMarketController : Controller
{
    private readonly IMarketSession _session;
    private readonly IMarketAppService _service;

    public MyMarketController(IMarketSession session, IMarketAppService service)
    {
        _session = session;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = _session.GetSession();

        var result = await _service.GetMarketsByUser(user.Id);

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var resultSearch = await _service.GetMarketById(id);
        var resultProducts = await _service.GetProductsInMarket(id);
        resultSearch.Data.ProductInMarketList = resultProducts.Data;

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
            return View("Create", modelForm);

        var user = _session.GetSession();

        var result = await _service.CreateMarketWithRequest(modelForm.ToModel(), user);

        if (!result.Success)
        {
            TempData["ErroMSG"] = result.Message;
            return View("Create", modelForm);
        }

        TempData["SucessoMSG"] = result.Message;
        return RedirectToAction("Index");
    }

    public IActionResult CreateProduct(Guid marketId)
    {
        return NotFound();
    }

    public IActionResult EditProduct()
    {
        return NotFound();
    }

    public IActionResult DeleteProduct()
    {
        return NotFound();
    }
}