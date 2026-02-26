using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Filter;
using Services;

namespace MercadosProdutos.Controllers;

[PageUserAssociated]
[Authorize(Roles = "Reviewer")]
public class ReviewersController : Controller
{
    private readonly IReviewMarketService _reviewMarketService;

    public ReviewersController(IReviewMarketService reviewMarketService)
    {
        _reviewMarketService = reviewMarketService;
    }

    public IActionResult Index()
    {
        return View();
    }
}