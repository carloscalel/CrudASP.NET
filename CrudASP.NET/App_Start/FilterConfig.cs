using System.Web;
using System.Web.Mvc;

namespace CrudASP.NET
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogRequestAttribute()); // <--- agrega nuestro filtro asegúrate que en Global.asax.cs se llame
        }
    }
}
