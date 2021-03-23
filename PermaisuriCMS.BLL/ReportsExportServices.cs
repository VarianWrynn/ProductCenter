using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using PermaisuriCMS.Model.Reports;

namespace PermaisuriCMS.BLL
{
    public class ReportsExportServices
    {
        /// <summary>
        ///  2014年1月15日12:07:13 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public String ExportReportByChannelList(ReportsModel model)
        {

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var adapter = (IObjectContextAdapter)db;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 5 * 60; // value in seconds

                List<ReportByProductModel> list = new List<ReportByProductModel>();

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
                            OrderBy = "ChannelName";
                            break;
      
                        case 2://  SalesTotal
                            OrderBy = "SalesTotal";
                            break;

                        case 3:// UnitesSold
                            OrderBy = "UnitsSold";
                            break;

                        case 4://AvgAmount
                            OrderBy = "AvgAmount";
                            break;

                        default:
                            OrderBy = "UnitsSold";
                            break;
                    }
                }

                //var res = db.Excel_ReportByProduct_SP(model.Channel, model.Brand, model.StartTime, model.EndTime, OrderBy, OrderType).ToList().AsEnumerable();
                var res = db.Excel_ReportByChannel_SP(model.Channel, model.Brand, model.StartTime, model.EndTime, OrderBy, OrderType).AsEnumerable();

                StringBuilder sw = new StringBuilder("Channel,Sales Totals ,Units Sold, Avg.Sale Amount");
                sw.AppendLine();

                //直接从DB读取转化为String返回去，少了一层实体类的转化，减少性能损耗....
                // Lee 2013年10月17日17:14:00
                foreach (Excel_ReportByChannel_SP_Result r in res)
                {
                    sw.Append(r.ChannelName).Append(",");
                    sw.Append(r.SalesTotal.ConvertToNotNull().ToString("C", new CultureInfo("en-US")).Replace(",", "")).Append(",");
                    sw.Append(r.UnitsSold.ConvertToNotNull()).Append(",");
                    sw.Append(r.AvgAmont.ConvertToNotNull().ToString("C", new CultureInfo("en-US")).Replace(",", "")).Append(",");
                    sw.AppendLine();
                }
                return sw.ToString();
            }
        }


        /// <summary>
        /// 导出报表类型：Sales by Product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public String ExportReportByProductList(ReportsModel model)
        {

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var adapter = (IObjectContextAdapter)db;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 5 * 60; // value in seconds

                List<ReportByProductModel> list = new List<ReportByProductModel>();

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

                //var res = db.Excel_ReportByProduct_SP(model.Channel, model.Brand, model.StartTime, model.EndTime, OrderBy, OrderType).ToList().AsEnumerable();
                var res = db.Excel_ReportByProduct_SP(model.SKUOrder, model.ProductName, model.Channel, model.Brand,
                    model.StartTime, model.EndTime, OrderBy, OrderType).AsEnumerable();

                StringBuilder sw = new StringBuilder("SKUOrder,Product Name,Sales Totals ,Units Sold,Avg.Sale Amount");
                sw.AppendLine();

                //直接从DB读取转化为String返回去，少了一层实体类的转化，减少性能损耗....
                // Lee 2013年10月17日17:14:00
                foreach (Excel_ReportByProduct_SP_Result r in res)
                {
                    sw.Append(r.SKUOrder).Append(",");
                    sw.Append(r.ProductName.Replace(",", "-")).Append(",");
                    sw.Append(r.SalesTotal.ConvertToNotNull().ToString("C", new CultureInfo("en-US")).Replace(",", "")).Append(",");
                    sw.Append(r.UnitsSold).Append(",");
                    sw.Append(r.AvgAmont.ConvertToNotNull().ToString("C", new CultureInfo("en-US")).Replace(",", "")).Append(",");
                    sw.AppendLine();
                }
                return sw.ToString();
            }
        }


        /// <summary>
        /// 导出报表类型：Sales by HMNUM
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public String ExportReportByHMList(ReportsModel model)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var adapter = (IObjectContextAdapter)db;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 5 * 60; // value in seconds

                List<ReportByProductModel> list = new List<ReportByProductModel>();

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
                            OrderBy = "HMNUM";
                            break;
                        case 2://order by SKUName
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
                var res = db.Excel_ReportByHM_SP(model.HMNUM, model.HMName, model.Channel, model.Brand,
                    model.StartTime, model.EndTime, OrderBy, OrderType).AsEnumerable();

                StringBuilder sw = new StringBuilder("HMNUM,HMNUM Name,Sales Totals ,Units Sold,Avg.Sale Amount");
                sw.AppendLine();

                //直接从DB读取转化为String返回去，少了一层实体类的转化，减少性能损耗....
                // Lee 2013年10月17日17:14:00
                foreach (Excel_ReportByHM_SP_Result r in res)
                {
                    sw.Append(r.HMNUM).Append(",");
                    sw.Append(r.HMName.Replace(",", "-")).Append(",");
                    sw.Append(r.SalesTotal.ConvertToNotNull().ToString("C", new CultureInfo("en-US")).Replace(",", "")).Append(",");
                    sw.Append(r.UnitsSold).Append(",");
                    sw.Append(r.AvgAmont.ConvertToNotNull().ToString("C", new CultureInfo("en-US")).Replace(",", "")).Append(",");
                    sw.AppendLine();
                }
                return sw.ToString();
            }
        }

        /// <summary>
        /// Export Low inventory Report to Excel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public String ExportReportLowInventoryList(ReportsModel model)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var res = db.Reports_LowInventory_V.Where(v => v.BrandID > 0);
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
                    ExpectedInventory = v.Exp_Qty,
                    Inventory = v.StockByPcs.HasValue ? v.StockByPcs.Value : 0,
                    NextShipment = v.RepDate,
                    pImage = v.pImage
                }).Select(x => new ReportLowInventoryModel
                {
                    Part = x.Key.Part,
                    ExpectedInventory = x.Sum(vv => vv.Exp_Qty.HasValue ? vv.Exp_Qty.Value : 0),
                    Inventory = x.Key.Inventory,
                    NextShipment = x.Key.NextShipment,
                    pImage = x.Key.pImage,
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
                StringBuilder sw = new StringBuilder("Part#,Inventory,ExpectedInventory,NextShipment,AffectedSKUS_Temp");
                sw.AppendLine();
                foreach (var r in newGroup)
                {
                    sw.Append(r.Part.Trim()).Append(",");
                    sw.Append(r.Inventory).Append(",");
                    sw.Append(r.ExpectedInventory).Append(",");
                    sw.Append(r.NextShipment.HasValue ? r.NextShipment.Value.ToString("yyyy-MM-dd") : "N/A").Append(",");
                    sw.Append(r.AffectedSKUS_Temp.ConvertToString('|')).Append(",");
                    sw.AppendLine();
                }
                return sw.ToString();
            }
        }



        public String ExportReportDevList(ReportsModel model)
        {

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                List<ReportProductDevelopmentModel> list = new List<ReportProductDevelopmentModel>();


                var res = db.CMS_SKU.Where(v => v.StatusID != 5);//排除掉状态为5 Active的数据
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
                StringBuilder sw = new StringBuilder("Product,Brand,Channel,QueueStatus,Last Updated");
                sw.AppendLine();
                foreach (var r in res)
                {
                    sw.Append(r.ProductName.Trim()).Append(",");
                    sw.Append(r.Brand==null?"NONE":r.Brand.BrandName).Append(",");
                    sw.Append(r.Channel == null ? "NONE" : r.Channel.ChannelName).Append(",");
                    sw.Append(r.SKU_Status == null ? "NONE" : r.SKU_Status.StatusName).Append(",");
                    sw.Append(r.UpdateOn.ConvertToStringWithoutHours()).Append(",");
                    sw.AppendLine();
                }
                return sw.ToString();
            }
        }



        public String ExportReportLowInventoryBySKUList(ReportsModel model)
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
                var newGroup = res.GroupBy(v => new
                {
                    SKU = v.SKU,
                    ChannelName = v.ChannelName,
                    ExpectedInventory = v.Exp_Qty,// db.Reports_LowInventory_V.Sum(vv => vv.SKU_QTY),使用这种方法，会造成Group by 之后2000条数据，则重新查询Reports_LowInventory_V 2000次！！2013年9月9日9:47:49
                    Inventory = v.StockByPcs.HasValue ? v.StockByPcs.Value : 0,
                    //NextShipment = v.RepDate.HasValue ? v.RepDate.Value : DateTime.MinValue,
                    NextShipment = v.RepDate,
                    pImage = v.pImage
                }).Select(x => new ReportLowInventoryBySKUModel
                {
                    SKU = x.Key.SKU,
                    ChannelName = x.Key.ChannelName,
                    ExpectedInventory = x.Sum(vv => vv.Exp_Qty.HasValue ? vv.Exp_Qty.Value : 0),
                    Inventory = x.Min(vv => vv.StockByPcs.HasValue ? vv.StockByPcs.Value : 0),
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
                        default:

                            break;
                    }
                }


                StringBuilder sw = new StringBuilder("SKU,ChannelName,Inventory,Expected Inventory,Next Shipment");
                sw.AppendLine();
                foreach (var r in newGroup)
                {
                    sw.Append(r.SKU).Append(",");
                    sw.Append(r.ChannelName).Append(",");
                    sw.Append(r.Inventory).Append(",");
                    sw.Append(r.ExpectedInventory).Append(",");
                    //sw.Append(r.NextShipment.ToString("yyyy-MM-dd")).Append(",");
                    sw.Append(r.NextShipment.HasValue ? r.NextShipment.Value.ToString("yyyy-MM-dd") : "N/A").Append(",");
                    sw.AppendLine();
                }
                return sw.ToString();
            }
        }



        public String ExportReportCost_Margin_By_Product(ReportsModel model)
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
                StringBuilder sw = new StringBuilder("SKUOrder,ProductName, Channel,Brand,Retail, COG ,Margin,Inventory");
                sw.AppendLine();

                //直接从DB读取转化为String返回去，少了一层实体类的转化，减少性能损耗....
                // Lee 2013年10月17日17:14:00
                var  memoryQuery = query.ToList();
                foreach (var r in memoryQuery)
                {
                    sw.Append(r.SKUOrder).Append(",");
                    sw.Append(r.Name.Replace(",", "-")).Append(",");
                    sw.Append(r.ChannelName).Append(",");
                    sw.Append(r.BrandName).Append(",");
                    sw.Append(r.NormalSelling.ToString("C", new CultureInfo("en-US")).Replace(",", "")).Append(",");
                    sw.Append(r.COG.ConvertToNotNull().ToString("C", new CultureInfo("en-US")).Replace(",", "")).Append(",");
                    sw.Append(r.Margin.ConvertToNotNull().ToString("C", new CultureInfo("en-US")).Replace(",", "")).Append(",");
                    sw.Append(r.Inventory).Append(",");
                    sw.AppendLine();
                }
                return sw.ToString();
            }
        }
    }


}
