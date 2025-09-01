using CrudASP.NET.Models;
using System;
using System.Web;
using System.Web.Mvc;

public class PermissionAttribute : AuthorizeAttribute
{
    private readonly string _module;
    private readonly string _action;

    public PermissionAttribute(string module, string action)
    {
        _module = module;
        _action = action;
    }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        return AuthHelper.UserHasPermission(_module, _action);
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        // Redirigir a vista de acceso denegado
        filterContext.Result = new RedirectResult("~/Home/AccessDenied");

    }
}
