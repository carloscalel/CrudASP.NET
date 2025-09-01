using CrudASP.NET.Models; // namespace real de tus entidades
using System;
using System.Web;
using System.Web.Mvc;

public class LogRequestAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // Guarda controller/action para que SaveChanges() también los pueda leer
        var http = filterContext.HttpContext;
        http.Items["__ctrl"] = (string)filterContext.RouteData.Values["controller"];
        http.Items["__act"] = (string)filterContext.RouteData.Values["action"];
        base.OnActionExecuting(filterContext);
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        try
        {
            var http = filterContext.HttpContext;
            var user = http?.User?.Identity?.Name ?? Environment.UserName;
            var url = http?.Request?.Url?.ToString();
            var ip = http?.Request?.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(ip)) ip = http?.Request?.UserHostAddress;

            var rl = new RequestLogs
            {
                UserName = user,
                Controller = (string)filterContext.RouteData.Values["controller"],
                ActionName = (string)filterContext.RouteData.Values["action"],
                Url = url,
                HttpMethod = http?.Request?.HttpMethod,
                StatusCode = http?.Response?.StatusCode,
                IpAddress = ip,
                UserAgent = http?.Request?.UserAgent,
                Timestamp = DateTime.Now
            };

            using (var db = new BitacoraMvcDbEntities())
            {
                db.RequestLogs.Add(rl);
                db.SaveChanges();
            }
        }
        catch
        {
            // Evita que la bitácora rompa la petición si algo falla
        }

        base.OnActionExecuted(filterContext);
    }
}
