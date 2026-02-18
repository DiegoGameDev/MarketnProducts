using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MercadosProdutos.Models;
using Helper;
using Filter;
using Microsoft.AspNetCore.Authorization;

namespace MercadosProdutos.Controllers;

[PageUserConnected]
[Authorize(Roles = "Default")]
public class HomeController : Controller
{
    private readonly IMarketSession _session;

    public HomeController(IMarketSession marketSession)
    {
        _session = marketSession;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult MarketPlace()
    {
        return RedirectToAction("Index", "MarketPlace");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
