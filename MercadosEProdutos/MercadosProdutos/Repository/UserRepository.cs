using DBContext;
using DBModel;
using Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Results;


namespace Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ResultOperation<string>> GenerateTokenAsync(User user)
    {
        if (user == null)
            return ResultOperation<string>.Fail("Usuário é nulo");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        return ResultOperation<string>.Ok(token, "Token Gerado");
    }

    public async Task<ResultOperation<User>> AddUserAsync(User user, string password)
    {
        if (user == null) return ResultOperation<User>.Fail("Usuario nulo");

        var result = await _userManager.CreateAsync(user, password);
    
        if (!result.Succeeded)
        {
            return ResultOperation<User>.Fail("Falha na criação do usuario: " + $"{result.Errors.ElementAt(0).Description}");
        }

        var userResult = await GetByLoginAsync(user.Email);

        return ResultOperation<User>.Ok(user);
    }

    public async Task<ResultOperation<bool>> AddRoleAsync(User user, string role)
    {
        if (user == null)
            return ResultOperation<bool>.Fail("Usuário é nulo");

        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
            return ResultOperation<bool>.Fail("Não foi possível adicionar a role");

        return ResultOperation<bool>.Ok(true, "Role adicionada com sucesso");
    }

    public async Task<ResultOperation<List<User>>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        return ResultOperation<List<User>>.Ok(users);
    }

    public async Task<ResultOperation<User>> GetByLoginAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return ResultOperation<User>.Fail("Usuário não encontrado");

        return ResultOperation<User>.Ok(user);
    }

    public async Task<ResultOperation<User>> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return ResultOperation<User>.Fail("Usuário não encontrado");

        return ResultOperation<User>.Ok(user);
    }

    public async Task<ResultOperation<bool>> UpdateAsync(User user)
    {
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return ResultOperation<bool>.Fail("Não foi possível atualizar");

        return ResultOperation<bool>.Ok(true, "Atualizado com sucesso");
    }

    public async Task<ResultOperation<bool>> DeleteAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return ResultOperation<bool>.Fail("Usuário não encontrado");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            return ResultOperation<bool>.Fail("Erro ao deletar usuário");

        return ResultOperation<bool>.Ok(true, "Usuário deletado");
    }

    public async Task<ResultOperation<User>> VerifyUserLogin(string login, string password)
    {
        var user = await _userManager.Users.FirstAsync(x => x.NormalizedEmail == login.ToUpper());

        if (user == null)
            return ResultOperation<User>.Fail("Não existe usuário com esse email");

        var resultCheckPassword = await _userManager.CheckPasswordAsync(user, password);

        if (resultCheckPassword)
            return ResultOperation<User>.Ok(user, "Usuário encontrado com sucesso");

        return ResultOperation<User>.Fail("Senha inválida");
    }

    public async Task<ResultOperation<User>> ConfirmedEmailAsync(User user, string token)
    {
        if (user == null)
            return ResultOperation<User>.Fail("Usuário não encontrado");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return ResultOperation<User>.Fail("Link expirado ou já foi utilizado. Se seu email não foi confirmado, tente fazer login novamente para receber outro link de confirmação");

        await _userManager.AddToRoleAsync(user, "Default");

        return ResultOperation<User>.Ok(user, "Email confirmado com sucesso");
    }

    public async Task<ResultOperation> UserExists(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return ResultOperation.Fail("Usuário não encontrado");

        return ResultOperation.Ok("Usuario existe");
    }

    public async Task<ResultOperation<IEnumerable<User>>> GetByUserType(UserType userType)
    {
        IEnumerable<User> users = await _userManager.Users.Where(x => x.userType == userType).ToListAsync();

        return ResultOperation<IEnumerable<User>>.Ok(users, $"{users.Count()} Usuarios do tipo: {userType} foram encontrados");
    }
}