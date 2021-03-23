using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    public class HMNUMController : Controller
    {
        //
        // GET: /HMNUM/

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 获取HMNUM的信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetHMNUMList(CMS_HMNUM_Model model, int page, int rows)
        {
            try
            {
                HMNUMServices hSvr = new HMNUMServices();
                int count = 0;
                List<CMS_HMNUM_Model> list = hSvr.GetHMNUMList(model, page, rows, out count);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = count,
                        rows = list
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// 更新HMNUMCosting的信息，用于HMNUM Management页面的的inline-edit的编辑更新
        /// 需要注意的是每一次的跟新都将在库表新增一条价格信息，影响将来报表的生成。
        /// CreateDate:2013年11月13日6:00:34
        /// </summary>
        /// <param name="model"></param>
        /// <param name="costing"></param>
        /// <returns></returns>
        public ActionResult EditHMNUMCosting(CMS_HMNUM_Model model, CMS_HM_Costing_Model costing)
        {
            try
            {
                string cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
                var serializer = new JavaScriptSerializer();
                string decCookies = CryptTools.Decrypt(cookis);
                User_Profile_Model curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;

                HMNUMServices hSvr = new HMNUMServices();
                Boolean isCreated = hSvr.EditHMNUMCosting(model, costing, curUserInfo.User_Account);
                return Json(new NBCMSResultJson
                {
                    Status = isCreated == true ? StatusType.OK : StatusType.Error,
                    Data = isCreated == true ? "Done" : "Fail to udate current HM#'s costing"
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }



        public ActionResult SynchHMNewData()
        {
            try
            {
                HMNUMServices HMSvr = new HMNUMServices();
                int retVal = HMSvr.SynchHMNewData();
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = "Synchronizing Successfully"
                });
            }
            catch (Exception ex)//如果存储过程内部发生错，就跳到这里来了。。。。无数据返回-1
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Fatal(ex.Source);
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
