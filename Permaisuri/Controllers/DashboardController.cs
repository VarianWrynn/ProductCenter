using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PermaisuriCMS.Model;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using System.Configuration;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Data.Entity.Core;

namespace Permaisuri.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            var curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
            var pcSvr = new ProductCommonServices();
            var channelModelList = pcSvr.GetAllChannels(curUserInfo.IsChannelControl, curUserInfo.User_Guid);
            return View(channelModelList);
        }

        /// <summary>
        /// Get PO Orders for Dashboard displaying (HightCharts)
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPoOrders(List<Int32> channelList, String sTime, String eTime)
        {
            try
            {
                var sPage = Request["page"];
                var sRows = Request["rows"];
                var page = 1;
                var rows = 10;
                if (!string.IsNullOrEmpty(sPage))
                {
                    page = Convert.ToInt32(sPage);
                }
                if (!string.IsNullOrEmpty(sRows))
                {
                    rows = Convert.ToInt32(sRows);
                }

                var useInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                var dSvs = new DashboardServices();
                int totalRecord;
                var lowInventoryList = dSvs.GetLowInventory(page, rows, out totalRecord, useInfo);
                var serializer = new JavaScriptSerializer {MaxJsonLength = Int32.MaxValue};
                /*当订单数据量超过13000行记录的时候会报错，需要特殊配置*/
                //var serializer = new JavaScriptSerializer();
                // For simplicity just use Int32's max value.
                // You could always read the value from the config section mentioned above.
                var resultData = new
                {
                    Status = StatusType.OK,
                    Data = dSvs.GetEcomOrder(channelList.ToArray(), sTime, eTime).ToArray(),
                    Metrics = new
                    {
                        AvgOrders = dSvs.GetAvgDailyOrders(sTime, eTime),
                        AvgAmt = dSvs.GetAvgOrderAmount(sTime, eTime).ToString("C", new CultureInfo("en-US")),
                        ProductDev = dSvs.GetProductsDevCount(),
                        ItemAtt = dSvs.GetAttentionItemsCount()
                    },
                    LowInventory = lowInventoryList,
                    ProductDevList = dSvs.GetProductsDevList(),
                    TotalRecord = totalRecord
                };
                var result = new ContentResult
                {
                    Content = serializer.Serialize(resultData),
                    ContentType = "application/json"
                };
                return result;
            }
            catch (EntityCommandExecutionException ex)
            {
                if (ex.InnerException != null)
                {
                    NBCMSLoggerManager.Error(ex.InnerException.Message);
                    NBCMSLoggerManager.Error(ex.InnerException.Source);
                    NBCMSLoggerManager.Error(ex.InnerException.StackTrace);
                    NBCMSLoggerManager.Error("");
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Exception,
                        Data = ex.InnerException.Message
                    });
                }
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
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
        /// 这个方法是用于Dashboard页面重新查询订单信息（点击页面Query按钮）的时候用的
        /// CreateDate:2013年12月23日16:49:48
        /// </summary>
        /// <param name="channelList"></param>
        /// <param name="sTime"></param>
        /// <param name="eTime"></param>
        /// <returns></returns>
        public ActionResult QueryPoOrder(List<Int32> channelList, String sTime, String eTime)
        {
            try
            {
                var dSvs = new DashboardServices();
                /*当订单数据量超过13000行记录的时候会报错，需要特殊配置*/
                var serializer = new JavaScriptSerializer {MaxJsonLength = Int32.MaxValue};
                var resultData = new
                {
                    Status = StatusType.OK,
                    Data = dSvs.GetEcomOrder(channelList.ToArray(), sTime, eTime).ToArray(),
                    Metrics = new
                    {
                        AvgOrders = dSvs.GetAvgDailyOrders(sTime, eTime),
                        AvgAmt = dSvs.GetAvgOrderAmount(sTime, eTime).ToString("C", new CultureInfo("en-US"))
                    },
                };
                var result = new ContentResult
                {
                    Content = serializer.Serialize(resultData),
                    ContentType = "application/json"
                };
                return result;
            }
            catch (EntityCommandExecutionException ex)
            {
                if (ex.InnerException != null)
                {
                    NBCMSLoggerManager.Error(ex.InnerException.Message);
                    NBCMSLoggerManager.Error(ex.InnerException.Source);
                    NBCMSLoggerManager.Error(ex.InnerException.StackTrace);
                    NBCMSLoggerManager.Error("");
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Exception,
                        Data = ex.InnerException.Message
                    });
                }
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
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
