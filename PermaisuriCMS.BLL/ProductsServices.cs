using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using System.Globalization;
using System.Reflection;


namespace PermaisuriCMS.BLL
{
    /// <summary>
    /// 这个类用于旧页面的兼容，已经不在使用，请调用ProductConfigurationServices页面
    /// </summary>
    public class ProductsServices
    {

        private readonly string _zeroMoeny = 0.ToString("C", new CultureInfo("en-US"));
        private const string ModelName = "ProductConfiguration";


        /// <summary>
        /// 为渠道产品和原有产品建立关联。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="SKUID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool InsertProductPieces(CMS_HMNUM_Model model, long SKUID, User_Profile_Model user, out string msg)
        {
            msg = "";
            //using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            //{
            //    int retVal = 0;
            //    var query = db.SKU_HM_Relation.FirstOrDefault(r => r.SKUID == SKUID);
            //    if (query == null)
            //    {
            //        query = new SKU_HM_Relation
            //        {
            //            SKUID = SKUID,
            //            ProductID = model.ProductID,
            //            R_QTY = model.SKU_HM_Relation,
            //            CreateBy = user.User_Account, 
            //            CreateOn = DateTime.Now,
            //            UpdateBy = user.User_Account,
            //            UpdateOn = DateTime.Now
            //        };
            //        db.SKU_HM_Relation.Add(query);
            //        retVal = db.SaveChanges();
            //    }
            //    else
            //    {
            //        query.ProductID = model.ProductID;
            //        query.R_QTY = model.RelationQTY;
            //        query.UpdateOn = DateTime.Now;
            //        query.UpdateBy = user.User_Account;
            //    }
            //    return retVal >= 0;
            return false;
        }

        /// <summary>
        /// 编辑渠道产品和原有产品建立关联。思路： 首先我们一定要根据RID查询出关系表的这个数据，如果不存在则提示数据不存在，存在则对这个数据进行更新！
        /// Date:2013年11月12日10:32:08
        /// 该方法目前停用，前端字段已经设置成不可修改了
        /// </summary>
        /// <param name="model"></param>
        /// <param name="SKUID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateProductPieces(CMS_HMNUM_Model model, long SKUID, User_Profile_Model user, out string msg)
        {
            msg = "";
            //using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            //{
            //    int retVal = 0;
            //    var query = db.SKU_HM_Relation.FirstOrDefault(r => r.SKUID == SKUID);
            //    if (query == null)
            //    {
            //        msg = "this item does NOT exist";
            //        return retVal > 0;
            //    }
            //    query.ProductID = model.ProductID;
            //    query.R_QTY = model.RelationQTY;
            //    query.UpdateOn = DateTime.Now;
            //    query.UpdateBy = user.User_Account;
            //    retVal = db.SaveChanges();
            //    return retVal >= 0;
            //}

            return false;
        }

        /// <summary>
        /// 修改SKUOrder和HM#关联表的数量。该方法提供给ProductConfiguration页面选择“数量"下拉单的时候使用。前台只传递2个参数，
        /// 一个是关系表的ID，一个是下拉单选择的数量
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateSellPack(SKU_HM_Relation_Model model, out string msg, User_Profile_Model user)
        {
            msg = "";
            using (var db = new PermaisuriCMSEntities())
            {
                var retVal = 0;
                var query = db.SKU_HM_Relation.FirstOrDefault(r => r.SKUID == model.SKUID);
                if (query == null)
                {
                    msg = "this item does not exist";
                    return retVal > 0;
                }

                var oldValue = query.R_QTY;
                query.R_QTY = model.R_QTY;
                var newValue = model.R_QTY;

                string logDesc = string.Empty;
                logDesc += " <b> [Pieces:] </b> : old value =  <span style='color:red'>  " + oldValue + " </span> , new Value = <span style='color:red'>  " + newValue + "  </span>";
                logDesc += "<br>";

                BllExtention.DbRecorder(new LogOfUserOperatingDetails
                {
                    ModelName = ModelName,
                    ActionName = MethodBase.GetCurrentMethod().Name,
                    ActionType = LogActionTypeEnum.Update.GetHashCode(),

                    SKUID = query.SKUID,
                    SKU = query.CMS_SKU.SKU,
                    ChannelID = query.CMS_SKU.ChannelID,
                    ChannelName = query.CMS_SKU.Channel.ChannelName,
                    ProductID = query.ProductID,
                    HMNUM = query.CMS_HMNUM.HMNUM,

                    Descriptions = logDesc,
                    CreateBy = user.User_Account,
                    CreateOn = DateTime.Now
                });

                retVal = db.SaveChanges();
                return retVal >= 0;
            }
        }


        /// <summary>
        /// 在ProductConfiguration页面删除ProductPieces
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteProductPieces(long SKUID)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var relaton = new SKU_HM_Relation { SKUID = SKUID };
                db.Set<SKU_HM_Relation>().Attach(relaton);
                db.SKU_HM_Relation.Remove(relaton);
                return db.SaveChanges() > 0;
            }
        }


        /// <summary>
        /// 为颜色材料类别MCCC四个字段获取智能提示下拉单： CreatedDate:2014年3月26日10:30:35
        /// 大类的ParentID从原来的不指定改成显式的指定为0,用于解决当前端输入的非一级目录的时候，也把信息返回给前端的BUG （Linda)。Changed Date:2014年6月3日10:51:50 //WL-1
        /// </summary>
        /// <param name="term"></param>
        /// <param name="type"></param>
        /// <param name="parentId">只有查询SubCategory的时候这个字段才有意义</param>
        /// <returns></returns>
        public List<MCCC> GetAutoCompeltedMCCC(string term, string type, long parentId)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                switch (type)
                {
                    case "Material":
                        return db.CMS_SKU_Material.Where(m => m.MaterialName.StartsWith(term))
                            .OrderByDescending(m => m.MaterialID).Take(10)
                            .Select(m => new MCCC
                            {
                                ID = m.MaterialID,
                                Name = m.MaterialName
                            }).ToList();
                    case "Colour":
                        return db.CMS_SKU_Colour.Where(m => m.ColourName.StartsWith(term))
                            .OrderByDescending(m => m.ColourID).Take(10)
                           .Select(m => new MCCC
                           {
                               ID = m.ColourID,
                               Name = m.ColourName
                           }).ToList();
                    case "Category":
                        return db.CMS_SKU_Category.Where(m => m.ParentID == 0 && m.CategoryName.StartsWith(term))//WL-1
                           .OrderByDescending(m => m.CategoryID).Take(10)
                          .Select(m => new MCCC
                          {
                              ID = m.CategoryID,
                              Name = m.CategoryName
                          }).ToList();
                    case "SubCategory":
                        return db.CMS_SKU_Category.Where(m => m.ParentID == parentId && m.CategoryName.StartsWith(term))
                            .OrderByDescending(m => m.CategoryID).Take(10)
                            .Select(m => new MCCC
                             {
                                 ID = m.CategoryID,
                                 Name = m.CategoryName
                             }).ToList();
                    default:
                        return null;
                }
            }
        }

        public bool CheckMCC(MCCC mcModel, ref string msg)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var mat = db.CMS_SKU_Material.FirstOrDefault(s => s.MaterialName == mcModel.Material);
                if (mat == null)
                {
                    msg = "Material";
                    return false;
                }
                var col = db.CMS_SKU_Colour.FirstOrDefault(s => s.ColourName == mcModel.Colour);
                if (col == null)
                {
                    msg = "Colour";
                    return false;
                }
                var cat = db.CMS_SKU_Category.FirstOrDefault(s => s.CategoryName == mcModel.Category);
                if (cat == null)
                {
                    msg = "Category";
                    return false;
                }
                var query = db.CMS_SKU_Category.FirstOrDefault(s => s.CategoryName == mcModel.SubCategory);
                if (query == null)
                {
                    msg = "SubCategory";
                    return false;
                }
                return true;
            }
        }


        /// <summary>
        /// 根据ID来获取当前SKU产品，目前主要提供给HMNUM页面连接查询
        /// CrateDate:2013年11月21日9:38:24
        /// </summary>
        /// <param name="skuid"></param>
        /// <returns></returns>
        public CMS_SKU_Model GetSingleSKU(long skuid)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_SKU.Include(w => w.Brand)
                    .Include(w => w.Channel)
                    .Include(w => w.CMS_SKU_Costing)
                    .Include(w => w.CMS_ShipViaType)
                    .Include(w => w.CMS_SKU_Category)
                    .Include(w => w.CMS_SKU_Category_Sub)
                    .Include(w => w.CMS_SKU_Colour)
                    .Include(w => w.CMS_SKU_Material)
                    .Include(w => w.SKU_HM_Relation)
                    .Include(w => w.SKU_HM_Relation.CMS_HMNUM)//2014年5月13日17:57:55 经过测测试，这样子reference无效....
                    .Include(w => w.SKU_HM_Relation.CMS_HMNUM.CMS_StockKey)
                    .Include(w => w.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Costing)
                    .Include(w => w.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V)
                    .Include(w => w.SKU_HM_Relation.CMS_HMNUM.WebPO_Colour_V)
                    .Include(w => w.SKU_HM_Relation.CMS_HMNUM.WebPO_Material_V)
                    .Include(w => w.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory)
                    .Include(w => w.SKU_HM_Relation.CMS_HMNUM.CMS_HMGroup_Relation)
                    .Include(w => w.SKU_HM_Relation.CMS_HMNUM.CMS_ProductCTN)
                    .Include(w => w.SKU_HM_Relation.CMS_HMNUM.CMS_ProductDimension)
                    .FirstOrDefault(w => w.SKUID == skuid);

                return query == null ? null : new CMS_SKU_Model
                {
                    SKUID = query.SKUID,
                    ChannelID = query.ChannelID,
                    ChannelName = query.Channel.ChannelName,
                    Keywords = query.Keywords,
                    URL = query.URL,
                    ProductDesc = query.ProductDesc,
                    ProductName = query.ProductName,
                    SKU = query.SKU,
                    SKU_QTY = query.SKU_QTY,
                    Specifications = query.Specifications,
                    StatusID = query.StatusID,
                    UPC = query.UPC,
                    Visibility = query.Visibility.HasValue ? query.Visibility.Value : 0,
                    BrandID = query.Brand.BrandID,
                    BrandName = query.Brand.BrandName,
                    RetailPrice = query.RetailPrice,
                    strRetailPrice = query.RetailPrice.ToString("C", new CultureInfo("en-US")),
                    pImage = query.SKU_Media_Relation.FirstOrDefault(m => m.PrimaryImage == true) == null ? "" :
                    query.SKU_Media_Relation.FirstOrDefault(m => m.PrimaryImage == true).MediaLibrary.ImgName,
                    pMedia = query.SKU_Media_Relation.FirstOrDefault(m => m.PrimaryImage == true) == null ? null :
                      new MediaLibrary_Model
                      {
                          fileFormat = query.SKU_Media_Relation.FirstOrDefault(m => m.PrimaryImage == true).MediaLibrary.fileFormat,
                          HMNUM = query.SKU_Media_Relation.FirstOrDefault(m => m.PrimaryImage == true).MediaLibrary.HMNUM,
                          ImgName = query.SKU_Media_Relation.FirstOrDefault(m => m.PrimaryImage == true).MediaLibrary.ImgName,
                          ProductID = query.SKU_Media_Relation.FirstOrDefault(m => m.PrimaryImage == true).MediaLibrary.ProductID,
                          MediaID = query.SKU_Media_Relation.FirstOrDefault(m => m.PrimaryImage == true).MediaLibrary.MediaID,
                          SerialNum = query.SKU_Media_Relation.FirstOrDefault(m => m.PrimaryImage == true).MediaLibrary.SerialNum
                      },
                    IsGroup = query.SKU_HM_Relation != null && query.SKU_HM_Relation.CMS_HMNUM.IsGroup.ConvertToNotNull(),
                    UpdateOn = query.UpdateOn.HasValue ? query.UpdateOn.Value.ToString("yyyy-MM-dd HH:mm:ss") : "N/A",
                    SKU_Costing = new CMS_SKU_Costing_Model
                    {
                        SKUCostID = query.CMS_SKU_Costing == null ? 0 : query.CMS_SKU_Costing.SKUCostID,
                        SalePrice = query.CMS_SKU_Costing == null ? _zeroMoeny : query.CMS_SKU_Costing.SalePrice.ToString("C", new CultureInfo("en-US")),
                        EstimateFreight = query.CMS_SKU_Costing == null ? _zeroMoeny : query.CMS_SKU_Costing.EstimateFreight.ToString("C", new CultureInfo("en-US")),
                        EffectiveDate = query.CMS_SKU_Costing.EffectiveDate
                    },

                    /*Add new Type for SKU 2014年1月11日10:45:00*/
                    Colour = new CMS_SKU_Colour_Model
                    {
                        ColourID = query.ColourID,
                        ColourName = query.CMS_SKU_Colour == null ? "NONE" : query.CMS_SKU_Colour.ColourName
                    },
                    Material = new CMS_SKU_Material_Model
                    {
                        MaterialID = query.MaterialID,
                        MaterialName = query.CMS_SKU_Material == null ? "NONE" : query.CMS_SKU_Material.MaterialName
                    },
                    Category = new CMS_SKU_Category_Model
                    {
                        CategoryID = query.CategoryID,
                        CategoryName = query.CMS_SKU_Category == null ? "NONE" : query.CMS_SKU_Category.CategoryName
                    },
                    SubCategory = new CMS_SKU_Category_Model
                    {
                        CategoryID = query.SubCategoryID,
                        CategoryName = query.CMS_SKU_Category_Sub == null ? "NONE" : query.CMS_SKU_Category_Sub.CategoryName
                    },

                    ////获取状态，用于下拉单展示，注意这个是单独在另外一张报查询，无关联当前表 该方法转移到CMSCacheableDataController页面实现 2014年5月13日10:57:27
                    //StatusList = db.SKU_Status.Select(s => new SKU_Status_Model
                    //{
                    //    StatusID = s.StatusID,
                    //    StatusName = s.StatusName
                    //}).ToList(),

                    ShipViaTypeID = query.ShipViaTypeID.HasValue ? query.ShipViaTypeID.Value : 0,
                    CMS_ShipViaType = query.CMS_ShipViaType == null ? null : new CMS_ShipViaType_Model
                    {
                        ShipViaTypeID = query.CMS_ShipViaType.ShipViaTypeID,
                        ShipViaTypeName = query.CMS_ShipViaType.ShipViaTypeName,
                        CMS_ShipVia_Default = query.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true) == null ? null : new CMS_ShipVia_Model
                        {
                            SHIPVIA = query.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true).SHIPVIA
                        },

                        //eCom同步的SKU提取应该要和SKUConfiguration页面分开，因为明显的 eCom同步提取的信息更多更复杂。。。。2014年5月13日11:14:07
                        //CMS_ShipVia_Default = query.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true) == null ? null : new CMS_ShipVia_Model
                        //{
                        //    SHIPVIA = query.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true).SHIPVIA,
                        //    //SHIPVIAID = query.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true).SHIPVIAID,
                        //    //CarrierCode = query.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true).CarrierCode,
                        //    //CarrierRouting = query.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true).CarrierRouting,
                        //    //ExpressMethod = query.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true).ExpressMethod,
                        //    //ExpressNumLength = query.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true).ExpressNumLength.HasValue ?
                        //    //    query.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true).ExpressNumLength.Value : 0
                        //}
                    },

                    SKU_HM_Relation = new SKU_HM_Relation_Model
                    {
                        R_QTY = query.SKU_HM_Relation.R_QTY,
                        StockKeyID = query.SKU_HM_Relation.StockKeyID,
                        CMS_HMNUM = new CMS_HMNUM_Model
                        {
                            ProductID = query.SKU_HM_Relation.ProductID,
                            HMNUM = query.SKU_HM_Relation.CMS_HMNUM.HMNUM,
                            SellSets = 1,//正常情况下 ，只有组合产品的子产品才有sellsets这个概念，但是eCom非组合产品在界面上也有sellSets这个概念，所以赋予 1 2014年5月14日15:30:17
                            StockKey = query.SKU_HM_Relation.CMS_HMNUM.StockKey, //add by Lee 2014-2-14
                            StockKeyID = query.SKU_HM_Relation.CMS_HMNUM.StockKeyID,//2014年5月5日17:19:37
                            MasterPack = query.SKU_HM_Relation.CMS_HMNUM.MasterPack,
                            NetWeight = query.SKU_HM_Relation.CMS_HMNUM.NetWeight.ConvertToNotNull(),
                            BoxNum = query.SKU_HM_Relation.R_QTY / Convert.ToInt32(query.SKU_HM_Relation.CMS_HMNUM.MasterPack),

                            ProductName = query.SKU_HM_Relation.CMS_HMNUM.ProductName,
                            StockKeyQTY = query.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY.ConvertToNotNull(),
                            Comments = query.SKU_HM_Relation.CMS_HMNUM.Comments,
                            IsGroup = query.SKU_HM_Relation.CMS_HMNUM.IsGroup.HasValue ? query.SKU_HM_Relation.CMS_HMNUM.IsGroup.Value : false,
                            HM_Costing = new CMS_HM_Costing_Model
                            {
                                FirstCost = query.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Costing.FirstCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                                LandedCost = query.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Costing.LandedCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                                EstimateFreight = query.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Costing.EstimateFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                                OceanFreight = query.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Costing.OceanFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                                USAHandlingCharge = query.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Costing.USAHandlingCharge.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                                Drayage = query.SKU_HM_Relation.CMS_HMNUM.CMS_HM_Costing.Drayage.ConvertToNotNull().ToString("C", new CultureInfo("en-US"))
                            },
                            CMS_ShipVia_Type = new CMS_ShipViaType_Model
                            {
                                ShipViaTypeID = query.SKU_HM_Relation.CMS_HMNUM.CMS_ShipViaType == null ? 0 :
                                                  query.SKU_HM_Relation.CMS_HMNUM.CMS_ShipViaType.ShipViaTypeID,
                                ShipViaTypeName = query.SKU_HM_Relation.CMS_HMNUM.CMS_ShipViaType == null ? "" :
                                                  query.SKU_HM_Relation.CMS_HMNUM.CMS_ShipViaType.ShipViaTypeName,//返回空给前端
                                CMS_ShipVia_Default = query.SKU_HM_Relation.CMS_HMNUM.CMS_ShipViaType == null ? null :
                                                (query.SKU_HM_Relation.CMS_HMNUM.CMS_ShipViaType.CMS_ShipVia == null ? null :
                                                    (query.SKU_HM_Relation.CMS_HMNUM.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true) == null ? null :
                                                    new CMS_ShipVia_Model
                                                    {
                                                        SHIPVIA = query.SKU_HM_Relation.CMS_HMNUM.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true).SHIPVIA
                                                    })
                                                )
                            },

                            //再也不要相信这个verCD的假设了，在测试了的时候被提了100遍的BUG，一百遍啊一百遍！！！ 2014年2月19日15:31:48
                            Category = new WebPO_Category_V_Model
                            {
                                CategoryID = query.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V == null ?
                                                0 : query.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V.CategoryID,
                                CategoryName = query.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V == null ?
                                                "NONE" : query.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V.CategoryName,

                                OrderIndex = query.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V == null ?
                                                0 : query.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V.OrderIndex.ConvertToNotNull(),

                                ParentCategoryID = query.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V == null ?
                                                0 : query.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V.ParentCategoryID.ConvertToNotNull(),

                                ParentCategoryName = query.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V == null ?
                                            "NONE" : query.SKU_HM_Relation.CMS_HMNUM.WebPO_Category_V.ParentCategoryName
                            },

                            HMColour = new CMS_HMNUM_Colour_Model
                            {
                                ColourID = query.SKU_HM_Relation.CMS_HMNUM.WebPO_Colour_V == null ? 0 : query.SKU_HM_Relation.CMS_HMNUM.WebPO_Colour_V.ColourID,
                                ColourName = query.SKU_HM_Relation.CMS_HMNUM.WebPO_Colour_V == null ? "NONE" : query.SKU_HM_Relation.CMS_HMNUM.WebPO_Colour_V.ColourDesc
                            },

                            HMMaterial = new CMS_HMNUM_Material_Model
                            {
                                MaterialID = query.SKU_HM_Relation.CMS_HMNUM.WebPO_Material_V == null ? 0 : query.SKU_HM_Relation.CMS_HMNUM.WebPO_Material_V.MaterialID,
                                MaterialName = query.SKU_HM_Relation.CMS_HMNUM.WebPO_Material_V == null ? "NONE" : query.SKU_HM_Relation.CMS_HMNUM.WebPO_Material_V.MaterialName
                            },


                            //在非组合产品下 需要直接取初始化 CTNList 和  DimList，反之在组合产品下，需要获取的是 Children_CMS_HMNUM_List 下的这2个参数，直接获取这2个无意义。
                            //需要做空判断，因为...有可能。。有产品没相纸 2013年11月21日17:39:51
                            CTNList = query.SKU_HM_Relation == null ? null : query.SKU_HM_Relation.CMS_HMNUM.CMS_ProductCTN.Select(h => new CMS_ProductCTN_Model
                            {
                                CTNID = h.CTNID,
                                CTNTitle = h.CTNTitle,
                                CTNHeight = h.CTNHeight.ConvertToNotNull(),//JUST Testing 2013年11月11日11:36:48
                                CTNLength = h.CTNLength.HasValue ? h.CTNLength.Value : 0,
                                CTNWidth = h.CTNWidth.HasValue ? h.CTNWidth.Value : 0,
                                CTNCube = h.CTNCube.HasValue ? h.CTNCube.Value : 0,
                                CTNWeight = h.CTNWeight.HasValue ? h.CTNWeight.Value : 0//2013年12月25日16:50:02

                            }).ToList(),
                            DimList = query.SKU_HM_Relation == null ? null : query.SKU_HM_Relation.CMS_HMNUM.CMS_ProductDimension.Select(d => new CMS_ProductDimension_Model
                            {
                                DimID = d.DimID,
                                DimTitle = d.DimTitle,
                                DimHeight = d.DimHeight.HasValue ? d.DimHeight.Value : 0,
                                DimLength = d.DimLength.HasValue ? d.DimLength.Value : 0,
                                DimWidth = d.DimWidth.HasValue ? d.DimWidth.Value : 0,
                                DimCube = d.DimCube.HasValue ? d.DimCube.Value : 0
                            }).ToList(),

                            //Add 2014年3月22日
                            webSystemImage = new HMCommonServices().GetImagesFromOtherSystem(query.SKU_HM_Relation.CMS_HMNUM.HMNUM).FirstOrDefault(),

                            Children_CMS_HMNUM_List = query.SKU_HM_Relation.CMS_HMNUM.CMS_HMGroup_Relation.Select(cr => new CMS_HMNUM_Model
                            {
                                SellSets = cr.SellSets,//...
                                ProductID = cr.CMS_HMNUM_Children.ProductID,
                                HMNUM = cr.CMS_HMNUM_Children.HMNUM,
                                StockKey = cr.CMS_HMNUM_Children.StockKey,
                                MasterPack = cr.CMS_HMNUM_Children.MasterPack,
                                BoxNum = cr.SellSets / Convert.ToInt32(cr.CMS_HMNUM_Children.MasterPack),
                                ProductName = cr.CMS_HMNUM_Children.ProductName,
                                StockKeyQTY = cr.CMS_HMNUM_Children.CMS_HM_Inventory.StockkeyQTY.ConvertToNotNull(),
                                NetWeight = cr.CMS_HMNUM_Children.NetWeight.ConvertToNotNull(),
                                CMS_ShipVia_Type = new CMS_ShipViaType_Model
                                {
                                    ShipViaTypeID = cr.CMS_HMNUM_Children.CMS_ShipViaType == null ? 0 :
                                                      cr.CMS_HMNUM_Children.CMS_ShipViaType.ShipViaTypeID,
                                    ShipViaTypeName = cr.CMS_HMNUM_Children.CMS_ShipViaType == null ? "" :
                                                      cr.CMS_HMNUM_Children.CMS_ShipViaType.ShipViaTypeName,//返回空给前端

                                    CMS_ShipVia_Default = cr.CMS_HMNUM_Children.CMS_ShipViaType == null ? null :
                                      (cr.CMS_HMNUM_Children.CMS_ShipViaType.CMS_ShipVia == null ? null :
                                          (cr.CMS_HMNUM_Children.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true) == null ? null :
                                          new CMS_ShipVia_Model
                                          {
                                              SHIPVIA = cr.CMS_HMNUM_Children.CMS_ShipViaType.CMS_ShipVia.FirstOrDefault(r => r.IsDefaultShipVia == true).SHIPVIA
                                          })
                                      )
                                },

                                HM_Costing = new CMS_HM_Costing_Model//eCom同步之用 2014年5月6日14:44:07
                                {
                                    FirstCost = cr.CMS_HMNUM_Children.CMS_HM_Costing.FirstCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                                    LandedCost = cr.CMS_HMNUM_Children.CMS_HM_Costing.LandedCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                                    EstimateFreight = cr.CMS_HMNUM_Children.CMS_HM_Costing.EstimateFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                                    OceanFreight = cr.CMS_HMNUM_Children.CMS_HM_Costing.OceanFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                                    USAHandlingCharge = cr.CMS_HMNUM_Children.CMS_HM_Costing.USAHandlingCharge.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                                    Drayage = cr.CMS_HMNUM_Children.CMS_HM_Costing.Drayage.ConvertToNotNull().ToString("C", new CultureInfo("en-US"))
                                },
                                CTNList = cr.CMS_HMNUM_Children.CMS_ProductCTN.Select(cc => new CMS_ProductCTN_Model
                                {
                                    CTNID = cc.CTNID,
                                    CTNTitle = cc.CTNTitle,
                                    CTNHeight = cc.CTNHeight.ConvertToNotNull(),//JUST Testing 2013年11月11日11:36:48
                                    CTNLength = cc.CTNLength.HasValue ? cc.CTNLength.Value : 0,
                                    CTNWidth = cc.CTNWidth.HasValue ? cc.CTNWidth.Value : 0,
                                    CTNCube = cc.CTNCube.HasValue ? cc.CTNCube.Value : 0,
                                    CTNWeight = cc.CTNWeight.HasValue ? cc.CTNWeight.Value : 0//2013年12月25日16:50:02
                                }).ToList(),
                                DimList = cr.CMS_HMNUM_Children.CMS_ProductDimension.Select(cd => new CMS_ProductDimension_Model
                                {
                                    DimID = cd.DimID,
                                    DimTitle = cd.DimTitle,
                                    DimHeight = cd.DimHeight.HasValue ? cd.DimHeight.Value : 0,
                                    DimLength = cd.DimLength.HasValue ? cd.DimLength.Value : 0,
                                    DimWidth = cd.DimWidth.HasValue ? cd.DimWidth.Value : 0,
                                    DimCube = cd.DimCube.HasValue ? cd.DimCube.Value : 0
                                }).ToList(),

                                //Add 2014年3月22日
                                webSystemImage = new HMCommonServices().GetImagesFromOtherSystem(cr.CMS_HMNUM_Children.HMNUM).FirstOrDefault(),

                                // Add 2014年4月22日 与eCom同步用
                                MediaList = cr.CMS_HMNUM_Children.CMS_StockKey.MediaLibrary.Select(r => new MediaLibrary_Model
                                {
                                    fileFormat = r.fileFormat,
                                    fileHeight = r.fileHeight.HasValue ? r.fileHeight.Value : 0,
                                    fileWidth = r.fileWidth.HasValue ? r.fileWidth.Value : 0,
                                    fileSize = r.fileSize,
                                    MediaID = r.MediaID,
                                    ImgName = r.ImgName,
                                    HMNUM = r.HMNUM,
                                    MediaType = r.MediaType,
                                    IsPrimaryImages = r.PrimaryImage.HasValue ? r.PrimaryImage.Value : false
                                }).ToList()


                            }).ToList()
                        }
                    },


                    channelMedias = query.SKU_Media_Relation.Select(r => new MediaLibrary_Model
                    {
                        fileFormat = r.MediaLibrary.fileFormat,
                        fileHeight = r.MediaLibrary.fileHeight.HasValue ? r.MediaLibrary.fileHeight.Value : 0,
                        fileWidth = r.MediaLibrary.fileWidth.HasValue ? r.MediaLibrary.fileWidth.Value : 0,
                        fileSize = r.MediaLibrary.fileSize,
                        MediaID = r.MediaLibrary.MediaID,
                        ImgName = r.MediaLibrary.ImgName,
                        HMNUM = r.MediaLibrary.HMNUM,
                        MediaType = r.MediaLibrary.MediaType,
                        IsPrimaryImages = r.PrimaryImage,
                        ProductID = r.MediaLibrary.ProductID,
                        CloudStatusID = r.MediaLibrary.CloudStatusID,
                        strCreateOn = r.MediaLibrary.CreateOn.ToString("yyyy-MM-dd HH:mm:ss"),
                        CMS_SKU = r.MediaLibrary.SKU_Media_Relation.Where(k => k.MediaID == r.MediaID).Select(s => new CMS_SKU_Model
                        {
                            ChannelName = s.CMS_SKU.Channel.ChannelName,
                            ProductName = s.CMS_SKU.ProductName,
                            SKU = s.CMS_SKU.SKU
                        }).ToList()
                    }).ToList()
                };

            }
        }
        /// <summary>
        /// 根据ProductID获取单个HM对象
        /// Author:Lee; Date:2013年11月12日11:11:42
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="relationQty"></param>
        /// <param name="productWebsiteRelationId"></param>
        /// <returns></returns>
        public CMS_HMNUM_Model GetSingleHMByID(long productId, int relationQty, long productWebsiteRelationId)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_HMNUM.Include(p => p.CMS_ProductCTN).Include(p => p.CMS_ProductDimension).FirstOrDefault(p => p.ProductID == productId);
                if (query == null)
                {
                    return null;
                }
                var HMInfo = new CMS_HMNUM_Model
                {
                    ProductID = query.ProductID,
                    Comments = query.Comments,
                    ProductName = query.ProductName,
                    StockKeyQTY = query.CMS_HM_Inventory == null ? 0 : query.CMS_HM_Inventory.StockkeyQTY.ConvertToNotNull(),
                    HMNUM = query.HMNUM,

                    HM_Costing = new CMS_HM_Costing_Model
                    {
                        FirstCost = query.CMS_HM_Costing.FirstCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                        LandedCost = query.CMS_HM_Costing.LandedCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                        EstimateFreight = query.CMS_HM_Costing.EstimateFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                        OceanFreight = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.OceanFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                        USAHandlingCharge = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.USAHandlingCharge.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                        Drayage = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.Drayage.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                        SellSets = relationQty,

                    },
                    CTNList = query.CMS_ProductCTN.Select(h => new CMS_ProductCTN_Model
                    {
                        CTNID = h.CTNID,
                        CTNTitle = h.CTNTitle,
                        CTNHeight = h.CTNHeight.ConvertToNotNull(),//JUST Testing 2013年11月11日11:36:48
                        CTNLength = h.CTNLength.HasValue ? h.CTNLength.Value : 0,
                        CTNWidth = h.CTNWidth.HasValue ? h.CTNWidth.Value : 0,
                        CTNCube = h.CTNCube.HasValue ? h.CTNCube.Value : 0,
                        CTNWeight = h.CTNWeight.HasValue ? h.CTNWeight.Value : 0 //2013年12月26日9:52:24
                    }).ToList(),
                    DimList = query.CMS_ProductDimension.Select(d => new CMS_ProductDimension_Model
                    {
                        DimID = d.DimID,
                        DimTitle = d.DimTitle,
                        DimHeight = d.DimHeight.HasValue ? d.DimHeight.Value : 0,
                        DimLength = d.DimLength.HasValue ? d.DimLength.Value : 0,
                        DimWidth = d.DimWidth.HasValue ? d.DimWidth.Value : 0,
                        DimCube = d.DimCube.HasValue ? d.DimCube.Value : 0
                    }).ToList(),

                    Children_CMS_HMNUM_List = query.CMS_HMGroup_Relation.Select(cr => new CMS_HMNUM_Model
                    {
                        ProductID = cr.CMS_HMNUM_Children.ProductID,
                        HMNUM = cr.CMS_HMNUM_Children.HMNUM,
                        ProductName = cr.CMS_HMNUM_Children.ProductName,
                        StockKeyQTY = cr.CMS_HMNUM_Children.CMS_HM_Inventory.StockkeyQTY.ConvertToNotNull(),
                        SellSets = cr.SellSets,//...
                        CTNList = cr.CMS_HMNUM_Children.CMS_ProductCTN.Select(cc => new CMS_ProductCTN_Model
                        {
                            CTNID = cc.CTNID,
                            CTNTitle = cc.CTNTitle,
                            CTNHeight = cc.CTNHeight.ConvertToNotNull(),//JUST Testing 2013年11月11日11:36:48
                            CTNLength = cc.CTNLength.HasValue ? cc.CTNLength.Value : 0,
                            CTNWidth = cc.CTNWidth.HasValue ? cc.CTNWidth.Value : 0,
                            CTNCube = cc.CTNCube.HasValue ? cc.CTNCube.Value : 0,
                            CTNWeight = cc.CTNWeight.HasValue ? cc.CTNWeight.Value : 0 //2013年12月26日9:52:37
                        }).ToList(),
                        DimList = cr.CMS_HMNUM_Children.CMS_ProductDimension.Select(cd => new CMS_ProductDimension_Model
                        {
                            DimID = cd.DimID,
                            DimTitle = cd.DimTitle,
                            DimHeight = cd.DimHeight.HasValue ? cd.DimHeight.Value : 0,
                            DimLength = cd.DimLength.HasValue ? cd.DimLength.Value : 0,
                            DimWidth = cd.DimWidth.HasValue ? cd.DimWidth.Value : 0,
                            DimCube = cd.DimCube.HasValue ? cd.DimCube.Value : 0
                        }).ToList()
                    }).ToList()
                };

                return HMInfo;
            }
        }

        /// <summary>
        /// 根据SKUOrder从 WebsitProducts_Model表获取出相关的所有产品。
        /// 供给ProducConfiguration页面的 RelatedProduct 部分使用
        /// Change:新增userInfo参数，用于渠道显示的控制 2014年1月6日13:14:26
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public List<CMS_SKU_Model> GetRelatedWebsitProducts(long SKUID, User_Profile_Model userInfo)
        {
            var list = new List<CMS_SKU_Model>();
            using (var db = new PermaisuriCMSEntities())
            {
                //var ProductIDs = db.SKU_HM_Relation.Where(r => r.SKUID == SKUID).Select(r => r.ProductID).ToArray();

                //var query = db.CMS_SKU.Include("Channel").Include("Brand").Where(wp => wp.SKU_HM_Relation
                //    .Any(r => ProductIDs.Contains(r.ProductID)) && wp.SKUID != SKUID);

                var query = db.CMS_SKU.Where(r => r.SKU_HM_Relation.CMS_HMNUM.SKU_HM_Relation.Any(rr => rr.SKUID == SKUID));

                if (userInfo.IsChannelControl == true)//渠道控制 2014年1月6日13:16:40
                {
                    var mChannels = db.Channel.Where(q => q.User_Channel_Relation.Any(r => r.User_Guid == userInfo.User_Guid)).Select(q => q.ChannelID).ToArray();
                    query = query.Where(q => mChannels.Contains(q.ChannelID));
                }

                list.AddRange(query.Select(p => new CMS_SKU_Model
                {
                    SKUID = p.SKUID, SKU = p.SKU, ChannelID = p.ChannelID, ChannelName = p.Channel == null ? "NONE" : p.Channel.ChannelName, ProductName = p.ProductName, BrandID = p.Brand == null ? 0 : p.Brand.BrandID, BrandName = p.Brand == null ? "" : p.Brand.BrandName, URL = p.URL
                }));
                return list;
            }
        }



        /// <summary>
        ///  获取Product表的 HMNUM 和 Descripton字段信息，用于Product Configuration的 Product Pieces 下拉单动态展示。
        ///  当用户选中下来单中的产品之后，在触发查询该产品的尺寸等信息...这样可以大幅减少Server--Browser之间的Http连接。
        /// </summary>
        /// <returns></returns>
        public List<CMS_HMNUM_Model> GetAllProducts()
        {
            var list = new List<CMS_HMNUM_Model>();
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_HMNUM.AsEnumerable();
                list.AddRange(query.Select(p => new CMS_HMNUM_Model
                {
                    HMNUM = p.HMNUM, ProductName = p.ProductName
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
            return new ProductCommonServices().GetAllBrands();
        }


        ///// <summary>
        ///// 获取所有渠道列表
        ///// </summary>
        ///// <returns></returns>
        //public List<Channel_Model> GetAllChannels(bool isControl, Guid User_Guid)
        //{
        //    return new ProductCommonServices().GetAllChannels(isControl, User_Guid);
        //}

        /// <summary>
        /// Change1: MCCC 你懂得 2014年1月28日16:27:30
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mcModel"></param>
        /// <param name="Modifier"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdatedProduct(CMS_SKU_Model model, MCCC mcModel, String Modifier, out string msg)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                msg = "";
                var query = db.CMS_SKU.Where(w => w.SKUID == model.SKUID).FirstOrDefault();
                if (query == null)
                {
                    msg = string.Format("Can not find id {0} record in DB!", model.SKUID);
                    return false;
                }

                var mat = db.CMS_SKU_Material.FirstOrDefault(s => s.MaterialName == mcModel.Material);
                if (mat == null)
                {
                    mat = new CMS_SKU_Material
                     {
                         CreateBy = Modifier,
                         CreateOn = DateTime.Now,
                         ModifyBy = Modifier,
                         ModifyOn = DateTime.Now,
                         MaterialDesc = mcModel.Material,
                         MaterialName = mcModel.Material
                     };
                    db.CMS_SKU_Material.Add(mat);
                }



                var col = db.CMS_SKU_Colour.FirstOrDefault(s => s.ColourName == mcModel.Colour);
                if (col == null)
                {
                    col = new CMS_SKU_Colour
                    {
                        CreateBy = Modifier,
                        CreateOn = DateTime.Now,
                        ModifyBy = Modifier,
                        ModifyOn = DateTime.Now,
                        ColourDesc = mcModel.Colour,
                        ColourName = mcModel.Colour
                    };
                    db.CMS_SKU_Colour.Add(col);
                }


                var cat = db.CMS_SKU_Category.FirstOrDefault(s => s.CategoryName == mcModel.Category);
                if (cat == null)
                {
                    cat = new CMS_SKU_Category
                    {
                        CreateBy = Modifier,
                        CreateOn = DateTime.Now,
                        UpdateBy = Modifier,
                        UpdateOn = DateTime.Now,
                        CategoryName = mcModel.Category,
                        CategoryDesc = mcModel.Category,
                        OrderIndex = 0,
                        ParentID = 0
                    };
                    db.CMS_SKU_Category.Add(cat);
                }

                db.SaveChanges();//这个必须存在，否则下面那个SubCategory就报错....

                var subCat = db.CMS_SKU_Category.FirstOrDefault(s => s.CategoryName == mcModel.SubCategory);
                if (subCat == null)
                {
                    subCat = new CMS_SKU_Category
                    {
                        CreateBy = Modifier,
                        CreateOn = DateTime.Now,
                        UpdateBy = Modifier,
                        UpdateOn = DateTime.Now,
                        CategoryName = mcModel.SubCategory,
                        CategoryDesc = mcModel.SubCategory,
                        OrderIndex = 0,
                        ParentID = cat.CategoryID
                    };
                    db.CMS_SKU_Category.Add(subCat);
                    /*2014年3月10日：如果这里不加上SaveChanges()则，后面Operating Details Log的new Value的ID一直会是0，非常的坑爹*/
                    db.SaveChanges();
                }
                query.MaterialID = mat.MaterialID;
                query.ColourID = col.ColourID;
                query.CMS_SKU_Category = cat;
                query.CMS_SKU_Category_Sub = subCat;

                query.Keywords = model.Keywords;
                query.RetailPrice = model.RetailPrice;
                query.ProductDesc = model.ProductDesc;
                query.SKU_QTY = model.SKU_QTY;
                query.Specifications = model.Specifications;
                query.StatusID = model.StatusID;
                query.UPC = model.UPC;
                query.URL = model.URL;
                query.Visibility = model.Visibility;
                query.BrandID = model.BrandID;
                query.ShipViaTypeID = model.ShipViaTypeID;

                if (query.CMS_Ecom_Sync == null)
                {

                    query.CMS_Ecom_Sync = new CMS_Ecom_Sync
                   {
                       SKUID = query.SKUID,
                       StatusID = 0,
                       StatusDesc = "NeedSend",
                       UpdateBy = Modifier,
                       UpdateOn = DateTime.Now
                   };
                }
                else
                {
                    query.CMS_Ecom_Sync.StatusID = 0;
                    query.CMS_Ecom_Sync.StatusDesc = "NeedSend";
                    query.CMS_Ecom_Sync.UpdateBy = Modifier;
                    query.CMS_Ecom_Sync.UpdateOn = DateTime.Now;
                }


                /*
                 *  {"违反了 PRIMARY KEY 约束 'PK_CMS_Ecom_Sync'。不能在对象 'dbo.CMS_Ecom_Sync' 中插入重复键。\r\n语句已终止。"}	 
                 * System.Exception {System.Data.SqlClient.SqlException}
                 */
                //query.CMS_Ecom_Sync = newSyncModel;
                /*不能放在SaveChange()之后执行，原因是因为  where entry.Property(name).IsModified 在SaveChange()之后成为了0...*/
                var entry = db.Entry(query);
                BllExtention.DbRecorder(entry, new LogOfUserOperatingDetails
                {
                    ModelName = ModelName,
                    ActionName = MethodBase.GetCurrentMethod().Name,
                    ActionType = LogActionTypeEnum.Update.GetHashCode(),


                    SKUID = query.SKUID,
                    SKU = query.SKU,
                    ChannelID = query.ChannelID,
                    ChannelName = query.Channel.ChannelName,
                    ProductID = query.SKU_HM_Relation.ProductID,
                    HMNUM = query.SKU_HM_Relation.CMS_HMNUM.HMNUM,

                    CreateBy = Modifier,
                    CreateOn = DateTime.Now
                });


                query.UpdateOn = DateTime.Now;
                query.UpdateBy = Modifier;


                //query.ColourID = model.ColourID; 尼玛的 血的教训啊 啊啊 2014年1月28日17:16:35
                //query.MaterialID = model.MaterialID;
                //query.CategoryID = model.CategoryID;
                //query.SubCategoryID = model.SubCategoryID;


                int retVal = db.SaveChanges();

                return retVal > 0;
            }
        }



        /// <summary>
        /// 产品复制，该方法支持前端多选Channel一次性插入..........
        /// CreateDate:2013年12月6日10:52:42
        /// </summary>
        /// <param name="SKUID"></param>
        /// <param name="newSKUOrder"></param>
        /// <param name="newBrandID"></param>
        /// <param name="ChannelList"></param>
        /// <param name="user_Account"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool DuplicateMultipleNewSKU(long SKUID, string newSKUOrder, int newBrandID, List<int> ChannelList, string user_Account, out string msg)
        {
            msg = string.Empty;
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                //step 1: 根据 SKUID查询出该产品信息
                var oldProduct = db.CMS_SKU.Where(p => p.SKUID == SKUID).FirstOrDefault();
                if (oldProduct == null)
                {
                    msg = string.Format("can not find ID={0}'s product", SKUID);
                    return false;
                }

                StringBuilder warningMsg = new StringBuilder();
                foreach (int newChannelID in ChannelList)
                {
                    //step 2:判断要新增的产品是否已经存在原有的表里面（数据重复）
                    var dupProudct = db.CMS_SKU.Where(p => p.SKU == newSKUOrder && p.ChannelID == newChannelID).FirstOrDefault();
                    if (dupProudct != null)
                    {
                        //msg = string.Format ("this item is already exists");
                        warningMsg.AppendFormat(" Item duplicate failed: SKU={0},Channel={1}  is already exists",
                            dupProudct.SKU, dupProudct.Channel == null ? "NONE" : dupProudct.Channel.ChannelName);
                    }
                    else
                    {
                        //step 3 复制产品

                        var newCosting = new CMS_SKU_Costing
                        {
                            SalePrice = oldProduct.CMS_SKU_Costing.SalePrice,
                            EffectiveDate = DateTime.Now,
                            CreateBy = user_Account,
                            CreateOn = DateTime.Now
                        };

                        var curChannel = db.Channel.FirstOrDefault(c => c.ChannelID == newChannelID);
                        CMS_SKU newProduct = new CMS_SKU
                        {
                            BrandID = newBrandID,
                            Channel = curChannel,
                            SKU = newSKUOrder,
                            Keywords = oldProduct.Keywords,
                            RetailPrice = oldProduct.RetailPrice,
                            ProductDesc = oldProduct.ProductDesc,
                            ProductName = oldProduct.ProductName,
                            SKU_QTY = oldProduct.SKU_QTY,
                            Specifications = oldProduct.Specifications,
                            StatusID = 7,
                            UPC = oldProduct.UPC,
                            URL = oldProduct.URL,
                            Visibility = oldProduct.Visibility,
                            CMS_SKU_Costing = newCosting,
                            CMS_Ecom_Sync = new CMS_Ecom_Sync
                               {
                                   StatusID = 0,
                                   StatusDesc = "NeedSend",
                                   UpdateBy = user_Account,
                                   UpdateOn = DateTime.Now
                               },

                            ColourID = oldProduct.ColourID,
                            MaterialID = oldProduct.MaterialID,
                            CategoryID = oldProduct.CategoryID,
                            SubCategoryID = oldProduct.SubCategoryID,

                            ShipViaTypeID = oldProduct.ShipViaTypeID,

                            CreateBy = user_Account,
                            CreateOn = DateTime.Now,
                            UpdateBy = user_Account,
                            UpdateOn = DateTime.Now
                        };
                        db.CMS_SKU.Add(newProduct);
                        db.SaveChanges();
                        newCosting.HisSKUID = newProduct.SKUID;//经测试：如果让这个生效，一定要在此之前先发生一次db.SaveChanges()

                        //step 4 复制Product Pieces
                        var newSKUID = newProduct.SKUID;
                        db.SKU_HM_Relation.Add(new SKU_HM_Relation
                        {
                            SKUID = newSKUID,
                            ProductID = oldProduct.SKU_HM_Relation.ProductID,
                            StockKeyID = oldProduct.SKU_HM_Relation.StockKeyID,
                            R_QTY = oldProduct.SKU_HM_Relation.R_QTY,
                            CreateOn = DateTime.Now,
                            CreateBy = user_Account,
                            UpdateBy = user_Account,
                            UpdateOn = DateTime.Now
                        });
                        //step 5 复制Images
                        foreach (SKU_Media_Relation imgR in oldProduct.SKU_Media_Relation)
                        {
                            db.SKU_Media_Relation.Add(new SKU_Media_Relation
                            {
                                MediaID = imgR.MediaID,
                                SKUID = newProduct.SKUID,
                                PrimaryImage = imgR.PrimaryImage,

                            });
                        }

                        //step 6 开始插入
                        db.SaveChanges();


                        //new Step 7 Log
                        BllExtention.DbRecorder(new LogOfUserOperatingDetails
                        {
                            ModelName = ModelName,
                            ActionName = MethodBase.GetCurrentMethod().Name,
                            ActionType = LogActionTypeEnum.Inert.GetHashCode(),

                            SKUID = newProduct.SKUID,
                            SKU = newProduct.SKU,
                            ChannelID = newChannelID,
                            ChannelName = curChannel.ChannelName,// newProduct.Channel==null!
                            ProductID = oldProduct.SKU_HM_Relation.ProductID,
                            HMNUM = oldProduct.SKU_HM_Relation.CMS_HMNUM.HMNUM,
                            Descriptions = "Duplicate New Prouct",
                            CreateBy = user_Account,
                            CreateOn = DateTime.Now
                        });
                    }
                }
                msg = warningMsg.ToString();
                return true;
            }
        }

        /// <summary>
        /// 把当前渠道的产品和图像信息关联起来
        /// </summary>
        /// <param name="mediaIDList">图像列表ID</param>
        /// <param name="SKUID">产品ID</param>
        /// <returns></returns>
        public bool AttachImagesToSKU(List<long> mediaIDList, long SKUID)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                //由于在图像选择的时候已经对已选图像做了过滤，所以这里不用再做delete...
                //using (TransactionScope transaction = new TransactionScope())
                //{
                // db.Database.ExecuteSqlCommand("delete from SKU_Media_Relation where SKUID = @SKUID", new SqlParameter("@SKUID", SKUID));

                //先查询当前的SKUID对于的图像是否设置了PrimaryImages,如果没有设置，则默认设置第一张图片为PrimaryImages
                var relImages = db.SKU_Media_Relation.FirstOrDefault(r => r.SKUID == SKUID && r.PrimaryImage == true);

                int i = 0;
                foreach (long mediaID in mediaIDList)
                {
                    if (relImages == null && i == 0)
                    {
                        db.SKU_Media_Relation.Add(new SKU_Media_Relation
                        {
                            MediaID = mediaID,
                            SKUID = SKUID,
                            PrimaryImage = true
                        });
                    }
                    else
                    {
                        db.SKU_Media_Relation.Add(new SKU_Media_Relation
                        {
                            MediaID = mediaID,
                            SKUID = SKUID,
                            PrimaryImage = false
                        });
                    }
                    i++;
                }
                int retInt = db.SaveChanges();
                // transaction.Complete();// don't be miss,or the SQL statement will never be executed
                return retInt > 0;
                //}
            }
        }

        /// <summary>
        /// 2014年5月31日10:30:40
        /// </summary>
        /// <param name="SKUID"></param>
        /// <param name="OtherSKUID"></param>
        public void CopyImagesFromOtherSKUID(long SKUID, long OtherSKUID, User_Profile_Model userInfo)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var OtherSKUIDList = db.SKU_Media_Relation.Where(s => s.SKUID == OtherSKUID).Select(s => s.MediaID).Distinct();
                 List<long> tempList = new List<long>();
                tempList.Add(SKUID);
                this.AttachImagesToSKUWithBatch(OtherSKUIDList.ToList(), tempList, userInfo);
            }
        }

        /// <summary>
        /// 多图像关联多渠道的SKUOrder.规则：如果原先的SKU已经关联了当中的某些图像，则保留，新建那些还未关联的图像。所以这里只能用双循环一个个判断。
        /// CreatedDate:2014年2月14日11:02:45
        /// </summary>
        /// <param name="mediaIDList"></param>
        /// <param name="SKUIDList"></param>
        /// <returns>返回主显图像</returns>
        public void AttachImagesToSKUWithBatch(List<long> mediaIDList, List<long> SKUIDList, User_Profile_Model userInfo)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                foreach (long SKUID in SKUIDList)
                {
                    bool IsExistPM = false;//是否已经存在主显图像？
                    var temObj = db.SKU_Media_Relation.FirstOrDefault(r => r.SKUID == SKUID && r.PrimaryImage == true);
                    if (temObj != null)
                    {
                        IsExistPM = true;
                    }

                    foreach (long mediaID in mediaIDList)
                    {
                        //先判断是否存在，存在这忽略不计。
                        var query = db.SKU_Media_Relation.FirstOrDefault(r => r.SKUID == SKUID && r.MediaID == mediaID);

                        if (query == null)//如果存在这个记录，则插入，否则不插入
                        {
                            if (IsExistPM)//如果存在主显图像，则设置其他的图像的主显属性为false
                            {
                                db.SKU_Media_Relation.Add(
                                  new SKU_Media_Relation
                                  {
                                      MediaID = mediaID,
                                      SKUID = SKUID,
                                      PrimaryImage = false
                                  });
                            }
                            else //如果【不】存在主显图像，则设置当前图像为的主显图像为true，【同时】设置IsExistPM 为true通知内层循环。
                            {
                                db.SKU_Media_Relation.Add(
                                  new SKU_Media_Relation
                                  {
                                      MediaID = mediaID,
                                      SKUID = SKUID,
                                      PrimaryImage = true
                                  });

                                IsExistPM = true;
                            }

                        }
                    } //end of    foreach (long mediaID in SKUIDList) 内层：媒体层循环

                } // end of foreach (long SKUID in SKUIDList) 外层：SKU层的循环。
                /*这句话必须放在remove之前执行，否则 query.CMS_SKU == null ....2014年3月11日17:08:37*/
 
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 这里需要考虑到StockKey的情况....
        /// Change1:增加对Duplicated Channel的过滤。2014年2月24日
        /// Change2:重复的Channel背后指向的是不同的SKU，真正和图像产生关联的不是Channel，而是SKU！所以不能消除 2014年2月24日19:05:44
        /// </summary>
        /// <param name="HMNUM"></param>
        /// <returns></returns>
        public List<Channel_Model> GetChannelsByHM(string Stockkey)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                /*
                 select distinct(C.ChannelName) from CMS_SKU WP
                 * Inner join Channel C on C.ChannelID =WP.ChannelID
                 * where Exists (select null from  SKU_HM_Relation R where R.SKUID = WP.SKUID and R.ProductID = 87)
                 */

                var query = db.SKU_HM_Relation.Where(r => r.CMS_HMNUM.StockKey == Stockkey).Select(
                        K => new Channel_Model
                        {
                            SKUID = K.CMS_SKU.SKUID,
                            ChannelID = K.CMS_SKU.Channel.ChannelID,
                            ChannelName = K.CMS_SKU.Channel.ChannelName + "-" + K.CMS_SKU.SKU
                        }
                    );


                return query.ToList();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="costing">输入输出参数，输入时候代表客户端需要更新的价格传递给服务器，输出代表价格跟新后在数据库的实际存储方式。比如，客户端输入10个小数点的数字后....</param>
        /// <param name="User_Account"></param>
        /// <returns></returns>
        public bool EditSKUCosting(long SKUID, ref CMS_SKU_Costing_Model costing, string User_Account, Nullable<decimal> RetailPrice)
        {
            //逻辑：先讲当前最新的价格插入到Costing表（注意是新增不是编辑），然后更新当前HMNUM的Costing信息，取最新的那条。
            //EF本身自带有Transaction功能。
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                int retVal = 0;
                var newCosting = new CMS_SKU_Costing
                {
                    SalePrice = Convert.ToDecimal(costing.SalePrice),
                    EstimateFreight = Convert.ToDecimal(costing.EstimateFreight),
                    EffectiveDate = DateTime.Now,
                    CreateBy = User_Account,
                    CreateOn = DateTime.Now
                };

                //db.CMS_SKU_Costing.Add(newCosting);
                //long newCostID = newCosting.SKUCostID;//经测试：如果让这个生效，一定要在此之前先发生一次db.SaveChanges()

                var SKU = db.CMS_SKU.FirstOrDefault(h => h.SKUID == SKUID);
                var oldCost = SKU.CMS_SKU_Costing.SalePrice;
                SKU.CMS_SKU_Costing = newCosting;
                SKU.UpdateBy = User_Account;
                SKU.UpdateOn = DateTime.Now;
                if (RetailPrice != null)
                {
                    SKU.RetailPrice = RetailPrice.Value;
                }

                if (SKU.CMS_Ecom_Sync == null)
                {

                    SKU.CMS_Ecom_Sync = new CMS_Ecom_Sync
                    {
                        SKUID = SKU.SKUID,
                        StatusID = 0,
                        StatusDesc = "NeedSend",
                        UpdateBy = User_Account,
                        UpdateOn = DateTime.Now
                    };
                }
                else
                {
                    SKU.CMS_Ecom_Sync.StatusID = 0;
                    SKU.CMS_Ecom_Sync.StatusDesc = "NeedSend";
                    SKU.CMS_Ecom_Sync.UpdateBy = User_Account;
                    SKU.CMS_Ecom_Sync.UpdateOn = DateTime.Now;
                }


                string logDesc = string.Empty;
                logDesc += " <b> [ Sale Prices:] </b> : old value =  <span style='color:red'>  " + oldCost + " </span> , new Value = <span style='color:red'>  " + newCosting.SalePrice + "  </span>";
                logDesc += " <b> [ EstimateFreight:] </b> : old value =  <span style='color:red'>  " + oldCost + " </span> , new Value = <span style='color:red'>  " + newCosting.EstimateFreight + "  </span>";
                logDesc += "<br>";

                BllExtention.DbRecorder(new LogOfUserOperatingDetails
                {
                    ModelName = ModelName,
                    ActionName = MethodBase.GetCurrentMethod().Name,
                    ActionType = LogActionTypeEnum.Update.GetHashCode(),

                    SKUID = SKU.SKUID,
                    SKU = SKU.SKU,
                    ChannelID = SKU.ChannelID,
                    ChannelName = SKU.Channel.ChannelName,
                    ProductID = SKU.SKU_HM_Relation.ProductID,
                    HMNUM = SKU.SKU_HM_Relation.CMS_HMNUM.HMNUM,

                    Descriptions = logDesc,
                    CreateBy = User_Account,
                    CreateOn = DateTime.Now
                });

                retVal = db.SaveChanges();

                newCosting.HisSKUID = SKU.SKUID;//经测试：如果让这个生效，一定要在此之前先发生一次db.SaveChanges()
                costing.SalePrice = newCosting.SalePrice.ToString("C", new CultureInfo("en-US"));
                costing.EstimateFreight = newCosting.EstimateFreight.ToString("C", new CultureInfo("en-US"));

                db.SaveChanges();
                return retVal > 0;
            }
        }

        /// <summary>
        /// 删除掉网站产品和其中一张图像的关系
        /// </summary>
        /// <param name="SKUID"></param>
        /// <param name="MediaID"></param>
        /// <returns></returns>
        public bool RemoveSKUMedia(long SKUID, long MediaID, User_Profile_Model user)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.SKU_Media_Relation.Where(r => r.SKUID == SKUID && r.MediaID == MediaID).FirstOrDefault();
                //db.Set<SKU_Media_Relation>().Attach(query);

                /*这句话必须放在remove之前执行，否则 query.CMS_SKU == null ....2014年3月11日17:08:37*/
                BllExtention.DbRecorder(new LogOfUserOperatingDetails
                {
                    ModelName = ModelName,
                    ActionName = MethodBase.GetCurrentMethod().Name,
                    ActionType = LogActionTypeEnum.Delete.GetHashCode(),

                    SKUID = query.CMS_SKU.SKUID,
                    SKU = query.CMS_SKU.SKU,
                    ChannelID = query.CMS_SKU.ChannelID,
                    ChannelName = query.CMS_SKU.Channel.ChannelName,// newProduct.Channel==null!
                    ProductID = query.MediaLibrary.ProductID,
                    HMNUM = query.MediaLibrary.HMNUM,

                    Descriptions = "Remove SKU\'s iamge, iamge name = [<b>" + query.MediaLibrary.ImgName + "</b>]",
                    CreateBy = user.User_Account,
                    CreateOn = DateTime.Now
                });

                if (query.PrimaryImage)//2014年3月24日
                {
                    var relation = db.SKU_Media_Relation.FirstOrDefault(r => r.SKUID == SKUID && r.MediaID != MediaID);
                    if (relation != null)
                    {
                        relation.PrimaryImage = true;
                    }
                }
                db.SKU_Media_Relation.Remove(query);


                return db.SaveChanges() > 0;
            }
        }

    }
}
