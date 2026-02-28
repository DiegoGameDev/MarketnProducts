using DBModel;
using Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Results;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Services;

public class LoginAppService : ILoginAppService
{
    private readonly IMarketSession _context;
    private readonly IUserRepository _DBContext;
    private readonly SignInManager<User> _Signin;
    private readonly IEmailSender _emailSender;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly IWebHostEnvironment _env;

    public LoginAppService(IMarketSession session, IUserRepository UserDB, SignInManager<User> signIn,
     IEmailSender emailSender, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor,
      IWebHostEnvironment env)
    {
        _context = session;
        _DBContext = UserDB;
        _Signin = signIn;
        _emailSender = emailSender;
        _urlHelperFactory = urlHelperFactory;
        _actionContextAccessor = actionContextAccessor;
        _env = env;
    }

    public async Task<ResultOperation> Login(string email, string password)
    {
        var user = await _DBContext.GetByLoginAsync(email);

        if (user == null)
            return ResultOperation.Fail("Usuário não encontrado.");

        var resultSignIn = await _Signin.PasswordSignInAsync(user.Data, password, false, false);
        if (!resultSignIn.Succeeded)
        {
            return ResultOperation.Fail("Credenciais inválidas.");
        }
        
        if (user.Data.EmailConfirmed == false)
        {
            await _Signin.SignOutAsync();
            return ResultOperation.Fail("Por favor, confirme seu email antes de fazer login.");
        }

        _context.EnterSession(user.Data);
        return ResultOperation.Ok("Login bem-sucedido.");
    }

    public async Task<ResultOperation> Register(User user, string password)
    {
        try
        {
            var create = await _DBContext.AddUserAsync(user, password);
            if (!create.Success)
                return new ResultOperation { Success = false, Message = create.Message };
            
            var token = await _DBContext.GenerateTokenAsync(create.Data);
            if (!token.Success)            return new ResultOperation { Success = false, Message = token.Message };

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var confirmationLink = urlHelper.Action("ConfirmEmail", "Login", new { userID = create.Data.Id, token = token.Data });

            string path = Path.Combine(_env.ContentRootPath, "EmailTemplates", "ConfirmEmail.html");

            await _emailSender.SendEmailAsync(user.Email, "Confirme seu email", $"Cliqui aqui para confirmar: {confirmationLink}");

            return ResultOperation.Ok("Usuário criado com sucesso! Por favor, verifique seu email para confirmar a conta.");
        }
        catch (Exception e) 
        {
            return ResultOperation.Fail("Ocorreu um erro ao criar a conta: " + e.Message);
            throw;
        }
        
    }

    public async Task<ResultOperation> VerifyEmail(string userID, string token)
    {
        var user = await _DBContext.GetByIdAsync(userID);
        if (user == null)
        {
            return ResultOperation.Fail("Usuário não encontrado.");
        }

        var result = await _DBContext.ConfirmedEmailAsync(user.Data, token);
        if (!result.Success)
        {
            return ResultOperation.Fail("Token inválido ou expirado.");
        }

        _context.EnterSession(user.Data);
        await _Signin.SignInAsync(user.Data, isPersistent: false);
        return ResultOperation.Ok("Email verificado com sucesso.");
    }

    public async Task<ResultOperation> Logout()
    {
        await _Signin.SignOutAsync();
        _context.ExitSession();
        return ResultOperation.Ok("Logout bem-sucedido.");
    }
}