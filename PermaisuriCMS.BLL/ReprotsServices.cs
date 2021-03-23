using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using PermaisuriCMS.Model.Reports;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

namespace PermaisuriCMS.BLL
{
    public class ReprotsServices
    {
        /// <summary>
        /// Change1:新增报表的total列的统计您信息。2014年2月10日10:46:43
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ChannelReportWithStatisModel GetSaleReprotByChannelList(ReportsModel model)
        {
           
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var adapter = (IObjectContextAdapter)db;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 3 * 60; // value in seconds

                ChannelReportWithStatisModel reportModel = new ChannelReportWithStatisModel();
               List<ChannelReportModel> list = new List<ChannelReportModel>();

               String OrderType = "asc";
               String OrderBy = "UnitsSold";
               if (model.OrderType > 0)
               {
                   OrderType = "desc";
               }
               if (model.OrderBy > 0)
               {
                   switch (model.OrderBy)
                   {
                       case 1://order by Channel
                           OrderBy = "ChannelName";
                           break;
                       case 2://order by  Sales Totals
                           OrderBy = "SalesTotal";
                           break;
                       case 3:// order by  Units Sold
                           OrderBy = "UnitsSold";
                           break;

                       case 4:// order by Avg. Sale Amount
                           OrderBy = "AvgAmont";
                           break;

                       default:
                           OrderBy = "UnitsSold";
                           break;
                   }
               }

               var countParam = new System.Data.Entity.Core.Objects.ObjectParameter("count", typeof(int));//注意，第一个参数count名称必须与Store Procedure里面的输出参数的名称一一对应！不区分大小写

               var SumSalesTotalParam =new  ObjectParameter("SumSalesTotal", typeof(int));
               var SumUnitsSoldParam = new ObjectParameter("SumUnitsSold", typeof(int));
               var SumAvgAmontParam = new ObjectParameter("SumAvgAmont", typeof(int));


               var res = db.Ecom_ReportByChannel_SP(model.page, model.rows, model.Channel, model.Brand, model.StartTime, model.EndTime,
                   OrderBy, OrderType, countParam, SumSalesTotalParam, SumUnitsSoldParam, SumAvgAmontParam).ToList();
               
                //count = Convert.ToInt32(countParam.Value);

               reportModel.Count =Convert.ToInt32(countParam.Value) ;
               reportModel.SumSalesTotal = Convert.ToInt32(SumSalesTotalParam.Value).ToString("C", new CultureInfo("en-US"));
               reportModel.SumUnitsSold = Convert.ToInt32(SumUnitsSoldParam.Value);
               reportModel.SumAvgAmont = Convert.ToInt32(SumAvgAmontParam.Value).ToString("C", new CultureInfo("en-US"));
               foreach (Ecom_ReportByChannel_SP_Result r in res)
               {
                   list.Add(new ChannelReportModel
                   {
                       Channel = r.ChannelName,
                       ChannelID = r.ChannelID,
                       SalesTotal = r.SalesTotal.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                       UnitsSold = r.UnitsSold.ConvertToNotNull(),
                       AvgAmont = r.AvgAmont.ConvertToNotNull().ToString("C", new CultureInfo("en-US"))
                   });
               }
               reportModel.ReportList = list;
               return reportModel;
            }
        }

        /// <summary>
        /// 报表类型：Sales by Product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ReportByProducttWithStatisModel GetReportByProductList(ReportsModel model)
        {

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var adapter = (IObjectContextAdapter)db;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 3 * 60; // value in seconds


                ReportByProducttWithStatisModel reportModel = new ReportByProducttWithStatisModel();
                List<ReportByProductModel> list = new List<ReportByProductModel>();

              
                // ToList() resovle:The result of a query cannot be enumerated more than once problems

                String OrderType = "asc";
                String OrderBy = "UnitsSold";
                if (model.OrderType > 0)
                {
                    OrderType = "desc";
                }
                if (model.OrderBy > 0)
                {
                    switch (model.OrderBy)
                    {
                        case 1://order by SKUOrder
                            OrderBy = "SKUOrder";
                            break;
                        case 2://order by SKUName
                            OrderBy = "ProductName";
                            break;
                        case 3://  SalesTotal
                            OrderBy = "SalesTotal";
                            break;

                        case 4:// UnitesSold
                            OrderBy = "UnitsSold";
                            break;

                        case 5://AvgAmount
                            OrderBy = "AvgAmount";
                            break;

                        default:
                            OrderBy = "UnitsSold";
                            break;
                    }
                }
                //DbType.Int32
                var countParam = new ObjectParameter("count", typeof(int));//注意，第一个参数count名称必须与Store Procedure里面的输出参数的名称一一对应！不区分大小写

                var SumSalesTotalParam = new ObjectParameter("SumSalesTotal", typeof(int));
                var SumUnitsSoldParam = new ObjectParameter("SumUnitsSold", typeof(int));
                var SumAvgAmontParam = new ObjectParameter("SumAvgAmont", typeof(int));


                var res = db.Ecom_ReportByProduct_SP(model.page, model.rows, model.SKUOrder,model.ProductName, model.Channel, model.Brand,
                    model.StartTime, model.EndTime, OrderBy, OrderType, countParam, SumSalesTotalParam, SumUnitsSoldParam, SumAvgAmontParam).ToList().AsEnumerable();

                reportModel.Count = Convert.ToInt32(countParam.Value);
                reportModel.SumSalesTotal = Convert.ToInt32(SumSalesTotalParam.Value).ToString("C", new CultureInfo("en-US"));
                reportModel.SumUnitsSold = Convert.ToInt32(SumUnitsSoldParam.Value);
                reportModel.SumAvgAmont = Convert.ToInt32(SumAvgAmontParam.Value).ToString("C", new CultureInfo("en-US"));


                //count = Convert.ToInt32(countParam.Value);

                foreach (Ecom_ReportByProduct_SP_Result r in res)
                {
                    list.Add(new ReportByProductModel
                    {
                       SKUOrder = r.SKUOrder,
                       ProductName =r.ProductName,
                       SalesTotal = r.SalesTotal.ConvertToNotNull().ToString("C",new CultureInfo("en-US")),
                       UnitsSold =r.UnitsSold.ConvertToNotNull(),
                       AvgAmont =r.AvgAmont.ConvertToNotNull().ToString("C", new CultureInfo("en-US"))
                       
                    });
                }
                reportModel.ReportList = list;
                return reportModel;
            }
        }


        /// <summary>
        /// 报表类型：Sales by HMNUMBBER
        /// Author:Lee
        /// Date: 2013年10月14日11:06:26
        ///讲次报表改成从Ecom订单上获取。2014年1月15日16:40:04
        ///Change2:新增统计信息 2014年2月11日11:06:51
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ReportByHMWithStatisModel GetReportByHMList(ReportsModel model)
        {

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var adapter = (IObjectContextAdapter)db;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 3 * 60; // value in seconds


                ReportByHMWithStatisModel reportModel = new ReportByHMWithStatisModel();

                List<ReportByHMModel> list = new List<ReportByHMModel>();
                String OrderType = "asc";
                String OrderBy = "UnitsSold";
                if (model.OrderType > 0)
                {
                    OrderType = "desc";
                }
                if (model.OrderBy > 0)
                {
                    switch (model.OrderBy)
                    {
                        case 1://order by HMNUM
                            OrderBy = "HMNUM";
                            break;
                        case 2://order by HMName
                            OrderBy = "HMName";
                            break;
                        case 3://  SalesTotal
                            OrderBy = "SalesTotal";
                            break;

                        case 4:// UnitesSold
                            OrderBy = "UnitsSold";
                            break;

                        case 5://AvgAmount
                            OrderBy = "AvgAmount";
                            break;

                        default:
                            OrderBy = "UnitsSold";
                            break;
                    }
                }

                var countParam = new ObjectParameter("count", DbType.Int32);//注意，第一个参数count名称必须与Store Procedure里面的输出参数的名称一一对应！不区分大小写
                var SumSalesTotalParam = new ObjectParameter("SumSalesTotal", typeof(int));
                var SumUnitsSoldParam = new ObjectParameter("SumUnitsSold", typeof(int));
                var SumAvgAmontParam = new ObjectParameter("SumAvgAmont", typeof(int));

                var res = db.Ecom_ReportByHM_SP(model.page, model.rows,model.HMNUM,model.HMName, model.Channel, model.Brand,
                    model.StartTime, model.EndTime, OrderBy, OrderType, countParam, SumSalesTotalParam, SumUnitsSoldParam, SumAvgAmontParam).ToList().AsEnumerable();

                //var res = db.Ecom_ReportByHM__SP(model.page, model.rows, model.Channel, model.Brand, OrderBy, OrderType, countParam).AsEnumerable();
                //注意：如果不先toList,而直接AsEnumerable()的话，会导致每次的输出参数count都是错的（11条）而不是实际的数据！！！2014年1月2日18:15:17
                //count = Convert.ToInt32(countParam.Value);


                reportModel.Count = Convert.ToInt32(countParam.Value);
                reportModel.SumSalesTotal = Convert.ToInt32(SumSalesTotalParam.Value).ToString("C", new CultureInfo("en-US"));
                reportModel.SumUnitsSold = Convert.ToInt32(SumUnitsSoldParam.Value);
                reportModel.SumAvgAmont = Convert.ToInt32(SumAvgAmontParam.Value).ToString("C", new CultureInfo("en-US"));


                foreach (Ecom_ReportByHM_SP_Result r in res)
                {
                    list.Add(new ReportByHMModel
                    {
                        HMNUM = r.HMNUM,
                        HMName =r.HMName,
                        IsGroup =r.IsGroup.ConvertToNotNull(),
                        SalesTotal =r.SalesTotal.ConvertToNotNull().ToString("C",new CultureInfo("en-US")),
                        UnitsSold =r.UnitsSold.ConvertToNotNull(),
                        AvgAmont =r.AvgAmont.ConvertToNotNull().ToString("C",new CultureInfo("en-US"))
                    });
                }

                reportModel.ReportList = list;
                return reportModel;
            }
        }


        /// <summary>
        /// Low inventory Report
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ReportLowInventoryModel> GetReportLowInventoryList(ReportsModel model, out int count)
        {

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                List<ReportLowInventoryModel> list = new List<ReportLowInventoryModel>();
                var res = db.Reports_LowInventory_V.Where(v => v.BrandID > 0);//INQ to Entities does not recognize the method 'Int32 ConvertToNotNull(System.Nullable`1[System.Int32])' method, and this method cannot be translated into a store expression.
                //var res = db.Reports_LowInventory_V.Where(V=>V.HMNUM!="");//这种方法Group by 会有问题
                if (model.Channel > 0)
                {
                    res = res.Where(v => v.ChannelID == model.Channel);
                }
                if (model.Brand > 0)
                {
                    res = res.Where(v => v.BrandID == model.Brand);
                }
                if (!string.IsNullOrEmpty(model.AffectedSKU))
                {
                    res = res.Where(v => v.SKU.Contains(model.AffectedSKU));
                }
                var newGroup = res.GroupBy(v => new
                {
                    Part = v.HMNUM,
                    ExpectedInventory = v.Exp_Qty,// db.Reports_LowInventory_V.Sum(vv => vv.SKU_QTY),使用这种方法，会造成Group by 之后2000条数据，则重新查询Reports_LowInventory_V 2000次！！2013年9月9日9:47:49
                    Inventory = v.StockByPcs.HasValue? v.StockByPcs.Value:0,
                    NextShipment = v.RepDate,
                    //StringNextShipment = v.RepDate.HasValue ? v.RepDate.Value.ToString("yyyy-MM-dd") : "N/A",
                    pImage = v.pImage
                }).Select(x => new ReportLowInventoryModel
                {
                    Part = x.Key.Part,
                    ExpectedInventory = x.Sum(vv => vv.Exp_Qty.HasValue?vv.Exp_Qty.Value:0),
                    Inventory = x.Key.Inventory,
                    NextShipment = x.Key.NextShipment,
                    pImage =x.Key.pImage,
                    AffectedSKUS_Temp = db.Reports_LowInventory_V.Where(v => v.HMNUM == x.Key.Part).Select(s => s.SKU).Distinct()
                }).AsEnumerable();

                if (model.OrderBy > 0)
                {
                    switch (model.OrderBy)
                    {
                        case 1://order by hmnum
                            if (model.OrderType > 0)
                            {
                                newGroup = newGroup.OrderByDescending(v => v.Part);
                            }
                            else
                            {
                                newGroup = newGroup.OrderBy(v => v.Part);
                            }
                            break;
                        case 2://order by Inventory
                            if (model.OrderType > 0)
                            {
                                newGroup = newGroup.OrderByDescending(v => v.Inventory);
                            }
                            else
                            {
                                newGroup = newGroup.OrderBy(v => v.Inventory);
                            }
                            break;
                        case 3:// order by  Expected Inventory
                            if (model.OrderType > 0)
                            {
                                newGroup = newGroup.OrderByDescending(v => v.ExpectedInventory);
                            }
                            else
                            {
                                newGroup = newGroup.OrderBy(v => v.ExpectedInventory);
                            }
                            break;

                        case 4://order by  Next Shipment
                            if (model.OrderType > 0)
                            {
                                newGroup = newGroup.OrderByDescending(v => v.NextShipment);
                            }
                            else
                            {
                                newGroup = newGroup.OrderBy(v => v.NextShipment);
                            }
                            break;
                        case 5://Affected SKUs
                            if (model.OrderType > 0)
                            {
                                newGroup = newGroup.OrderByDescending(v => v.AffectedSKUS_Temp);
                            }
                            else
                            {
                                newGroup = newGroup.OrderBy(v => v.AffectedSKUS_Temp);
                            }
                            break;
                        default:

                            break;
                    }
                }
                count = newGroup.Count();

                newGroup = newGroup.Skip((model.page - 1) * model.rows).Take(model.rows);

                list = newGroup.Select(x => new ReportLowInventoryModel
                {
                    ExpectedInventory = x.ExpectedInventory,
                    Inventory = x.Inventory,
                    NextShipment = x.NextShipment,
                    StringNextShipment = x.NextShipment.HasValue?x.NextShipment.Value.ToString("yyyy-MM-dd"):"N/A",
                    Part = x.Part,
                    pImage =x.pImage,
                    AffectedSKUS = x.AffectedSKUS_Temp.ToList()
                }).ToList();
                return list;
            }
        }



        /// <summary>
        /// d.	Add new report: Low Inventory by SKU
        /// i.	Same columns as Low Inventory by HM but listed by SKU instead of part number
        /// ii.	Add column for channel name to SKU inventory report
        /// Author Lee, Date:2013年10月14日16:37:48
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ReportLowInventoryBySKUModel> GetReportLowInventoryBySKUList(ReportsModel model, out int count)
        {

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                List<ReportLowInventoryBySKUModel> list = new List<ReportLowInventoryBySKUModel>();
                var res = db.Reports_LowInventory_V.Where(v => v.BrandID > 0);
                if (model.Channel > 0)
                {
                    res = res.Where(v => v.ChannelID == model.Channel);
                }
                if (model.Brand > 0)
                {
                    res = res.Where(v => v.BrandID == model.Brand);
                }
                //if (!string.IsNullOrEmpty(model.AffectedSKU))
                //{
                //    res = res.Where(v => v.SKU.Contains(model.AffectedSKU));
                //}
                var newGroup = res.GroupBy(v => new
                {
                    SKU = v.SKU,
                    ChannelName = v.ChannelName,
                    ExpectedInventory = v.Exp_Qty,// db.Reports_LowInventory_V.Sum(vv => vv.SKU_QTY),使用这种方法，会造成Group by 之后2000条数据，则重新查询Reports_LowInventory_V 2000次！！2013年9月9日9:47:49
                    Inventory = v.StockByPcs.HasValue ? v.StockByPcs.Value : 0,
                    NextShipment = v.RepDate,
                    pImage = v.pImage
                }).Select(x => new ReportLowInventoryBySKUModel
                {
                    SKU = x.Key.SKU,
                    ChannelName =x.Key.ChannelName,
                    ExpectedInventory = x.Sum(vv => vv.Exp_Qty.HasValue ? vv.Exp_Qty.Value : 0),
                    //Inventory = x.Key.Inventory,
                    Inventory = x.Min(vv=>vv.StockByPcs.HasValue?vv.StockByPcs.Value:0),
                    NextShipment = x.Key.NextShipment,
                    pImage = x.Key.pImage
                }).AsEnumerable();

                if (model.OrderBy > 0)
                {
                    switch (model.OrderBy)
                    {
                        case 1://order by SKU
                            if (model.OrderType > 0)
                            {
                                newGroup = newGroup.OrderByDescending(v => v.SKU);
                            }
                            else
                            {
                                newGroup = newGroup.OrderBy(v => v.SKU);
                            }
                            break;
                        case 2://order by ChannelName
                            if (model.OrderType > 0)
                            {
                                newGroup = newGroup.OrderByDescending(v => v.ChannelName);
                            }
                            else
                            {
                                newGroup = newGroup.OrderBy(v => v.ChannelName);
                            }
                            break;
                        case 3://order by Inventory
                            if (model.OrderType > 0)
                            {
                                newGroup = newGroup.OrderByDescending(v => v.Inventory);
                            }
                            else
                            {
                                newGroup = newGroup.OrderBy(v => v.Inventory);
                            }
                            break;
                        case 4:// order by  Expected Inventory
                            if (model.OrderType > 0)
                            {
                                newGroup = newGroup.OrderByDescending(v => v.ExpectedInventory);
                            }
                            else
                            {
                                newGroup = newGroup.OrderBy(v => v.ExpectedInventory);
                            }
                            break;

                        case 5://order by  Next Shipment
                            if (model.OrderType > 0)
                            {
                                newGroup = newGroup.OrderByDescending(v => v.NextShipment);
                            }
                            else
                            {
                                newGroup = newGroup.OrderBy(v => v.NextShipment);
                            }
                            break;
                        //case 6://Affected SKUs
                        //    if (model.OrderType > 0)
                        //    {
                        //        newGroup = newGroup.OrderByDescending(v => v.AffectedSKUS_Temp);
                        //    }
                        //    else
                        //    {
                        //        newGroup = newGroup.OrderBy(v => v.AffectedSKUS_Temp);
                        //    }
                        //    break;
                        default:

                            break;
                    }
                }
                count = newGroup.Count();

                newGroup = newGroup.Skip((model.page - 1) * model.rows).Take(model.rows);

                list = newGroup.Select(x => new ReportLowInventoryBySKUModel
                {
                    SKU = x.SKU,
                    ChannelName = x.ChannelName,
                    ExpectedInventory = x.ExpectedInventory,
                    Inventory = x.Inventory,
                    NextShipment = x.NextShipment,
                    StringNextShipment = x.NextShipment.HasValue ? x.NextShipment.Value.ToString("yyyy-MM-dd") : "N/A",
                    pImage = x.pImage
                }).ToList();
                return list;
            }
        }


        /// <summary>
        /// Produc tDevelopment Report
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ReportProductDevelopmentModel> GetReportProductDevelopmentList(ReportsModel model, out int count)
        {

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                List<ReportProductDevelopmentModel> list = new List<ReportProductDevelopmentModel>();

              
                var res = db.CMS_SKU.Where(v=>v.StatusID!=5);//排除掉状态为5 Active的数据
                if (model.Channel > 0)
                {
                    res = res.Where(v => v.ChannelID == model.Channel);
                }
                if (model.Brand > 0)
                {
                    res = res.Where(v => v.BrandID == model.Brand);
                }
                if (model.QueueStatus > 0)
                {
                    res = res.Where(v => v.StatusID == model.QueueStatus);
                }
                if (model.OrderBy > 0)
                {
                    switch (model.OrderBy)
                    {
                        case 1://order by Product(SKU+Name)
                            if (model.OrderType > 0)
                            {
                                res = res.OrderByDescending(v => v.SKU).ThenByDescending(v => v.ProductName);
                            }
                            else
                            {
                                res = res.OrderBy(v => v.SKU).ThenBy(v => v.ProductName);
                            }
                            break;
                        case 2://order by Brand
                            if (model.OrderType > 0)
                            {
                                res = res.OrderByDescending(v => v.Brand.BrandName);
                            }
                            else
                            {
                                res = res.OrderBy(v => v.Brand.BrandName);
                            }
                            break;
                        case 3:// order by  Channel
                            if (model.OrderType > 0)
                            {
                                res = res.OrderByDescending(v => v.Channel.ChannelName);
                            }
                            else
                            {
                                res = res.OrderBy(v => v.Channel.ChannelName);
                            }
                            break;

                        case 4://order by Queue Status
                            if (model.OrderType > 0)
                            {
                                res = res.OrderByDescending(v => v.StatusID);
                            }
                            else
                            {
                                res = res.OrderBy(v => v.StatusID);
                            }
                            break;
                        case 5://Last Updated
                            if (model.OrderType > 0)
                            {
                                res = res.OrderByDescending(v => v.UpdateOn);
                            }
                            else
                            {
                                res = res.OrderBy(v => v.UpdateOn);
                            }
                            break;
                        default:

                            break;
                    }
                }

                count = res.Count();
                res = res.Skip((model.page - 1) * model.rows).Take(model.rows);
                foreach (CMS_SKU w in res)
                {
                    list.Add(new ReportProductDevelopmentModel {
                        Brand = w.Brand == null ? "" : w.Brand.BrandName,
                        Channel =w.Channel==null?"NONE": w.Channel.ChannelName,
                        LastUpdated = w.UpdateOn.HasValue? w.UpdateOn.Value.ToString("yyyy-MM-dd"):"N/A",
                        Product = w.SKU+"  |  "+w.ProductName,
                        QueueStatus = w.StatusID,
                        ChannelID =w.ChannelID,
                        SKUOrder =w.SKU,
                        SKUID = w.SKUID
                    });
                }
                return list;
            }
        }


        /// <summary>
        /// 新增的报表类型
        /// David Eike: why is historical data necessary? Isn't it just Margin = (Retail - COG - Freight) for each line item (times units sold)?
        /// David Eike: In other words, an estimate based on current costing
        /// David Eike: right, it is not a guarantee of accuracy
        /// Lee.Wong: we just retrieve the laest data?
        /// David Eike: yeah, this is not an accounting application
        /// David Eike: just a guide to measure pricing strategies
        /// </summary>
        /// <param name="model"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<Reports_Margin_By_Product_V_Model> GetReportCostMarginByProduct(ReportsModel model, out int count)
        {

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                List<Reports_Margin_By_Product_V_Model> list = new List<Reports_Margin_By_Product_V_Model>();
                var query = db.Reports_Margin_By_Product_V.AsQueryable();
                if (model.Channel > 0)
                {
                    query = query.Where(v => v.ChannelID == model.Channel);
                }
                if (model.Brand > 0)
                {
                    query = query.Where(v => v.BrandID == model.Brand);
                }
                if (model.OrderBy > 0)
                {
                    switch (model.OrderBy)
                    {
                        case 1://order by SKUOrder
                            if (model.OrderType > 0)
                            {
                                query = query.OrderByDescending(v => v.SKUOrder);
                            }
                            else
                            {
                                query = query.OrderBy(v => v.SKUOrder);
                            }
                            break;
                        case 2://order by SKUName
                            if (model.OrderType > 0)
                            {
                                query = query.OrderByDescending(v => v.Name);
                            }
                            else
                            {
                                query = query.OrderBy(v => v.Name);
                            }
                            break;
                        case 3:// order by  ChannelName
                            if (model.OrderType > 0)
                            {
                                query = query.OrderByDescending(v => v.ChannelName);
                            }
                            else
                            {
                                query = query.OrderBy(v => v.ChannelName);
                            }
                            break;
                        case 4:// order by  BrandName
                            if (model.OrderType > 0)
                            {
                                query = query.OrderByDescending(v => v.BrandName);
                            }
                            else
                            {
                                query = query.OrderBy(v => v.BrandName);
                            }
                            break;

                        case 5:// order by  NormalSelling
                            if (model.OrderType > 0)
                            {
                                query = query.OrderByDescending(v => v.NormalSelling);
                            }
                            else
                            {
                                query = query.OrderBy(v => v.NormalSelling);
                            }
                            break;

                        case 6:// //order by COG
                            if (model.OrderType > 0)
                            {
                                query = query.OrderByDescending(v => v.COG);
                            }
                            else
                            {
                                query = query.OrderBy(v => v.COG);
                            }
                            break;

                        case 7:// order by Margin
                            if (model.OrderType > 0)
                            {
                                query = query.OrderByDescending(v => v.Margin);
                            }
                            else
                            {
                                query = query.OrderBy(v => v.Margin);
                            }
                            break;

                        case 8:// order by Inventory
                            if (model.OrderType > 0)
                            {
                                query = query.OrderByDescending(v => v.Inventory);
                            }
                            else
                            {
                                query = query.OrderBy(v => v.Inventory);
                            }
                            break;

                        default:

                            break;
                    }
                }
                count = query.Count();
                query = query.Skip((model.page - 1) * model.rows).Take(model.rows);

                list = query.Select(v => new Reports_Margin_By_Product_V_Model
                {
                    BrandID = v.BrandID,
                    BrandName = v.BrandName,
                    ChannelID = v.ChannelID,
                    ChannelName = v.ChannelName,
                    ImageName = v.ImageName,
                    Inventory = v.Inventory,
                    COG = v.COG.HasValue == true ? v.COG.Value : 0,
                    Margin = v.Margin.HasValue==true? v.Margin.Value:0,
                    Name = v.Name,
                    NormalSelling = v.NormalSelling,
                    SKUOrder = v.SKUOrder
                }).ToList();
                return list;
            }
        }
    }
}
