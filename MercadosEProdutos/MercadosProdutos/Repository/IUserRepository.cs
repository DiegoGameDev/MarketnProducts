using DBContext;
using DBModel;
using Enums;
using Results;

namespace Repository;

public interface IUserRepository
{
    Task<ResultOperation> UserExists(string id);
    Task<ResultOperation<User>> GetByIdAsync(string id);
    Task<ResultOperation<IEnumerable<User>>> GetByUserType(UserType userType);
    Task<ResultOperation<User>> GetByLoginAsync(string email);
    Task<ResultOperation<List<User>>> GetAllAsync();
    Task<ResultOperation<string>> GenerateTokenAsync(User user);
    Task<ResultOperation<User>> AddUserAsync(User user, string password);
    Task<ResultOperation<bool>> AddRoleAsync(User user, string role);

    Task<ResultOperation<bool>> UpdateAsync(User user);
    Task<ResultOperation<User>> ConfirmedEmailAsync(User user, string token);
    Task<ResultOperation<bool>> DeleteAsync(int id);

    Task<ResultOperation<User>> VerifyUserLogin(string login, string password) ;
}
