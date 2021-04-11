//*****************************************************************************************************************************************************************************************
//											Modification history
//*****************************************************************************************************************************************************************************************
// C/A/D Change No   Author     Date        Description 

//	C	WL-1		Lee		    26/09/2013	The major goal of this search tool is to allow the relationships between HM #, 
//                                          SKU# and product names to become more clear. So, for example, if someone searches 
//                                          for a particular SKU, other SKUs that share the same HM # would also be returned 
//                                          in the results. The same is true for English name/word searches.(David)
// C    WL-2        Lee         27/09/2013  降低ProductSearch页面的数据库连接次数，从原来的40次降低到每次请求10次
// C    WL-3        Lee         2/14/2014   新增查询条件：按照用户修改进行查询(updateBy)
// C    WL-4        Lee         2/17/2014   新增一个到处Excel的方法，由于查询条件十分复杂，不建议再写一个新方法，以后需要维护两份。所以这里做个折衷。
//******************************************************************************************************************************************************************************************

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;

namespace PermaisuriCMS.BLL
{

    /// <summary>
    /// 用于多个Controller经常要使用到的方法，比如计算库存，获取Channel,Brand等列表的方法等
    /// </summary>
    public class ProductCommonServices
    {
        private readonly string _zeroMoeny = 0.ToString("C", new CultureInfo("en-US"));
        private readonly String _webPoUrl = ConfigurationManager.AppSettings["WebPOProductImageUrl"];
        private const String WebpoRelStr = "../../../"; //替换掉webPO数据库提取出来的路径前缀


        ///// <summary>
        ///// 重载方法：获取库存信息。提供给SearchProduct页面使用。以下参数全部都是搜索条件。由于需要新增一个参数指示是否导出Excel,而又不能影响旧的接口，所以这里
        ///// 对原有的方法进行重载，默认设置不需要导出Excel（isNeedExported=false);
        ///// </summary>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="queryModel"></param>
        ///// <param name="totalRecord"></param>
        ///// <param name="userInfo"></param>
        ///// <returns></returns>
        //public List<CMS_SKU_Model> GetProductInventorys(SKU_Query_Model queryModel, out int totalRecord, User_Profile_Model userInfo)
        //{
        //    string strCSV = string.Empty;
        //    totalRecord = 0;
        //  //  var query = GetProductInventorys(queryModel, out totalRecord, userInfo, false, out strCSV);
        //    return null;
        //}

        /// <summary>
        /// 获取库存信息。提供给SearchProduct页面使用。以下参数全部都是搜索条件
        /// </summary>
        /// <param name="queryModel">ProductInventory_V_Model</param>
        /// <param name="totalRecord"></param>
        /// <param name="userInfo">2014年1月6日12:33:32：新增用户模型，用来判断当前用户如果启用了渠道过滤，则需要再次对查询的结果再次过滤，以实现渠道控制的结果</param>
        /// <returns></returns>
        public List<CMS_SKU_Model> GetProductInventorys(SKU_Query_Model queryModel, out int totalRecord, User_Profile_Model userInfo)
        {


            var list = new List<CMS_SKU_Model>();

            using (var db = new PermaisuriCMSEntities())
            {

                var dbQuery = this.GetSKUListFromDB(queryModel, userInfo, db);
                totalRecord = dbQuery.Count();
                var query = dbQuery.Skip((queryModel.page - 1) * queryModel.rows).Take(queryModel.rows).ToList();

                //db.Configuration.LazyLoadingEnabled = false; //Object reference not set to an instance of an object. 2014年5月19日9:57:23

                list.AddRange(query.Select(p =>
                {
                    Debug.Assert(p.SKU_HM_Relation != null, "p.SKU_HM_Relation != null");
                    return new CMS_SKU_Model
                    {
                        SKUID = p.SKUID, SKU = p.SKU, ChannelID = p.ChannelID,
                        ChannelName = p.Channel == null ? "" : p.Channel.ChannelName, ProductName = p.ProductName,
                        Price = p.CMS_SKU_Costing?.SalePrice ?? 0,
                        strPrice = p.CMS_SKU_Costing == null
                            ? _zeroMoeny
                            : p.CMS_SKU_Costing.SalePrice.ToString("C", new CultureInfo("en-US")),

                        SKU_QTY = p.SKU_QTY,
                        StockByPcs = p.SKU_HM_Relation == null
                            ? 0
                            : p.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY.ConvertToNotNull(),
                        IsGroup = p.SKU_HM_Relation != null && p.SKU_HM_Relation.CMS_HMNUM.IsGroup.ConvertToNotNull(),
                        UpdateOn = p.UpdateOn.HasValue ? p.UpdateOn.Value.ToString("yyyy-MM-dd HH:mm:ss") : "N/A",
                        SKU_HM_Relation = new SKU_HM_Relation_Model
                        {
                            R_QTY = p.SKU_HM_Relation.R_QTY, StockKeyID = p.SKU_HM_Relation.StockKeyID, CMS_HMNUM =
                                new CMS_HMNUM_Model
                                {
                                    HMNUM = p.SKU_HM_Relation.CMS_HMNUM.HMNUM,
                                    ProductName = p.SKU_HM_Relation.CMS_HMNUM.ProductName,
                                    StockKeyQTY = p.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY
                                        .ConvertToNotNull(),
                                    Comments = p.SKU_HM_Relation.CMS_HMNUM.Comments,
                                    IsGroup = p.SKU_HM_Relation.CMS_HMNUM.IsGroup ?? false,
                                  
                                    Children_CMS_HMNUM_List = p.SKU_HM_Relation.CMS_HMNUM.CMS_HMGroup_Relation.Select(
                                        r => new CMS_HMNUM_Model
                                        {
                                            HMNUM = r.CMS_HMNUM_Children.HMNUM,
                                            ProductName = r.CMS_HMNUM_Children.ProductName,
                                            Comments = r.CMS_HMNUM_Children.Comments,
                                            StockKeyQTY = r.CMS_HMNUM_Children.CMS_HM_Inventory.StockkeyQTY
                                                .ConvertToNotNull()
                                        }).ToList()
                                }
                        },
                        CMS_Ecom_Sync = p.CMS_Ecom_Sync == null
                            ? null
                            : new CMS_Ecom_Sync_Model
                            {
                                Comments = p.CMS_Ecom_Sync.Comments, SKUID = p.CMS_Ecom_Sync.SKUID,
                                StatusDesc = p.CMS_Ecom_Sync.StatusDesc, StatusID = p.CMS_Ecom_Sync.StatusID,
                                UpdateBy = p.CMS_Ecom_Sync.UpdateBy, UpdateOn = p.CMS_Ecom_Sync.UpdateOn
                            }
                    };
                }));
                return list;
            }
        }

        
        //public string  ExoprtedSKUDataToServer(SKU_Query_Model queryModel, User_Profile_Model userInfo)
        //{
        //    using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
        //    {
        //        CMSNPOIServices NPOISvr = new CMSNPOIServices();
        //        var query = this.GetSKUListFromDB(queryModel, userInfo,db);
        //        //return NPOISvr.ExportSKU(query.AsEnumerable(), userInfo);
        //        return NPOISvr.ExportSKU(query, userInfo, db);
        //    }

        //}

        /// <summary>
        ///  独立出查询的公共部分，用于前端导出和查询的分开处理,需要注意的是返回以IQueryable列表的数据，dbContent类应当从调用方传递进来
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="userInfo"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private IQueryable<CMS_SKU> GetSKUListFromDB(SKU_Query_Model queryModel, User_Profile_Model userInfo, PermaisuriCMSEntities db)
        {

            // var query = db.CMS_SKU.AsQueryable();inClude一个减少10次查询.....
            //s.SKU_HM_Relation.Count>0表示排除掉那些异常的数据，即那些SKU没有关联HNNUM的数据！
            var query = db.CMS_SKU
                .Include(s => s.CMS_SKU_Costing)
                .Include(s => s.SKU_Status)
                .Include(s => s.Channel)
                .Include(s => s.Brand)
                .Include(s => s.SKU_HM_Relation)
                .Include(s => s.CMS_Ecom_Sync)
                //.Include(s => s.SKU_Media_Relation)
                //.Include(s => s.CMS_SKU_Category)
                //.Include(s => s.CMS_SKU_Material)
                //.Include(s => s.CMS_SKU_Colour)
                //.Include(s => s.SKU_HM_Relation.CMS_HMNUM)
                //.Include(s => s.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory)
                //.Include(s => s.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Costing)
                //.Include(s => s.SKU_HM_Relation.CMS_HMNUM.WebPO_Colour_V)
                //.Include(s => s.SKU_HM_Relation.CMS_HMNUM.WebPO_Material_V)
                //.Include(s => s.SKU_HM_Relation.CMS_StockKey)
                //.Include(s => s.SKU_HM_Relation.CMS_HMNUM.CMS_ProductCTN)
                //.Include(s => s.SKU_HM_Relation.CMS_HMNUM.CMS_ProductDimension)
                .Where(s => s.SKU_HM_Relation != null);
            if (queryModel.CMS_Ecom_Sync != null)
            {
                query = query.Where(s => s.CMS_Ecom_Sync != null && s.CMS_Ecom_Sync.StatusID != 1);//1代表Ecom更新成功
                if (queryModel.CMS_Ecom_Sync.StatusID > 0)
                {
                    query = query.Where(s => s.CMS_Ecom_Sync.SKUID == queryModel.CMS_Ecom_Sync.StatusID);
                }
            }
            switch (queryModel.InventoryType)
            {
                case 0:
                    break;
                case 1://In Stock  
                    //query = query.Where(q => q.ProductWebsiteRelation.Any(r => r.ProductInventoryNonRealTime_v.StockByPcs > 30));
                    // 如果   【1】：不加上count判断，那些没有和Product建立关联的SKU也会被查询进来并且，但是状态却显示Out of Stock
                    //       【2】:如果用any而不是All,由于一个SKU可以对应多个relation,当其中一个relation符合条件（而不是全部relation）的时候就会返回记录！
                    query = query.Where(q => q.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY > 30);

                    break;
                case 2://Low Inventory
                    query = query.Where(q => q.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY < 30 && q.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY > 1);
                    break;
                case 3://Out of Stock
                    query = query.Where(q => q.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY < 1);
                    break;
                case 4:// Low Inventory && Out of Stock (for Dahsboard )
                    //query = query.Where(q => q.ProductWebsiteRelation.All(r => r.ProductInventoryNonRealTime_v.StockByPcs < 30));
                    // 只要有一个relation是符合的就算
                    query = query.Where(q => q.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY < 30);
                    break;
            }

            switch (queryModel.multiplePartType)
            {
                case 1: //1: - query Not-Group
                    //query = query.Where(q => q.SKU_HM_Relation.Any(r => r.CMS_HMNUM.IsGroup == false || r.CMS_HMNUM.IsGroup == null));
                    query = query.Where(q => q.SKU_HM_Relation.CMS_HMNUM.IsGroup != true);
                    break;
                case 2://2:- query Group only
                    //query = query.Where(q => q.MultipleParts_V.SKUID == null);
                    query = query.Where(q => q.SKU_HM_Relation.CMS_HMNUM.IsGroup == true);
                    break;
            }

            //需要查询出所有引用到这个HM#/HMName相关的SKU！
            if (!string.IsNullOrEmpty(queryModel.Keywords))
            {

                //Step 1: 根据HM#/HMName 查询出所有的ProductWebsiteRelation表的列，然后再查询出HMNUM列
                // var ProductIDs = db.SKU_HM_Relation.Where(r => r.CMS_HMNUM.HMNUM.Contains(queryModel.Keywords)||
                var productIDs = db.SKU_HM_Relation.Where(r => r.CMS_HMNUM.HMNUM.StartsWith(queryModel.Keywords) ||
                    r.CMS_HMNUM.ProductName.Contains(queryModel.Keywords)).Select(r => r.ProductID).Distinct().ToArray();

                //Step 2: 根据HMNUM数组in出所有的WebSiteProdoucts对于的列
                //query = query.Where(q => q.SKU_HM_Relation.Any(r => ProductIDs.Contains(r.ProductID)));
                query = query.Where(q => productIDs.Contains(q.SKU_HM_Relation.ProductID));
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
                query = query.Where(r => hmiDs.Contains(r.SKU_HM_Relation.ProductID));

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
                //Is it possible to aggregate that data into a single figure? If not, we can find a substitute metric for this area.
                if (queryModel.Status == 10)
                {
                    query = query.Where(s => (s.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY < 1 && s.StatusID == 5) ||
                                       (s.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY > 0) && s.StatusID == 6);
                }
                else
                {
                    query = query.Where(q => q.StatusID == queryModel.Status);
                }
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
                        query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY) 
                            : query.OrderBy(q => q.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY);
                        break;
                    case 5:
                        query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.CMS_SKU_Costing.SalePrice) 
                            : query.OrderBy(q => q.CMS_SKU_Costing.SalePrice);
                        break;
                    case 6:
                        query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.Channel.ChannelName) : query.OrderBy(q => q.Channel.ChannelName);
                        break;

                    case 7://updateOn
                        query = queryModel.OrderType > 0 ? query.OrderByDescending(q => q.UpdateOn) : query.OrderBy(q => q.UpdateOn);
                        break;

                    default:
                        query = query.OrderBy(q => q.Channel.ChannelName);
                        break;
                }
            }
            else
            {
                //query = query.OrderBy(q =>q.ChannelID);
                query = query.OrderBy(q => q.Channel.ChannelName);
            }
            return query;

        }

       /// <summary>
        ///  获取所有渠道列表,由于增加了用户和渠道关展示的功能，所以新增了2个参数做判断
       /// </summary>
       /// <param name="isControl">判断当前用户是否有和渠道做了关联</param>
       /// <param name="userGuid">当前用户的GUID</param>
       /// <returns></returns>
        public List<Channel_Model> GetAllChannels(bool isControl, Guid userGuid)
        {
            var list = new List<Channel_Model>();
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.Channel.AsQueryable();
                if (isControl)
                {
                    /*神奇的是这里不用Include就可以直接查询出来，我猜是因为foreqch的时候没有去延迟查询relation表里面的User_Prfoile数据
                     * 2013年11月4日10:52:16 Lee
                     * SELECT  * FROM [dbo].[Channel] AS [Extent1] WHERE  EXISTS (SELECT 1 AS [C1]
	                   FROM [dbo].[User_Channel_Relation] AS [Extent2] WHERE ([Extent1].[ChannelID] = [Extent2].[ChannelID])
	                    AND ([Extent2].[User_Guid] = '6E421787-30AF-443B-950A-17081EE5F218'))*/

                    query = query.Where(q => q.User_Channel_Relation.Any(r => r.User_Guid == userGuid));
                }
                //query.OrderBy(c => c.ChannelName); 这样写无效...
                query = query.OrderBy(c => c.ChannelName);
                list.AddRange(query.Select(c => new Channel_Model
                {
                    API = c.API, ChannelID = c.ChannelID, ChannelName = c.ChannelName, Export2CSV = c.Export2CSV, ShortName = c.ShortName
                }));
                return list;
            }
        }

   
        /// <summary>
        /// 获取所有的品牌列表
        /// </summary>
        /// <returns></returns>
        public List<Brands_Info_Model> GetAllBrands()
        {
            var list = new List<Brands_Info_Model>();
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.Brand.OrderBy(b=>b.BrandName);
                list.AddRange(query.Select(b => new Brands_Info_Model
                {
                    Brand_Id = b.BrandID, Brand_Name = b.BrandName
                }));
                return list;
            }
        }

        /// <summary>
        /// For ProductConfiguration page AJAX Searching,
        /// ChangeDate:新增一个int类型用来指示:0-default:获取全部 1：获取非组合产品 ，2：仅获取组合产品
        /// </summary>
        /// <param name="hmModel"></param>
        /// <param name="skuid"></param>
        /// <returns></returns>
        public List<CMS_HMNUM_Model> GetProductsByKeyWords(CMS_HMNUM_Model hmModel, long skuid)
        {
            var list = new List<CMS_HMNUM_Model>();
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_HMNUM.AsQueryable();
                if (skuid > 0)
                {
                    query = query.Except(query.Where(p => p.SKU_HM_Relation.Any(r => r.SKUID == skuid)));
                }

                if (!String.IsNullOrEmpty(hmModel.ProductName))
                {
                    query = query.Where(p => p.ProductName.StartsWith(hmModel.ProductName));
                }

                //2014年3月18日：改变模糊搜索的方法从原来全量模糊改成现在的开头模糊
                if (!String.IsNullOrEmpty(hmModel.HMNUM))
                {
                    query = query.Where(p => p.HMNUM.StartsWith(hmModel.HMNUM));
                }

                if (hmModel.StatusID > 0) // 新增审核状态判断 2014年5月20日14:59:51
                {
                    query = query.Where(p => p.StatusID == hmModel.StatusID);
                }

                if (hmModel.HMType > 0) //2013年11月27日9:15:42
                {
                    switch (hmModel.HMType)
                    {
                        case 1: //非组合产品，注意需要查询为Null的历史数据
                            query = query.Where(p => p.IsGroup == false || p.IsGroup == null);
                            break;
                        case 2: //仅组合产品
                            query = query.Where(p => p.IsGroup == true);
                            break;
                        default:
                            break;
                    }
                }

                if (hmModel.ExcludedProductIDs != null && hmModel.ExcludedProductIDs.Count > 0)
                {
                    query = query.Except(query.Where(c => hmModel.ExcludedProductIDs.Contains(c.ProductID)));
                }

                query = query.Take(10);
                query = query.Include(h => h.CMS_HM_Inventory)
                        .Include(h => h.CMS_StockKey.MediaLibrary)
                        .Include(h => h.WebPO_Category_V)
                        .Include(h => h.WebPO_Material_V)
                        .Include(h => h.WebPO_Colour_V)
                        .Include(h => h.CMS_ShipViaType)
                    ;
                list.AddRange(from p in query
                    let img = db.WebPO_ImageUrls_V.FirstOrDefault(v => v.HMNUM == p.HMNUM)
                    select new CMS_HMNUM_Model
                    {
                        ProductID = p.ProductID, MasterPack = p.MasterPack,
                        strMasterPack = p.IsGroup == true ? "N/A" : p.MasterPack.ToString(), HMNUM = p.HMNUM,
                        ProductName = p.ProductName, StockKey = p.StockKey, StockKeyID = p.StockKeyID,
                        StockKeyQTY = p.CMS_HM_Inventory.StockkeyQTY.ConvertToNotNull(), // 库存 2013年11月24日18:17:06
                        //MaxImaSeq = p.MediaLibrary.Max(r => r.SerialNum)//Sequence contains no elements
                        MaxImaSeq = p.CMS_StockKey.MediaLibrary.FirstOrDefault() == null
                            ? 0
                            : p.CMS_StockKey.MediaLibrary.OrderByDescending(r => r.SerialNum).FirstOrDefault()
                                .SerialNum,
                        webSystemImage = img == null
                            ? null
                            : new OtherSystemImages
                            {
                                SmallPic = _webPoUrl + img.SmallPic.Replace(WebpoRelStr, ""),
                                Pic = _webPoUrl + img.Pic.Replace(WebpoRelStr, ""), SystemName = "WebPO System"
                            },
                        Category = p.WebPO_Category_V == null
                            ? null
                            : new WebPO_Category_V_Model
                            {
                                CategoryName = p.WebPO_Category_V.CategoryName,
                                ParentCategoryID = p.WebPO_Category_V.ParentCategoryID.ConvertToNotNull(),
                                ParentCategoryName = p.WebPO_Category_V.ParentCategoryName
                            },
                        HMMaterial = p.WebPO_Material_V == null
                            ? null
                            : new CMS_HMNUM_Material_Model
                            {
                                MaterialName = p.WebPO_Material_V.MaterialName
                            },
                        HMColour = p.WebPO_Colour_V == null
                            ? null
                            : new CMS_HMNUM_Colour_Model
                            {
                                ColourName = p.WebPO_Colour_V.ColourDesc
                            },
                        CMS_ShipVia_Type = p.CMS_ShipViaType == null
                            ? null
                            : new CMS_ShipViaType_Model
                            {
                                ShipViaTypeID = p.CMS_ShipViaType.ShipViaTypeID,
                                ShipViaTypeName = p.CMS_ShipViaType.ShipViaTypeName
                            }
                    });
                return list;
            }
        }


        /// <summary>
        /// 检查HMNUM是否存在，用户添加HMNUM,SKU和Product Configuration页面Autocompleted做校验。
        /// CreateDate:2014年2月18日10:41:08
        /// </summary>
        /// <param name="hmnum"></param>
        /// <returns></returns>
        public CMS_HMNUM_Model CheckHmnum(string hmnum)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var hm = db.CMS_HMNUM.FirstOrDefault(h => h.HMNUM == hmnum);
                if (hm == null)
                {
                    return null;
                }
                var img = db.WebPO_ImageUrls_V.FirstOrDefault(v => v.HMNUM == hmnum);
                return new CMS_HMNUM_Model
                {
                    ProductID = hm.ProductID,
                    MasterPack = hm.MasterPack,
                    HMNUM = hm.HMNUM,
                    ProductName = hm.ProductName,
                    StockKey = hm.StockKey,
                    StockKeyID = hm.StockKeyID,
                    StatusID = hm.StatusID,
                    StockKeyQTY = hm.CMS_HM_Inventory.StockkeyQTY.ConvertToNotNull(),
                    strMasterPack = hm.MasterPack.ToString(),
                    MaxImaSeq = hm.CMS_StockKey.MediaLibrary.FirstOrDefault() == null ? 0 : hm.CMS_StockKey.MediaLibrary.OrderByDescending(r => r.SerialNum).FirstOrDefault().SerialNum,
                    webSystemImage = img == null ? null : new OtherSystemImages
                    {
                        SmallPic = _webPoUrl + img.SmallPic.Replace(WebpoRelStr, ""),
                        Pic = _webPoUrl + img.Pic.Replace(WebpoRelStr, ""),
                        SystemName = "WebPO System"
                    }
                };
            }
        }

    }


}
