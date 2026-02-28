using DBModel;
using Results;

namespace Services;

public interface ILoginAppService
{
    Task<ResultOperation> Register(User user, string password);
    Task<ResultOperation> Login(string email, string password);
    Task<ResultOperation> VerifyEmail(string userID, string token);
    Task<ResultOperation> Logout();
}