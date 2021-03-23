using System.Web.Mvc;

namespace Permaisuri.Areas.ModelBinderTest
{
    public class ModelBinderTestAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ModelBinderTest";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ModelBinderTest_default",
                "ModelBinderTest/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
