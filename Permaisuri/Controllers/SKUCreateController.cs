using System;
using System.Web.Mvc;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;
using System.Threading.Tasks;

namespace Permaisuri.Controllers
{
// ReSharper disable InconsistentNaming
    public class SKUCreateController : Controller
// ReSharper restore InconsistentNaming
    {
        //
        // GET: /SKUCreate/

        public  ActionResult Index(User_Profile_Model userModel)
        {
            var model = new CMS_SKU_Model { userInfo = userModel };//用于前端控制非本公司销售人员的展示字段
            return View(model);
        }


        public ActionResult GetProductInfo(CMS_HMNUM_Model hmModel)
        {
            return Json(new NBCMSResultJson
            {
                Status = StatusType.OK,
                Data = new ProductCommonServices().GetProductsByKeyWords(hmModel, 0)
            });
        }

        /// <summary>
        ///  检查SKU表是否存在这个SKU...
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public async Task<ActionResult> CheckSkuExist(string sku, int channelId)
        {
            var skuSvr = new SKUCreateServices();
            var isExist = await skuSvr.CheckSKUExist(sku, channelId);
            return Json(new NBCMSResultJson
            {
                //Status = isExist == true ? StatusType.OK : StatusType.Error
                Status = isExist ? StatusType.OK : StatusType.Error
            });
            //return isExist == true ? "ture" : "false";
        }


// ReSharper disable InconsistentNaming
        public ActionResult AddNewSKU(CMS_SKU_Model model, MCCC mcModel,CMS_HMNUM_Model hmModel, User_Profile_Model userModel)
// ReSharper restore InconsistentNaming
        {
            try
            {
                var skuSvr = new SKUCreateServices();
                string msg;
                var newSkuid =  skuSvr.AddProduct(model, mcModel, hmModel, userModel.User_Account, out msg);
                return Json(new NBCMSResultJson
                {
                    Status = newSkuid > 0 ? StatusType.OK : StatusType.Error,
                    Data = newSkuid
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

    }
}
