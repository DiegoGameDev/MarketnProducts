using MarketView;
using Filter;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace MercadosProdutos.Controllers;

[PageUserConnected]
public class MarketPlaceController : Controller
{
    private readonly IMarketRepository _context;

    public MarketPlaceController(IMarketRepository context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var marketDB = await _context.GetApprovedMarketListAsync();

        return View(marketDB.Data);
    }

    [HttpGet]
    public async Task<IActionResult> MarketView(Guid id)
    {
        var market = await _context.GetByIdAsync(id);

        if (!market.Success)
            return NotFound();

        var products = await _context.GetProductsFromMarket(market.Data);

        var mktView = new MarketViewModel()
        {
            Name = market.Data.marketName,
            Location = market.Data.marketLocal,
            description = market.Data.description,
        };

        foreach (var i in products.Data)
        {
            mktView.Products.Add(i.ToViewModel());
        }

        return View(mktView);
    }
}