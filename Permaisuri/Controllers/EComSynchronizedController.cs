using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Model;
using PermaisuriCMS.Common;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.IO;

namespace Permaisuri.Controllers
{
    public class EComSynchronizedController : Controller
    {
        //
        // GET: /EComSynchronized/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetSKUList(SKU_Query_Model model)
        {
            try
            {

                var ccl = new CommonController();
                var useInfo = ccl.GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);

                var pSvr = new ProductsServices();
                var totalRecord = 0;
                var list = new ProductCommonServices().GetProductInventorys(model, out totalRecord, useInfo);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = totalRecord,
                        rows = list
                    }
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


        ///// <summary>
        ///// 防止这里是为了：（2014年2月27日19:15:01）
        ///// 1：SKU页面的逻辑已经非常复杂，这一块剥离出来减少复杂度，同时也许以后这个模块会被重复利用；
        ///// 2：之所以直接引用eCOM.BLL模块仅仅是为了强类型对象的共享。正常应该通过HttpRequest进行通信。
        ////  3: Send2eComPath 字段的引入证明了剥离出来的必要性（2014年3月5日）
        ///// </summary>
        ///// <param name="SKUID"></param>
        ///// <returns></returns>
        public ActionResult SendToEcomByID(long skuid, string imageStoragePath)
        {
            var pSvr = new ProductsServices();
            var eComSvr = new ICMSECOM.BLL.Insert2EComServices();
            var skuModel = pSvr.GetSingleSKU(skuid);
            var send2EComPath = "";

            if (skuModel.pMedia != null)
            {
                //imageName = mediaModel.HMNUM + "\\" + mediaModel.ImgName + mediaModel.fileFormat;
                var imageName = skuModel.pMedia.HMNUM + "\\" + skuModel.pMedia.ImgName + skuModel.pMedia.fileFormat;
                send2EComPath = Path.Combine(imageStoragePath, imageName);
            }
            skuModel.CMSPhysicalPath = imageStoragePath;
            skuModel.Send2eComPath = send2EComPath;
            eComSvr.Processing(skuModel);
            return Json(new NBCMSResultJson
            {
                Status = StatusType.OK,
                Data = "OK"
            });
        }


        /// <summary>
        /// 现在有两种做法，一种是传递SKUID列表上来，这种方式快捷安全，但是需要多访问一次数据库，
        /// 另一种是传递一个对象列表上来，缺点是Model不好初始化。
        /// </summary>
        /// <param name="skuidList"></param>
        /// <returns></returns>
        public ActionResult EcomSynchronized(IEnumerable<long> skuidList)
        {

            var imageStoragePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ImageStoragePath"]);
            Parallel.ForEach(skuidList, s => this.SendToEcomByID(s, imageStoragePath));
            return Json(new NBCMSResultJson
            {
                Status = StatusType.OK,
                Data = "OK"
            });
        }


    }
}
