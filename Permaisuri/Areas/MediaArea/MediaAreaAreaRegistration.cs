using System.Web.Mvc;

namespace Permaisuri.Areas.MediaArea
{
    public class MediaAreaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MediaArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MediaArea_default",
                "MediaArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
