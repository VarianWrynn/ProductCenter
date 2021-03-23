using System.Web.Mvc;

namespace Permaisuri.Areas.JSONP
{
    public class JsonpAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "JSONP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "JSONP_default",
                "JSONP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
