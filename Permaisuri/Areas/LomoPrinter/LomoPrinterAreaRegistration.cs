using System.Web.Mvc;

namespace Permaisuri.Areas.LomoPrinter
{
    public class LomoPrinterAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "LomoPrinter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LomoPrinter_default",
                "LomoPrinter/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
