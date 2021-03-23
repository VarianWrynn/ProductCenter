//*****************************************************************************************************************************************************************************************
//											Modification history
//*****************************************************************************************************************************************************************************************
// C/A/D Change No   Author     Date        Description 

//	C	WL-1		Lee		    30/05/2014	改变CMS图像地址的配置信息，由原先的WebConfig固定配置改成根据当前用户请求域名返回这个配置。 
//                                          原因是CMS服务器利用Amazon加速之后(CDN)中国地区的域名和美国的区分开了，虽然后台还是一个IIS资源。
//******************************************************************************************************************************************************************************************

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    public class ProductConfigurationController : Controller
    {
        //
        // GET: /ProductConfiguration/

        /* [HttpGet,HttpPost] 这种方式不行，Get方式无法请求到！*/
        [HttpGet]
        public ActionResult ProductConfiguration(long skuid,User_Profile_Model userModel)
        {
            if (skuid < 1)
            {
                return View();
            }
            var psrv = new ProductsServices();
            var model = psrv.GetSingleSKU(skuid);
            //model.userInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
            model.userInfo = userModel;
            ViewBag.SKUInfo = new JavaScriptSerializer().Serialize(model);//把强类型对象传递到前端，提供JS脚本each统计箱柜尺寸信息...
            model.RelatedProducts = psrv.GetRelatedWebsitProducts(skuid, model.userInfo);


            //WL-1 -start

            //model.CMSImgUrl = ConfigurationManager.AppSettings["CMSImgUrl"];

           // model.CMSImgUrl = Request.Url.Scheme+"://" + Request.Url.Host + ":" + Request.Url.Port + "/MediaLib/Files/";

            if (Request.Url != null)
                model.CMSImgUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/MediaLib/Files/";
            //WL-1 -end

            model.StatusList = new CMSCacheableDataController().SkuStatusListObject();//2014年5月13日10:58:37
            return View(model);
        }

        /// <summary>
        /// 为颜色材料类别MCCC四个字段获取智能提示下拉单
        /// CreatedDate:2014年3月26日10:28:11
        /// </summary>
        /// <param name="term"></param>
        /// <param name="type"></param>
        /// <param name="ParentID">只有查询SubCategory的时候这个字段才有意义</param>
        /// <returns></returns>
// ReSharper disable InconsistentNaming
        public ActionResult GetAutoCompeltedMCCC(string term, string type, long ParentID)
// ReSharper restore InconsistentNaming
        {
            var pSrv = new ProductsServices();
            return Json(new NBCMSResultJson
            {
                Status = StatusType.OK,
                Data = pSrv.GetAutoCompeltedMCCC(term, type, ParentID)
            });
        }
        
        /// <summary>
        /// 注意没有调用try catch
        /// </summary>
        /// <param name="mcModel"></param>
        /// <returns></returns>
// ReSharper disable InconsistentNaming
        public ActionResult CheckMCC(MCCC mcModel)
// ReSharper restore InconsistentNaming
        {

            var pSvr = new ProductsServices();
            var msg = string.Empty;
            if (pSvr.CheckMCC(mcModel, ref msg))
            {
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = ""
                });
            }
            return Json(new NBCMSResultJson
            {
                Status = StatusType.Error,
                Data = msg
            });
        }


        /// <summary>
        /// Change1:新增了一个MCCC的模块，你懂得 2014年1月28日16:26:45
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mcModel"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public ActionResult UpdatedProduct([Bind(Exclude = "SKU")]CMS_SKU_Model model, MCCC mcModel, User_Profile_Model userModel)
        {

            try
            {
                if (model.SKUID == 0)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request is illegal."
                    });
                }
                //User_Profile_Model userModel = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);

                var pSvr = new ProductsServices();
                string msg;
                if (pSvr.UpdatedProduct(model, mcModel, userModel.User_Account, out msg))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "OK"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = msg == "update product failed." ? "" : msg
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

        /// <summary>
        /// 复制产品。
        /// Change:由先来的单个Channel的复制，改成现在的多个渠道一起复制，Lee 2013年12月6日10:29:05
        /// </summary>
        /// <returns></returns>
        public ActionResult DuplicateMultipleNewSku(long skuid, int newBrandId, string newSkuOrder, List<int> channelList, User_Profile_Model userModel)
        {
            try
            {
                //User_Profile_Model userModel = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);

                var pSvr = new ProductsServices();
                string msg;
                var retVal = pSvr.DuplicateMultipleNewSKU(skuid, newSkuOrder, newBrandId, channelList, userModel.User_Account, out msg);
                if (!retVal)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = msg == "" ? "failed to duplicate new SKU" : msg
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = msg == "Duplicate Product Successfully" ? "" : msg
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
        /// 更新HMNUMCosting的信息，用于HMNUM Configuration页面的的Costing的编辑更新
        /// 需要注意的是每一次的跟新都将在库表新增一条价格信息，影响将来报表的生成。
        /// 虽然和HMNUMController页面的方法一样，但是还是分开维护，因为2个展示有可能不同，遇到需求变动会变得痛苦！
        /// CreateDate:2013年11月14日11:19:26
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="costing"></param>
        /// <param name="retailPrice"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public ActionResult EditSkuCosting(long skuid, CMS_SKU_Costing_Model costing, decimal? retailPrice, User_Profile_Model userModel)
        {
            try
            {
                //User_Profile_Model userModel = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);

                var pSvr = new ProductsServices();
                if (!pSvr.EditSKUCosting(skuid, ref costing, userModel.User_Account, retailPrice))
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
        /// 获取图像列表，供给ProductConfigureation页面AttachSKU用...
        /// Change:新增按照图像分辨率进行分组的信息，以适应新的需求 2014年1月6日10:55:16
        /// </summary>
        /// <returns></returns>
        public ActionResult GetImageList(MediaLibrary_QueryModel model)
        {
            try
            {

                var count = 0;
                var groupByList = new List<MediaGroupBy_Model>();
                //针对SKU页面做的单独处理，如果当前页面没有关联任何的HM#，则不要展示任何形象。
                if (model.ProductID >= 1)
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = new
                        {
                            List = new MediaServices().GetMediaLibraryListWithGroup(model, out count, out groupByList),
                            groupByList,
                            Count = count
                        }
                    });
                var tempL = new List<MediaLibrary_Model>();

                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        List = tempL,
                        groupByList,
                        Count = count
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
        /// 提供ProductConfiguration页面直接上传Media之后，需要根据SKUID重新刷新获取图像信息的操作。由于GetImageList里面的SKUID已经另有其用，
        /// 所以无法共享一个方法，只能新建这个方法。
        /// CreateDate:2014年1月24日17:30:14
        /// </summary>
        /// <param name="skuid"></param>
        /// <returns></returns>
        public ActionResult GetImageListByWpid(long skuid)
        {
            try
            {

                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        List = new MediaServices().GetImageListBySKUID(skuid)
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
        /// 设置当前SKU产品的Primary Image。Add by Lee 2013年10月8日15:31:57
        /// </summary>
        /// <param name="skuid">SKU产品的ID</param>
        /// <param name="mediaId">图像的ID（存与MediaLibrary库表中的ID）</param>
        /// <returns></returns>
        public ActionResult SetPrimaryImage(long skuid, long mediaId)
        {
            try
            {
                var result = new MediaServices().SetPrimaryImage(skuid, mediaId);
                return Json(new NBCMSResultJson
                {
                    Status = result ? StatusType.OK : StatusType.Error,
                    Data = result ? "OK" : "Failed to set primary image"
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



        public ActionResult CopyImagesFromOtherSkuid(long skuid, long otherSkuid,User_Profile_Model userModel)
        {
            try
            {

                var pSvr = new ProductsServices();
                pSvr.CopyImagesFromOtherSKUID(skuid, otherSkuid, userModel);

                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = "OK"
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


        /// <summary>
        /// 多图像关联多渠道的SKUOrder.规则：如果原先的SKU已经关联了当中的某些图像，则保留，新建那些还未关联的图像。所以这里只能用双循环一个个判断。
        /// CreatedDate:2014年2月14日11:02:45
        /// </summary>
        /// <param name="mediaIdList"></param>
        /// <param name="wpidList"></param>
        /// <param name="userModel"></param>
        public ActionResult AttachImagesToSkuWithBatch(List<long> mediaIdList, List<long> wpidList, User_Profile_Model userModel)
        {
            try
            {

                var pSvr = new ProductsServices();
                pSvr.AttachImagesToSKUWithBatch(mediaIdList, wpidList, userModel);

                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = "OK"
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

        /// <summary>
        /// 查找当前HMNUM对于的所有的渠道。用于SKU关联图像的时候多渠道选择使用
        /// CreatedDate:2014年2月13日23:04:03
        /// Change1:格式返回不再是ChannelID和ChannleName,新增一个SKUID，这样子客户端在选择时候我们可以直接选取SKUID提交到
        /// 后台来更新，避免再次以通过ChannelID+HMNUM来查询SKUID！ 2014年2月14日10:47:42
        /// </summary>
        /// <returns></returns>
        public ActionResult GetChannelsByHm(string stockKey)
        {
            try
            {

                var pSvr = new ProductsServices();
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = pSvr.GetChannelsByHM(stockKey)
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


        /// <summary>
        ///  删除掉网站产品和其中一张图像的关系
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="mediaId"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public ActionResult RemoveSkuMedia(long skuid, long mediaId, User_Profile_Model userModel)
        {
            try
            {

                if (mediaId == 0 || mediaId == 0)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request is illegal."
                    });
                }

                var pSvr = new ProductsServices();
                //User_Profile_Model userModel =new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);

                if (pSvr.RemoveSKUMedia(skuid, mediaId, userModel))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Remove Successfully"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = "failed to remove this image"
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public ActionResult UpdateSellPack(SKU_HM_Relation_Model model, User_Profile_Model userModel)
        {
            try
            {
                if (model.SKUID < 1 || model.StockKeyID < 1 || model.ProductID < 1)//2013年11月26日10:35:58
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request parameters are illegal"
                    });
                }
                var pSvr = new ProductsServices();
                string msg;


                // User_Profile_Model userModel = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);

                if (pSvr.UpdateSellPack(model, out msg, userModel))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "OK"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = msg == "" ? "failed to insert new Product Pieces" : msg
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
