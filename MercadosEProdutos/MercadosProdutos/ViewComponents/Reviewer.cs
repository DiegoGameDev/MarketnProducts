using Microsoft.AspNetCore.Mvc;
using DBModel;
using Newtonsoft.Json;

namespace ViewComponents;

public class Reviewer : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userString = HttpContext.Session.GetString("UserLoggedSession");

        if (string.IsNullOrEmpty(userString))
            return null;

        User user = JsonConvert.DeserializeObject<User>(userString);
        return View("Default", user);
    }
}