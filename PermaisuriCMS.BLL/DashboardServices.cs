using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Caching;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;

namespace PermaisuriCMS.BLL
{
    public class DashboardServices
    {
        /// <summary>
        /// Get PO Oders Datas
        /// 暂时作废，作废时间 2013年12月17日16:53:35 Lee
        /// </summary>
        /// <returns></returns>
        public List<PO_Order_V_Model> GetPoOrders()
        {
            var list = new List<PO_Order_V_Model>();
            using (var db = new PermaisuriCMSEntities())
            {
                //var query = db.GetPOOrderList(ids);
                var query = db.PO_Order_V.Where(v => v.MerchantID !=null).OrderBy(s => s.OrderYear).ThenBy(s=>s.OrderMonth);
                list.AddRange(query.Select(po => new PO_Order_V_Model
                {
                    MerchantID = po.MerchantID, OrderDate = new DateTime(po.OrderYear, po.OrderMonth, 1), Orders = po.Orders.HasValue ? po.Orders.Value : 0
                }));
            }
            return list;
        }

        
        /// <summary>
        /// 获取Ecom订单的记录，原先由视图获取，但是视图无法按照条件过滤，所以这里改用StoreProcedure来过滤
        /// 如果在服务器适应符合HightCharts格式的数据，需要双循环，所以服务器给出基础数据，由客户端自己去格式化.......2013年12月18日12:06:39
        /// CreateDate:2013年12月17日16:52:42
        /// </summary>
        /// <param name="channelList"></param>
        /// <param name="sTime">开始时间</param>
        /// <param name="eTime">结束时间</param>
        /// <returns></returns>
        public List<Ecom_Order_SP_Model> GetEcomOrder(Int32[] channelList, String sTime, String eTime)
        {

            var list = new List<Ecom_Order_SP_Model>();
            using (var db = new PermaisuriCMSEntities())
            {
                ////var ecomOrder = db.Ecom_Order_SP(arrChannel.TrimEnd(','), sTime, eTime).ToList(); 加了in语句反而变慢了很多，所以取消掉 在EF内存里面做过滤 2013年12月20日10:05:17
                //var orderCache = CacheHelper.Get<List<Ecom_Order_SP_Model>>(orderCacheKey);
                //if (orderCache==null)
                //{
                //    CacheHelper.Insert(orderCacheKey, db.Ecom_Order_SP("2001-01-01", "2015-12-31").AsEnumerable(), null);
                //    CacheHelper.SaveTime = 30;
                //    orderCache = CacheHelper.Get<List<Ecom_Order_SP_Model>>(orderCacheKey);
                //}
                ////var ecomOrder = db.Ecom_Order_SP(sTime, eTime).ToList();
                //var ecomOrder = orderCache.Where(r=>r.);

                //var ecomOrder = db.Ecom_Order_SP(arrChannel.TrimEnd(','), sTime, eTime).ToList(); 加了in语句反而变慢了很多，所以取消掉 在EF内存里面做过滤 2013年12月20日10:05:17

                var ecomOrder = db.Ecom_Order_SP(sTime, eTime).ToList();
                if (channelList.Length > 0)
                {
                    ecomOrder = ecomOrder.Where(o => channelList.Contains(o.ChannelID)).ToList();
                }
                list.AddRange(ecomOrder.Select(po => new Ecom_Order_SP_Model
                {
                    name = po.MerchantID,
                    //x = (new DateTime(po.OrderYear, po.OrderMonth, 1).ToUniversalTime().Ticks-DatetimeMinTimeTicks)/1000,
                    x = new DateTime(po.OrderYear, po.OrderMonth, 1), y = po.Orders.ConvertToNotNull()
                }));
            }
            return list;
        }




        public int GetAvgDailyOrders(string startDate, string endDate)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                //注意，第一个输出参数的名称必须与Store Procedure里面的输出参数的名称一一对应！不区分大小写
                var avgOrderOut = new ObjectParameter("AvgOrder", DbType.Int32);
                db.Ecom_AvgOrder_SP(startDate, endDate, avgOrderOut);
                return Convert.ToInt32(avgOrderOut.Value);
            }
        }

        public decimal GetAvgOrderAmount(string startDate, string endDate)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                //注意，第一个输出参数的名称必须与Store Procedure里面的输出参数的名称一一对应！不区分大小写
                var retVal = db.Ecom_AvgOrderAmount_SP(startDate, endDate);
                return retVal.First().AvgOrderAmount.ConvertToNotNull();
            }
        }

        /// <summary>
        /// Products in Development Count
        /// </summary>
        /// <returns></returns>
        public int GetProductsDevCount()
        {
            using (var db = new PermaisuriCMSEntities())
            {
                //List<int> pStatuss = new List<int>();
                //pStatuss.Add(ProductStatus.New.GetHashCode());
                //pStatuss.Add(ProductStatus.NewDuplicated.GetHashCode());
                //pStatuss.Add(ProductStatus.MediaCreation.GetHashCode());
                //pStatuss.Add(ProductStatus.MarketingDevelopment.GetHashCode());
                //var arrStatus = pStatuss.ToArray();
                var query = db.CMS_SKU.Where(p => p.StatusID != 5);
                return query.Count();
            }
        }

        /// <summary>
        /// Products in Development List
        /// </summary>
        /// <returns></returns>
        public List<CMS_SKU_Model> GetProductsDevList()
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_SKU.Include(p => p.SKU_Status).Where(p => p.StatusID != 5).OrderBy(p => p.ChannelID).Take(10).ToList()
                    .Select(p => new CMS_SKU_Model 
                    {
                        SKUID = p.SKUID,
                        ProductName = p.ProductName,
                        SKU = p.SKU,
                        ChannelID = p.ChannelID,
                        ChannelName = p.Channel.ChannelName,
                        strModify_Date = p.UpdateOn.HasValue ? p.UpdateOn.Value.ToString("yyyy-MM-dd") : "N/A",
                        StatusID = p.StatusID,
                        StatusName = p.SKU_Status.StatusName
                    });
                return query.ToList();
            }
        }


        /// <summary>
        /// Items requiring attention
        /// The intention behind "Items Requiring Attention" was to display the number of products in a
        /// "conflicted status". For example, items in stock but not online, or items out of stock but still online.
        /// Is it possible to aggregate that data into a single figure? If not, we can find a substitute metric for
        /// this area.
        /// Author:David, Date:2013-10-24
        /// </summary>
        /// <returns></returns>
        public int GetAttentionItemsCount()
        {
            using (var db = new PermaisuriCMSEntities())
            {
                //var query = db.CMS_SKU.Include(w => w.SKU_HM_Relation).
                //    Where(s => (s.SKU_HM_Relation.Any(r => r.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY < 1) && s.StatusID == 5) ||
                //    (s.SKU_HM_Relation.Any(r => r.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY > 0) && s.StatusID == 6));

                var query = db.CMS_SKU.Include(w => w.SKU_HM_Relation).
                  Where(s => (s.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY < 1 && s.StatusID == 5) ||
                  (s.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY > 0 && s.StatusID == 6));

                return query.Count();
            }
        }

        /// <summary>
        /// 获取低库存数据
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="totalRecord"></param>
        /// <param name="page"></param>
        /// <param name="useInfo"></param>
        /// <returns></returns>
        public List<CMS_SKU_Model> GetLowInventory(int page, int rows, out int totalRecord, User_Profile_Model useInfo)
        {
            return new ProductCommonServices().GetProductInventorys(new SKU_Query_Model
            {
                page = page,
                rows = rows,
                Status = -1, //-1表示所有的状态都查询出来不分激活或者未激活的
                InventoryType = 4
            }, out totalRecord, useInfo);
        }
    }
}
