using System.Web;
using System.Web.Mvc;
using Permaisuri.Filters;

namespace Permaisuri
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new UserAuthAttribute());
            filters.Add(new NbcmsExceptionsAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}