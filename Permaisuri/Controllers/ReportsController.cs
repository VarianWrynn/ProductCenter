using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/
        //[HttpGet] [HttpPost]
        public ActionResult Index()
        {
            var reportType = 1;
            var sType = Request["ReportType"];
            if (!String.IsNullOrEmpty(sType))
            {
                reportType = Convert.ToInt32(sType);
            }
            //ViewBag.CMSImgUrl = ConfigurationManager.AppSettings["CMSImgUrl"];
            if (Request.Url != null)
                ViewBag.CMSImgUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/MediaLib/Files/";
            ViewBag.ReportType = reportType;
            return View();
        }

        /// <summary>
        /// 初始化Report页面，返回Channel ,Brand信息,同时默认返回Sales by Channel 的报表信息
        /// </summary>
        /// <param name="model">前端报表查询模型</param>
        /// <returns></returns>
        public ActionResult InitReports()//ReportsModel model
        {
            try
            {
                User_Profile_Model useInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        Channels = new ProductCommonServices().GetAllChannels(useInfo.IsChannelControl, useInfo.User_Guid),
                        IsChannelControl = useInfo.IsChannelControl,//用户指示报表前端是否展示All这个选项 Add on 2013年11月4日11:44:26
                        Brands = new ProductCommonServices().GetAllBrands(),
                        QueueStatus = new CMSCacheableDataServices().SKUStatusList()
                    }
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
        /// 根据前端查询信息动态展示各种不同类型的报表 供给：Reports页面使用.
        /// By using ToList, you are triggering the execution of the query and bringing all the results 
        /// from the database into memory. 
        /// 如果你数据库建立在本地而你本地的机器内存又吃紧，很可能导致查询超时！而你跟踪SQL Profiler数据库存储过程语句，
        /// 又会发现该语句在查询分析器查又非常的快，但是前端查询一直超时，原因很可能就是本地的机器/Web服务器的内存吃紧造成！
        /// 2013年9月25日9:47:13  Lee
        /// </summary>
        /// <param name="model">前端报表查询模型</param>
        /// <returns></returns>
        public ActionResult GetReportsList(ReportsModel model)
        {

            try
            {
                ReprotsServices repSvr = new ReprotsServices();

                var retObj = new NBCMSResultJson{
                    Status = StatusType.OK,
                    Data =null
                };
                int count = 0;
                switch (model.ReportType)
                {
                    case 1://Sales by Channel
                        retObj.Data = repSvr.GetSaleReprotByChannelList(model);
                        break;
                    case 2://Sales by Product
                        retObj.Data =repSvr.GetReportByProductList(model);
                        break;
                    case 3://Product Development Report
                        retObj.Data = new
                        {
                            List = repSvr.GetReportProductDevelopmentList(model, out count),
                            Count = count
                        };
                        break;
                    case 4://Low Inventory Report
                        retObj.Data = new
                        {
                            List = repSvr.GetReportLowInventoryList(model, out count),
                            //imgUrl = ConfigurationManager.AppSettings["EcomProductImageUrl"],
                            Count = count
                        };
                        break;
                    case 5://Sales by HMNUM

                        retObj.Data = repSvr.GetReportByHMList(model);
                        //retObj.Data = new
                        //{
                        //    List = repSvr.GetReportByHMList(model, out count),
                        //    imgUrl = ConfigurationManager.AppSettings["EcomProductImageUrl"],
                        //    Count = count
                        //};
                        break;
                    case 6://Low Inventory Report BY SKU
                        retObj.Data = new
                        {
                            List = repSvr.GetReportLowInventoryBySKUList(model, out count),
                            Count = count
                        };
                        break;

                    case 7://Cost and Margin by Product 
                        retObj.Data = new
                        {
                            List = repSvr.GetReportCostMarginByProduct(model, out count),
                            Count = count
                        };
                        break;

                    default:
                        retObj.Status = StatusType.Error;
                        retObj.Data = "A unhandle ReportType was requested";
                        break;
                }
                return Json(retObj);
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
                else
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
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("Message:"+ex.Message);
                NBCMSLoggerManager.Error("Source:"+ex.Source);
                NBCMSLoggerManager.Error("StackTrace:"+ex.StackTrace);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// 导出CSV
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult ExportToExcel(ReportsModel model)
        {
            try
            {
                ReportsExportServices repSvr = new ReportsExportServices();
                switch (model.ReportType)
                {
                    case 1://Sales by Channel
                        return this.ExcelCsv(repSvr.ExportReportByChannelList(model), "Sales_By_Channel.csv", "application/ms-txt");
                    case 2://Sales by Product
                        return this.ExcelCsv(repSvr.ExportReportByProductList(model), "Sales_By_Product.csv", "application/ms-txt");
                    //break;
                    case 3://Product Development Report
                        return this.ExcelCsv(repSvr.ExportReportDevList(model), "Product_Development_Report.csv", "application/ms-txt");
                    case 4://Low Inventory Report
                       // string sFormat4 = model.StartTime.ToString("yyyy-MM-dd") + "--" + model.EndTime.ToString("yyyy-MM-dd");
                        return this.ExcelCsv(repSvr.ExportReportLowInventoryList(model), "Low_Inventory_Report.csv", "application/ms-txt");
                    case 5://Sales by HMNUM
                        return this.ExcelCsv(repSvr.ExportReportByHMList(model), "Sales_By_HMNUM.csv", "application/ms-txt");
                    case 6://Low Inventory Report BY SKU
                        return this.ExcelCsv(repSvr.ExportReportLowInventoryBySKUList(model), "Low_Inventory_Report_By_SKU.csv", "application/ms-txt");
                    case 7://Cost Margin By Product Reports
                        return this.ExcelCsv(repSvr.ExportReportCost_Margin_By_Product(model), "Cost_Margin_By_Product.csv", "application/ms-txt");
                    default:

                        break;
                }
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("Message:" + ex.Message);
                NBCMSLoggerManager.Error("Source:" + ex.Source);
                NBCMSLoggerManager.Error("StackTrace:" + ex.StackTrace);
            }
            return this.ExcelCsv("There is a error has been occur   please contact administrator".ToString(), "Error.csv", "application/ms-txt");
        }

    }
}
