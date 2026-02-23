using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Helper;
using Services;
using marketView = ViewModels.Market;
using Filter;
using DBModel; // model market apenas para form

namespace MercadosProdutos.Controllers;

[PageUserConnected]
[Authorize(Roles = "Default")]
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

        var result = await _service.GetMarketsByUserApproved(user.Id);

        return View(result.Data);
    }

    #region MarketView

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var resultSearch = await _service.GetMarketById(id);
        var resultProducts = await _service.GetProductsInMarket(id);
        resultSearch.Data.ProductInMarketList = resultProducts.Data;

        if (resultSearch.Data.marketReviewStatus == Enums.MarketReviewStatus.Pending)
        {
            TempData["InfoMSG"] = "Sua solicitação de criação de mercado ainda está pendente de aprovação. Você pode editar as informações, mas elas só serão atualizadas após a aprovação.";
            return View("Index");
        }
         else if (resultSearch.Data.marketReviewStatus == Enums.MarketReviewStatus.Rejected)
        {
            TempData["ErroMSG"] = "Sua solicitação de criação de mercado foi rejeitada. Por favor, revise as informações e envie novamente para aprovação.";
            return View("Index");
        }

        return View(resultSearch.Data);
    }
    [HttpGet]
    public IActionResult Create(marketView modelForm)
    {
        return View(modelForm);
    }
    #endregion

    #region  MarketPostMethods

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

    [HttpPost]
    public async Task<IActionResult> UpdateMarket(Market modelForm)
    {
        if (!ModelState.IsValid)
        {
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Erro na chave '{key}': {error.ErrorMessage}");
                }
            }

            return View("Edit", modelForm);
        }

        modelForm.marketReviewStatus = Enums.MarketReviewStatus.Approved;
        var result = await _service.UpdateMarket(modelForm);

        if (!result.Success)
        {
            TempData["ErroMSG"] = result.Message;
            return View("Edit", modelForm);
        }

        TempData["SucessoMSG"] = result.Message;
        return RedirectToAction("Edit", modelForm);
    }

    #endregion

    #region ProductsView
    [HttpGet]
    public async Task<IActionResult> CreateProduct(Guid marketId)
    {
        return View("CreateProduct",new Product(){MarketID = marketId});
    }

    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        var result = await _service.GetProductById(id);

        return View("EditProduct", result.Data);
    }

    [HttpGet]
    public IActionResult DeleteProduct(int id)
    {
        var result = _service.GetProductById(id).Result;

        return View("DeleteProduct", result.Data);
    }
    #endregion

    #region ProductsPostMethods

    [HttpPost]
    public async Task<IActionResult> CreateProductConfirm(Product model)
    {
        var result = await _service.CreateProduct(model);

        if (!result.Success)
        {
            TempData["ErroMSG"] = result.Message;
            return RedirectToAction("CreateProduct", new { marketId = model.MarketID });
        }

        TempData["SucessoMSG"] = result.Message;
        return RedirectToAction("Edit", new { id = model.MarketID });
    }

    [HttpPut]
    public async Task<IActionResult> EditProductConfirm(Product model)
    {
        var result = await _service.EditProduct(model);

        if (!result.Success)
        {
            TempData["ErroMSG"] = result.Message;
            return RedirectToAction("EditProduct", model);
        }

        TempData["SucessoMSG"] = result.Message;
        return RedirectToAction("Edit", new { id = model.MarketID });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProductConfirm(Product model)
    {
        var result = await _service.DeleteProduct(model.MarketID, model.ID);

        if (!result.Success)
        {
            TempData["ErroMSG"] = result.Message;
            return RedirectToAction("DeleteProduct", model);
        }

        TempData["SucessoMSG"] = result.Message;
        return RedirectToAction("Edit", new { id = model.MarketID });
    }

    #endregion
}