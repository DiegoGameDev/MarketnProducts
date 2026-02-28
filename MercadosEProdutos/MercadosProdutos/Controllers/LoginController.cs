using Microsoft.AspNetCore.Mvc;
using Model;
using Services;

namespace MercadosProdutos.Controllers;

public class LoginController : Controller
{

    private readonly ILoginAppService _service;

    public LoginController(ILoginAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Index(LoginModel model)
    {
        return View(model);
    }

    public IActionResult Register(RegisterModel model)
    {
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAccount(RegisterModel userRegister)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErroMSG"] = "Os campos não foram preenchidos corretamente";
            return RedirectToAction("Register", "Login", userRegister);
        }

        var user = userRegister.ToUser();

        var result = await _service.Register(user, userRegister.password);
        if (!result.Success)
        {
            TempData["ErroMSG"] = result.Message;
            return RedirectToAction("Register", "Login", userRegister);
        }

        TempData["SucessoMSG"] = result.Message;
        return RedirectToAction("Index", "Login", new LoginModel { login = user.Email });
    }

    [HttpGet]
    //Este metodo será chamado quando o usuario confirmar seu email
    //Fica responsável por atualizar o usuario no banco e atiavr a conta
    public async Task<IActionResult> ConfirmEmail(string userID, string token)
    {
        var result = await _service.VerifyEmail(userID, token);
        if (!result.Success)
        {
            TempData["ErroMSG"] = result.Message;
            return RedirectToAction("Index", "Login");
        }
    
        TempData["SucessoMSG"] = result.Message;
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        var result = await _service.Logout();
        if (!result.Success)
        {
            TempData["ErroMSG"] = result.Message;
            return RedirectToAction("Index", "Login");
        }

        TempData["SucessoMSG"] = result.Message;
        return RedirectToAction("Index", "Login");
    }

    [HttpPost]
    public async Task<IActionResult> Connect(LoginModel userLogin)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErroMSG"] = "Os campos não foram preenchidos corretamente";
            return View("Index", userLogin);
        }
        
        var result = await _service.Login(userLogin.login, userLogin.password);
        if (!result.Success)        {
            TempData["ErroMSG"] = result.Message;
            return View("Index", userLogin);
        }

        return RedirectToAction("Index", "MarketPlace");
    }
}