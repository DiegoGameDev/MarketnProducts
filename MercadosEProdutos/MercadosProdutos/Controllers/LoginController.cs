using ResultOperation;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Model;
using DBModel;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MercadosProdutos.Controllers;

public class LoginController : Controller
{
    private readonly IMarketSession _context;
    private readonly IUserRepository _DBContext;
    private readonly SignInManager<User> _Signin;
    private readonly IEmailSender _emailSender;

    public LoginController(IMarketSession session, IUserRepository UserDB, SignInManager<User> signIn, IEmailSender emailSender)
    {
        _context = session;
        _DBContext = UserDB;
        _Signin = signIn;
        _emailSender = emailSender;
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

        var resultCreation = await _DBContext.AddUserAsync(user, userRegister.password);

        if (!resultCreation.Success)
            return RedirectToAction("Register", "Login", userRegister);

        user = resultCreation.Data;
    
        ResultOperation<string> token = await _DBContext.GenerateTokenAsync(user);

        // criação do link
        if (!token.Success)
            return RedirectToAction("Register", "Login", userRegister);

        var confirmationLink = Url.Action("ConfirmEmail",
            "Login",
            new { userID = user.Id, token = token.Data},
            Request.Scheme
        );

        await _emailSender.SendEmailAsync(user.Email, "Confirme seu email", $"Cliqui aqui para confirmar: {confirmationLink}");

        if (!resultCreation.Success)
        {
            TempData["ErroMSG"] = resultCreation.Message;
            return RedirectToAction("Register", "Login", userRegister);
        }
        
        return RedirectToAction("Index", "Login");
    }

    [HttpGet]
    //Este metodo será chamado quando o usuario confirmar seu email
    //Fica responsável por atualizar o usuario no banco e atiavr a conta
    public async Task<IActionResult> ConfirmEmail(string userID, string token)
    {
        var user = await _DBContext.GetByIdAsync(userID);

        if (!user.Success)
            return RedirectToAction("Index", "Login");

        var resultConfirmEmail = await _DBContext.ConfirmedEmailAsync(user.Data, token);

        if (!resultConfirmEmail.Success)
        {
            TempData["ErroMSG"] = resultConfirmEmail.Message;
            return RedirectToAction("Index", "Login");
        }

        TempData["SucessoMSG"] = resultConfirmEmail.Message;
        _context.EnterSession(user.Data);
        await _Signin.SignInAsync(user.Data, isPersistent: false);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Connect(LoginModel userLogin)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErroMSG"] = "Os campos não foram preenchidos corretamente";
            return View("Index", userLogin);
        }
        
        var user = await _DBContext.GetByLoginAsync(userLogin.login);

        if (user == null)
            return View("Index", userLogin);

        var resultSearch =  await _Signin.PasswordSignInAsync(user.Data, userLogin.password, isPersistent: false, lockoutOnFailure: false);
        

        try
        {
            if (!resultSearch.Succeeded)
                {
                    TempData["ErroMSG"] = "Credenciais inválidas";
                    return View("Index", userLogin);
                }

            _context.EnterSession(user.Data);
            await _Signin.SignInAsync(user.Data, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }
        catch (DbUpdateException error)
        {
            TempData["ErroMSG"] = "Erro no login" + $" : {error.Message}";
            return View("Index", userLogin);
        }
    }
}