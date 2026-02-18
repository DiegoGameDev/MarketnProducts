using DBModel;

namespace Helper;

public interface IMarketSession
{
    public User GetSession();
    public void EnterSession(User user);
    public void ExitSession();
}