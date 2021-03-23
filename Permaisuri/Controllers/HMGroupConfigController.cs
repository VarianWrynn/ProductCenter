using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    /// <summary>
    /// 组合HMNUM的配置
    /// </summary>
    public class HmGroupConfigController : Controller
    {
        //
        // GET: /HMGroupConfig/

        public ActionResult Config()
        {
            var hSvr = new HMGroupConfigServices();
            var pId = Request["ProductID"];
            if (String.IsNullOrEmpty(pId))
            {
                return View();
            }
            var model = hSvr.GetSingleHMGroup(new CMS_HMNUM_Model
            {
                ProductID = Convert.ToInt64(pId)
            });
            ViewBag.Media_Data_list = new JavaScriptSerializer().Serialize(model.MediaList);//提供JS脚本做图像的bubble展示使用 2014年5月16日14:25:18...
            //ViewBag.CMSImgUrl = ConfigurationManager.AppSettings["CMSImgUrl"];
            if (Request.Url != null)
                ViewBag.CMSImgUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/MediaLib/Files/";
            return View(model);
        }


        /// <summary>
        /// 更新HMNUMCosting的信息，用于HMNUM Configuration页面的的Costing的编辑更新
        /// 需要注意的是每一次的跟新都将在库表新增一条价格信息，影响将来报表的生成。
        /// 虽然和HMNUMController页面的方法一样，但是还是分开维护，因为2个展示有可能不同，遇到需求变动会变得痛苦！
        /// CreateDate:2013年11月14日11:19:26
        /// </summary>
        /// <param name="model"></param>
        /// <param name="costing"></param>
        /// <returns></returns>
        public ActionResult EditHmnumCosting(CMS_HMNUM_Model model, CMS_HM_Costing_Model costing)
        {
            try
            {
                var curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                var hcSvr = new HmConfigServices();
                if (!hcSvr.EditHmnumCosting(model, ref costing, curUserInfo.User_Account))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Fail to udate current HM#'s costing"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = costing
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
        /// 编辑HM#基本信息，用于HMNUM Configuration页面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult EditHmBasicInfo(CMS_HMNUM_Model model)
        {
            try
            {
                var curUserInfo =new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                var hcSvr = new HmConfigServices();
                if (!hcSvr.EditHmBasicInfo(model, curUserInfo.User_Account))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Fail to udate current HM#'s  basic info"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = "OK"
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



        public ActionResult UpdateSellSets(long productId, long childrenProductId, int sellSets)
        {
            if (productId < 1 || childrenProductId < 1 || sellSets < 1)
            {
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = "Request is illegal"
                });
            }
            try
            {
                var userInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                var gSvr = new HMGroupConfigServices();
                var isDone = gSvr.UpdateSellSets(productId, childrenProductId, sellSets, userInfo);
                return Json(new NBCMSResultJson
                {
                    Status = isDone == true ? StatusType.OK : StatusType.Error,
                    Data = isDone == true ? "OK" : "Fail to update sellset"
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
        /// HM# 页面删除图像
        /// 2013年12月25日10:56:35
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public ActionResult DeleteCmsMedia(long mediaId)
        {
            try
            {
                var cSvr = new HMCommonServices();
                string errMsg;
                if (!cSvr.DeleteCMSMedia(mediaId, out errMsg))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = errMsg==string.Empty? "Fail to delete this media":errMsg
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = "OK"
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
        /// 获取、组装WePO eCom 图像信息（根据HM#来查询）
        /// CreateDate:2013年12月25日14:51:04
        /// </summary>
        /// <param name="hmnum"></param>
        /// <returns></returns>
        public ActionResult GetImagesFromOtherSystem(string hmnum)
        {
            try
            {
                var cSvr = new HMCommonServices();
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = cSvr.GetImagesFromOtherSystem(hmnum)
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

    }
}
