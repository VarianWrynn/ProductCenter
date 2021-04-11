using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    public class ProductSearchController : Controller
    {
        //
        // GET: /ProductSearch/

        public ActionResult ProductSearch()
        {
            ViewBag.reqFrom = "ProductSearch";
            var lowInventory = Request["reqFrom"];//判断是直接打开ProductSearch页面还是从Dahsboard页面请求过来的
            if (!string.IsNullOrEmpty(lowInventory))
            {
                ViewBag.reqFrom = "lowInventory";
            }

            //新增一个Items requiring attention的参数 （从Dashboard来）
            var sStatus = Request["reqStatus"];
            if (!String.IsNullOrEmpty(sStatus))
            {
            }
            ViewBag.reqStatus = sStatus;
            return View();
        }


        /// <summary>
        /// 提供给SearchProducts页面使用，返回库存信息。（第一次展示页面使用）
        /// </summary>
        /// <returns></returns>
        public ActionResult InitSearchProducts(SKU_Query_Model model, User_Profile_Model useInfo)
        {
            try
            {
                int totalRecord;
                var list = new ProductCommonServices().GetProductInventorys(model, out totalRecord, useInfo);

                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        dgDatas = new
                        {
                            total = totalRecord,
                            rows = list
                        }
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
        /// 提供给SearchProduct页面使用，每次根据AJAX请求，动态返回库存信息。
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProductDGData(SKU_Query_Model model, User_Profile_Model useInfo)
        {
            try
            {
                var list = new ProductCommonServices().GetProductInventorys(model, out var totalRecord, useInfo);
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
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }


        public FileResult ExportToExcel(string postData, User_Profile_Model useInfo)
        {
            try
            {
                //反序列化客户端传递上来的数据
                var serializer = new JavaScriptSerializer();
                var model = serializer.Deserialize(postData, typeof(SKU_Query_Model)) as SKU_Query_Model;
                var pcSvr = new ProductSearchServices();
                var fileName = pcSvr.ExoprtedSKUDataToServer(model, useInfo);
                var exportPath = System.Web.HttpContext.Current.Server.MapPath("~/MediaLib/");
                var path = exportPath + fileName;
                return File(path, "application/ms-txt", fileName);
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("Message:" + ex.Message);
                NBCMSLoggerManager.Error("Source:" + ex.Source);
                NBCMSLoggerManager.Error("StackTrace:" + ex.StackTrace);
                return null;
                // return this.ExcelCSV("There is a error has been occur   please contact administrator".ToString(), "Product_Search_Result_Error.csv", "application/ms-txt");
            }
        }

        /// <summary>
        /// 用于ProductSearch页面的几个字段的AutoCompleted用(新需求：Millie,Mike)2014年3月7日
        /// </summary>
        /// <param name="term"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult GetAutoCompeltedInfo(string term, int type)
        {
            var pss = new ProductSearchServices();
            return Json(new NBCMSResultJson
            {
                Status = StatusType.OK,
                Data = pss.GetAutoCompeltedInfo(term, type)
            });
        }
    }
}
