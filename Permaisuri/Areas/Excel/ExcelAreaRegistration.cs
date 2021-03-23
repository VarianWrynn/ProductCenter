using System.Web.Mvc;

namespace Permaisuri.Areas.Excel
{
    public class ExcelAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Excel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Excel_default",
                "Excel/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
