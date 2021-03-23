using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;


namespace Permaisuri.Controllers
{
    /// <summary>
    /// 2013年11月13日11:56:50 Lee
    /// </summary>
    public class HMConfigController : Controller
    {
        //
        // GET: /HMConfig/

        public ActionResult Index()
        {
            CMS_HMNUM_Model model = null;
            var hSvr = new HmConfigServices();
            var pID = Request["ProductID"];
            if (String.IsNullOrEmpty(pID))
            {
                return View(model);
            }
            model = hSvr.GetSingleHm(new CMS_HMNUM_Model
            {
                ProductID = Convert.ToInt64(pID)
            });
            ViewBag.Media_Data_list = new JavaScriptSerializer().Serialize(model.MediaList);//提供JS脚本做图像的bubble展示使用 2014年5月16日14:25:18...
            //ViewBag.CMSImgUrl = ConfigurationManager.AppSettings["CMSImgUrl"];
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
        public ActionResult EditHMNUMCosting(CMS_HMNUM_Model model, CMS_HM_Costing_Model costing)
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
        public ActionResult EditHMBasicInfo(CMS_HMNUM_Model model)
        {
            try
            {
                User_Profile_Model curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                HmConfigServices hcSvr = new HmConfigServices();
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


        /// <summary>
        /// 根据Larfier测试结果，需要对没有箱柜尺寸的HM新增这些参数
        /// CreateDate:2013年12月10日15:32:00 Lee
        /// </summary>
        /// <param name="ctnModel"></param>
        /// <returns></returns>
        public ActionResult AddHMCarton(CMS_ProductCTN_Model ctnModel)
        {
            try
            {
                var curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                var hcSvr = new HmConfigServices();
                long CTNID = 0;
                if (!hcSvr.AddHmCarton(ctnModel, curUserInfo.User_Account, out CTNID))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Fail to Add current HM#'s cartons"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = CTNID
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
        /// 删除当前纸箱信息，用于HMConfig页面使用
        /// CreateDate:2013年12月10日16:59:43
        /// </summary>
        /// <param name="CTNID"></param>
        /// <returns></returns>
        public ActionResult DeleteCarton(long CTNID)
        {
            try
            {
                var hcSvr = new HmConfigServices();
                if (!hcSvr.DeleteCarton(CTNID))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Fail to Add current HM#'s cartons"
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
        /// 根据Larfier测试结果，需要对没有箱柜尺寸的HM新增这些参数
        /// CreateDate:2013年12月10日16:30:50
        /// </summary>
        /// <param name="ctnModel"></param>
        /// <returns></returns>
        public ActionResult AddDimension(CMS_ProductDimension_Model dimModel)
        {
            try
            {
                var curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                var hcSvr = new HmConfigServices();
                long DimID = 0;
                if (!hcSvr.AddDimension(dimModel, curUserInfo.User_Account, out DimID))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Fail to Add current HM#'s Dimension"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = DimID
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
        /// 删除尺寸信息，用户HMConfig页面使用
        /// CreateDate:2013年12月10日17:01:20
        /// </summary>
        /// <param name="DimID"></param>
        /// <returns></returns>
        public ActionResult DeleteDimension(long DimID)
        {
            try
            {
                var hcSvr = new HmConfigServices();
                if (!hcSvr.DeleteDimension(DimID))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Fail to Add current HM#'s cartons"
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
        /// 更新纸箱信息，用于HM# Configuration页面的纸箱信息更新
        /// </summary>
        /// <param name="ctnList"></param>
        /// <returns></returns>
        public ActionResult EditHmCartons(IEnumerable<CMS_ProductCTN_Model> ctnList)
        {
            try
            {
                var curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                var hcSvr = new HmConfigServices();
                if (!hcSvr.EditHmCartons(ctnList, curUserInfo.User_Account))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Fail to udate current HM#'s cartons"
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
        ///  更新产品尺寸信息，用于HM# Configuration页面的Product Demensions信息更新
        ///  Author:Lee Date:2013年11月16日10:51:34
        /// </summary>
        /// <param name="dimList"></param>
        /// <returns></returns>
        public ActionResult EditHmDimensions(IEnumerable<CMS_ProductDimension_Model> dimList)
        {
            try
            {
                var curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                var hcSvr = new HmConfigServices();
                if (!hcSvr.EditHmDimensions(dimList, curUserInfo.User_Account))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Fail to udate current HM#'s cartons"
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
                        Data = errMsg == string.Empty ? "Fail to delete this media" : errMsg
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
        /// <param name="HMNUM"></param>
        /// <returns></returns>
        public ActionResult GetImagesFromOtherSystem(string HMNUM)
        {
            try
            {
                var cSvr = new HMCommonServices();
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = cSvr.GetImagesFromOtherSystem(HMNUM)
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
        /// CreatedDate:2014年2月20日17:58:41
        /// 注意:组合产品也是调用这个方法。
        /// </summary>
        /// <param name="StockKeyID"></param>
        /// <returns></returns>
        public ActionResult GetImageListByProductID(long StockKeyID)
        {
            try
            {

                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        List = new MediaServices().GetImageListByProductID(StockKeyID)
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }


        /// <summary>
        /// 由于CMS的子产品允许有多张图片，eCom只接收一张，所以必须在CMS页面指定一个“Primary Image”的规则，设置了
        /// Primaryiamge的图片会发送到eCom去。（无法用组合产品的Primary image,因为当前子产品又可以和其他产品组合
        /// 成一个新的组合产品）。  2014年4月21日
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="MediaID"></param>
        /// <returns></returns>
        public ActionResult SetPrimaryImageForHMNUM(long ProductID, long MediaID)
        {
            try
            {
                var result = new MediaServices().SetPrimaryImageForHMNUM(ProductID, MediaID);
                return Json(new NBCMSResultJson
                {
                    Status = result == true ? StatusType.OK : StatusType.Error,
                    Data = result == true ? "OK" : "Failed to set primary image"
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }
        
    }
}
