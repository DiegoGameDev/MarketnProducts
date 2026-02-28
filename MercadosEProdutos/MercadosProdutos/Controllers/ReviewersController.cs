using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Filter;
using Services;
using DBModel;
using Enums;

namespace MercadosProdutos.Controllers;

[Authorize(Roles = "Reviewer")]
public class ReviewersController : Controller
{
    private readonly IReviewMarketService _service;

    public ReviewersController(IReviewMarketService reviewMarketService)
    {
        _service = reviewMarketService;
    }

    public async Task<IActionResult> Index()
    {
        return RedirectToAction("PendingList");
    }

    #region Listas 
    [HttpGet]
    public async Task<IActionResult> PendingList()
    {
        var request = await _service.GetPendingMarketListAsync();
        
        return View(request.Data);
    }

    [HttpGet]
    public async Task<IActionResult> ApprovedList()
    {
        var request = await _service.GetApprovedMarketListAsync();

        return View(request.Data);
    }

    [HttpGet]
    public async Task<IActionResult> RejectedList()
    {
        var request = await _service.GetRejectMarketListAsync();

        return View(request.Data);
    }
    #endregion

    #region Rejeitar e deletar Get

    [HttpGet]
    public async Task<IActionResult> RejectMarket(Guid id)
    {
        var request = await _service.GetMarketRequestByID(id);

        return View(request.Data);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteMarket(Guid id)
    {
        var request = await _service.GetMarketRequestByID(id);

        request.Data.Status = MarketStatus.Rejected;

        return View(request.Data);
    }
    #endregion

    #region Post Methods

    [HttpPost]
    public async Task<IActionResult> ConfirmDelete(MarketRequestCreation marketRequestCreation)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            TempData["ErroMSG"] = "O mercado não foi removido.";
            return RedirectToAction("DeleteMarket", new { id = marketRequestCreation.MarketId });
        }

        await _service.RemoveMarket(marketRequestCreation.MarketId, marketRequestCreation.RejectionReason);
        TempData["SucessoMSG"] = "O mercado foi removido com sucesso.";
        return RedirectToAction("PendingList");
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmReject(MarketRequestCreation marketRequestCreation)
    {
        var result = await _service.RejectMarket(marketRequestCreation.MarketId, marketRequestCreation.RejectionReason);

        if (result.Success)
        {
            TempData["SucessoMSG"] = "O mercado foi rejeitado com sucesso.";
        }
        else
        {
            TempData["ErroMSG"] = $"O mercado não foi rejeitado: {result.Message}";
        }
        return RedirectToAction("RejectedList");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveMarket(Guid id)
    {
        var result = await _service.ApproveMarket(id);

        if (result.Success)
        {
            TempData["SucessoMSG"] = "O mercado foi aprovado com sucesso";
            return RedirectToAction("ApprovedList");
        }
        
        TempData["ErroMSG"] = $"O mercado não foi aprovado: {result.Message}";
        return RedirectToAction("PendingList");
    }

    #endregion
}