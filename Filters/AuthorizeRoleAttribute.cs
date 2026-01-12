using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DahiliaCreations.Filters;

public class AuthorizeRoleAttribute : ActionFilterAttribute
{
    private readonly string _role;

    public AuthorizeRoleAttribute(string role)
    {
        _role = role;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var role = context.HttpContext.Session.GetString("UserRole");

        if (role != _role)
        {
            context.Result = new RedirectToActionResult("Login", "User", null);
        }
    }
}

