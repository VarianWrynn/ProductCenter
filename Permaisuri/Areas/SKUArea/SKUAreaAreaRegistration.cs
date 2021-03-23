using System.Web.Mvc;

namespace Permaisuri.Areas.SKUArea
{
    public class SKUAreaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SKUArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SKUArea_default",
                "SKUArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
