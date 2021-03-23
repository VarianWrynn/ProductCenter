using System;
using System.Collections.Generic;
using System.Linq;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace PermaisuriCMS.BLL
{
    public class ProductSearchServices
    {
        /// <summary>
        /// Change1:新增类型4、5 。 这个页面比较干净 不迁移到Common页面去了 2014年4月8日
        /// </summary>
        /// <param name="term"></param>
        /// <param name="type">1:hmnum+hmName; 2:SKU+SKUName; 3:ModifedName，4:HMNUM Only,5 SKUOrder Only</param>
        /// <returns></returns>
        public List<String> GetAutoCompeltedInfo(string term, int type)
        {

            using (var db = new PermaisuriCMSEntities())
            {
                switch (type)
                {
                    case 1:
                        var hMs = db.CMS_HMNUM.Where(h => h.HMNUM.StartsWith(term))//取出前10列的HMNUM
                            .OrderByDescending(h => h.ProductID).Take(10).Select(h => h.HMNUM).Distinct().ToList();

                        var hmNames = db.CMS_HMNUM.Where(h => h.ProductName.StartsWith(term))//取出前10列的HMNUM Name
                           .OrderByDescending(h => h.ProductID).Take(10).Select(h => h.ProductName).Distinct().ToList();

                        hMs.AddRange(hmNames);//合并
                        return hMs.Take(10).ToList();//再取合并后的前10
                    case 2:
                        var skUs = db.CMS_SKU.Where(h => h.SKU.StartsWith(term))//取出前10列
                           .OrderByDescending(h => h.SKUID).Take(10).Select(h => h.SKU).Distinct().ToList();

                        var skuNames = db.CMS_HMNUM.Where(h => h.ProductName.StartsWith(term))//取出前10列
                           .OrderByDescending(h => h.ProductID).Take(10).Select(h => h.ProductName).Distinct().ToList();

                        skUs.AddRange(skuNames);//合并
                        return skUs.Take(10).ToList();//再取合并后的前10

                    case 3:
                        var query = db.CMS_User_Profile_V.Where(h => h.User_Account.StartsWith(term)).OrderBy(h=>h.User_Account).Take(10).Select(h=>h.User_Account);
                        return query.ToList();

                    case 4:
                        var hMsOnly = db.CMS_HMNUM.Where(h => h.HMNUM.StartsWith(term))//取出前10列的HMNUM
                           .OrderByDescending(h => h.ProductID).Take(10).Select(h => h.HMNUM).Distinct().ToList();
                        return hMsOnly;

                    case 5:
                        var skUsOnly = db.CMS_SKU.Where(h => h.SKU.StartsWith(term))//取出前10列
                           .OrderByDescending(h => h.SKUID).Take(10).Select(h => h.SKU).Distinct().ToList();

                        return skUsOnly;

                    default:
                        return null;
                }
            }
        }


        /// <summary>
        /// 由于导出Excel数据用EF关联10多张表非常的慢，现在采用视图实现，前端的ProdctSearch查询有时间也必须改成和这个同步！
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public string ExoprtedSKUDataToServer(SKU_Query_Model queryModel, User_Profile_Model userInfo)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var adapter = (IObjectContextAdapter)db;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 5 * 60; // value in seconds

                var query = db.Export2Excel.AsQueryable();
                /*在这里直接include会导致8000+条数据都先去提取尺寸箱柜再做下面的过滤，这样不合适！2014年5月2日16:14:48*/
                //.Include(e => e.CMS_HMNUM)
                //.Include(e => e.CMS_HMNUM.CMS_ProductCTN)
                //.Include(e => e.CMS_HMNUM.CMS_ProductDimension).AsQueryable();

                switch (queryModel.InventoryType)
                {
                    case 0:
                        break;
                    case 1://In Stock  
                        //query = query.Where(q => q.ProductWebsiteRelation.Any(r => r.ProductInventoryNonRealTime_v.StockByPcs > 30));
                        // 如果   【1】：不加上count判断，那些没有和Product建立关联的SKU也会被查询进来并且，但是状态却显示Out of Stock
                        //       【2】:如果用any而不是All,由于一个SKU可以对应多个relation,当其中一个relation符合条件（而不是全部relation）的时候就会返回记录！
                        query = query.Where(q => q.StockkeyQTY > 30);

                        break;
                    case 2://Low Inventory
                        query = query.Where(q => q.StockkeyQTY < 30 && q.StockkeyQTY > 1);
                        break;
                    case 3://Out of Stock
                        query = query.Where(q => q.StockkeyQTY < 1);
                        break;
                    case 4:// Low Inventory && Out of Stock (for Dahsboard )

                        query = query.Where(q => q.StockkeyQTY < 30);
                        break;
// ReSharper disable RedundantEmptyDefaultSwitchBranch
                    default:
                        break;
// ReSharper restore RedundantEmptyDefaultSwitchBranch
                }

                switch (queryModel.multiplePartType)
                {
                    case 1: //1: - query Not-Group
                        //query = query.Where(q => q.SKU_HM_Relation.Any(r => r.CMS_HMNUM.IsGroup == false || r.CMS_HMNUM.IsGroup == null));
                        query = query.Where(q => q.IsGroup != true);
                        break;
                    case 2://2:- query Group only
                        //query = query.Where(q => q.MultipleParts_V.SKUID == null);
                        query = query.Where(q => q.IsGroup == true);
                        break;
// ReSharper disable RedundantEmptyDefaultSwitchBranch
                    default: break;
// ReSharper restore RedundantEmptyDefaultSwitchBranch
                }

                if (!string.IsNullOrEmpty(queryModel.Keywords))
                {

                    //Step 1: 根据HM#/HMName 查询出所有的ProductWebsiteRelation表的列，然后再查询出HMNUM列
                    // var ProductIDs = db.SKU_HM_Relation.Where(r => r.CMS_HMNUM.HMNUM.Contains(queryModel.Keywords)||
                    var productIDs = db.SKU_HM_Relation.Where(r => r.CMS_HMNUM.HMNUM.StartsWith(queryModel.Keywords) ||
                        r.CMS_HMNUM.ProductName.Contains(queryModel.Keywords)).Select(r => r.ProductID).Distinct().ToArray();

                    //Step 2: 根据HMNUM数组in出所有的WebSiteProdoucts对于的列
                    //query = query.Where(q => q.SKU_HM_Relation.Any(r => ProductIDs.Contains(r.ProductID)));
                    query = query.Where(q => productIDs.Contains(q.ProductID));
                }

                //需要查询出所有引用到这个SKU/SKUOrder相关的SKU！
                if (!string.IsNullOrEmpty(queryModel.SKUOrder))
                {
                    /*需要查询出当前SKU关联的HMNUM，再查询出HMNUM关联的所有的SKU的选项*/

                    /*以下这种查询方式导致超时！*/
                    //query = query.Where(r => r.SKU_HM_Relation.CMS_HMNUM.SKU_HM_Relation.Any(rr => rr.CMS_SKU.SKU.StartsWith(queryModel.SKUOrder)) ||
                    //    r.SKU_HM_Relation.CMS_HMNUM.SKU_HM_Relation.Any(rr => rr.CMS_SKU.ProductName.StartsWith(queryModel.SKUOrder)));

                    //1.先在关系表里面查询出所有包含这个关键字的HMNUMID 2...
                    var hmiDs = db.SKU_HM_Relation.Where(r => r.CMS_SKU.SKU.StartsWith(queryModel.SKUOrder) ||
                        r.CMS_SKU.ProductName.StartsWith(queryModel.SKUOrder)).Select(r => r.ProductID).ToList();
                    query = query.Where(r => hmiDs.Contains(r.ProductID));

                }

                if (!String.IsNullOrEmpty(queryModel.UpdateBy))//新增：按照用户查询。2014年2月14日16:23:18
                {
                    query = query.Where(q => q.UpdateBy.StartsWith(queryModel.UpdateBy));
                }
                if (queryModel.ChannelID > 0)
                {
                    query = query.Where(q => q.ChannelID == queryModel.ChannelID);
                }
                else if (userInfo.IsChannelControl)//如果当前用户开启了渠道控制，即使是要求查询全部，也要做渠道控制，如果只查询一个，大家都一样
                {
                    var mChannels = db.Channel.Where(q => q.User_Channel_Relation.Any(r => r.User_Guid == userInfo.User_Guid)).Select(q => q.ChannelID).ToArray();
                    query = query.Where(q => mChannels.Contains(q.ChannelID));
                }

                if (queryModel.BrandID > 0)
                {
                    query = query.Where(q => q.BrandID == queryModel.BrandID);
                }
                if (queryModel.CategoryID > 0)//2014年4月2日 lee
                {
                    query = query.Where(q => q.CategoryID == queryModel.CategoryID);
                }
                if (queryModel.Status > 0)
                {
                    //Items requiring attention
                    // The intention behind "Items Requiring Attention" was to display the number of products in a
                    // "conflicted status". For example, items in stock but not online, or items out of stock but still online.
                    // Is it possible to aggregate that data into a single figure? If not, we can find a substitute metric for this area.
                    query = queryModel.Status == 10 ? query.Where(s => (s.StockkeyQTY < 1 && s.StatusID == 5) || (s.StockkeyQTY > 0) && s.StatusID == 6) : query.Where(q => q.StatusID == queryModel.Status);
                }

                if (queryModel.OrderBy > 0)
                {
                    switch (queryModel.OrderBy)
                    {
                        case 1:
                            query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.ProductName) : query.OrderBy(q => q.ProductName);
                            break;
                        case 2:
                            query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.SKU) : query.OrderBy(q => q.SKU);
                            break;
                        case 3:
                            query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.SKU_QTY) : query.OrderBy(q => q.SKU_QTY);
                            break;
                        case 4:
                            query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.StockkeyQTY) : query.OrderBy(q => q.StockkeyQTY);
                            break;
                        case 5:
                            query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.SalePrice) : query.OrderBy(q => q.SalePrice);
                            break;
                        case 6:
                            query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.ChannelName) : query.OrderBy(q => q.ChannelName);
                            break;

                        case 7://updateOn
                            query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.UpdateOn) : query.OrderBy(q => q.UpdateOn);
                            break;

                        default:
                            query = query.OrderBy(q => q.ChannelName);
                            break;
                    }
                }
                else
                {
                    //query = query.OrderBy(q =>q.ChannelID);
                    query = query.OrderBy(q => q.ChannelName);
                }

                var npoiSvr = new CMSNPOIServices();

                query = query.Include(e => e.CMS_HMNUM)
                 .Include(e => e.CMS_HMNUM.CMS_ProductCTN)
                 .Include(e => e.CMS_HMNUM.CMS_ProductDimension);

                return npoiSvr.ExportSKU(query.ToList(), userInfo);
            }

        }
       
    }
}
