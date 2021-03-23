using PermaisuriCMS.Model;
using System.Web.Mvc;
using PermaisuriCMS.BLL;
using System.Web.Script.Serialization;

namespace Permaisuri.Controllers
{
    public class SkuConfigurationController : Controller
    {
        //
        // GET: /SKUConfiguration/

        public ActionResult Index(User_Profile_Model userModel)
        {
            //var model = new CMS_SKU_Model {userInfo = userModel};
            var model = new ProductsServices().GetSingleSKU(1);
            model.userInfo = userModel;
            ViewBag.SKUInfo = new JavaScriptSerializer().Serialize(model);//把强类型对象传递到前端，提供JS脚本each统计箱柜尺寸信息...
            model.RelatedProducts = new ProductsServices().GetRelatedWebsitProducts(1, model.userInfo);
            return View(model);
        }

    }
}
