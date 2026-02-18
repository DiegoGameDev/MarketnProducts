using DBModel;
using Newtonsoft.Json;

namespace Helper;

public class MarketSession : IMarketSession
{
    private readonly IHttpContextAccessor _context;
    public MarketSession(IHttpContextAccessor httpContext) => _context = httpContext;

    public void EnterSession(User user)
    {
        var userJson = JsonConvert.SerializeObject(user);

        _context.HttpContext.Session.SetString("UserLoggedSession", userJson);
    }

    public void ExitSession()
    {
        if (GetSession() == null)
            return;
        
        _context.HttpContext.Session.Clear();
    }

    public User GetSession()
    {
        var user = _context.HttpContext.Session.GetString("UserLoggedSession");

        if (string.IsNullOrEmpty(user))
            return default!;

        return JsonConvert.DeserializeObject<User>(user);
    }
}