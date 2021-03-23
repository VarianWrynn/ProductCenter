using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Permaisuri.Controllers;
using Permaisuri.Controllers.ControllerExt;
using PermaisuriCMS.Model;

namespace Permaisuri.Areas.JSONP.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /JSONP/Test/

        public ActionResult Index()
        {
            return View();
        }

        public JsonpResult UpdateBrand()
        {
            try
            {
                var jsonData = Request["data2"];
                if (String.IsNullOrEmpty(jsonData))
                {
                    return this.Jsonp(new {error = "8888", error_description = "参数传递非法"});
                }
                var ser = new JavaScriptSerializer();
                var model = ser.Deserialize(jsonData, typeof (Channel_Model)) as Channel_Model;
                return model == null ? this.Jsonp(new { error = "7777", error_description = "参数传递不合规" }) : this.Jsonp(model);
            }
            catch (Exception ex)
            {
                return this.Jsonp(new {error = "9999", error_description = ex.Message});
            }
        }
    }
}
