using DBModel;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Filter;

public class PageUserAssociated : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var userJson = context.HttpContext.Session.GetString("UserLoggedSession");
        

        if (string.IsNullOrEmpty(userJson))
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary{{"controller", "Login"}, {"action", "Index"}});
        }
        else
        {
            var user = JsonConvert.DeserializeObject<User>(userJson);

            if (user == null)
            {
                context.HttpContext.Session.Clear();
                context.Result = new RedirectToRouteResult(new RouteValueDictionary{{"controller", "Login"}, {"action", "Index"}});
            }
            else if (user.userType != Enums.UserType.Associado)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary{{"controller", "Home"}, {"action", "Index"}});
            }
        }


        base.OnActionExecuted(context);
    }
}