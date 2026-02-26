using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ViewComponents;

public class Home : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        string session = HttpContext.Session.GetString("UserLoggedSession");

        if (string.IsNullOrEmpty(session))
            return null;

        DBModel.User client = JsonConvert.DeserializeObject<DBModel.User>(session);
        return View("Default", client);
    }
}