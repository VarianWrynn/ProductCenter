using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.Common;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using System.Data.Entity;
using System.Globalization;
using System.Reflection;

namespace PermaisuriCMS.BLL
{
   public class HMGroupConfigServices
    {
        private string ZeroMoeny = 0.ToString("C", new CultureInfo("en-US"));
        private readonly string Model_Name = "HMNUM Configuration";

       /// <summary>
       /// 组合产品唯一需要注意的地方是尺寸箱柜需要从子产品获取...
       /// </summary>
       /// <param name="qModel"></param>
       /// <returns></returns>
        public CMS_HMNUM_Model GetSingleHMGroup(CMS_HMNUM_Model qModel)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_HMNUM.Include(h => h.CMS_ProductCTN).Include(h => h.CMS_ProductDimension)
                   .FirstOrDefault(h => h.ProductID == qModel.ProductID);
                if (query == null)
                {
                    return null;
                }
                CMS_HMNUM_Model rModel = new CMS_HMNUM_Model
                {
                    ProductID = query.ProductID,
                    StockKeyID = query.StockKeyID,
                    Comments = query.Comments,
                    ProductName = query.ProductName,
                    StockKey = query.StockKey,
                    MasterPack = query.MasterPack,
                    StockKeyQTY = query.CMS_HM_Inventory == null ? 0 : query.CMS_HM_Inventory.StockkeyQTY.ConvertToNotNull(),
                    StatusID = query.StatusID,
                    NetWeight = query.NetWeight.ConvertToNotNull(),
                    CMS_HMNUM_Status = new CMS_HMNUM_Status_Model
                    {
                        StatusID = query.CMS_HMNUM_Status.StatusID,
                        StatusName = query.CMS_HMNUM_Status.StatusName
                    },
                    CMS_ShipVia_Type = new CMS_ShipViaType_Model
                    {
                        //这里设置为-1的原因是因为....避免和List里面的All(id=0)起冲突...2014年5月7日18:04:15
                        ShipViaTypeID = query.CMS_ShipViaType == null ? -1 : query.CMS_ShipViaType.ShipViaTypeID,
                        ShipViaTypeName = query.CMS_ShipViaType == null ? "" : query.CMS_ShipViaType.ShipViaTypeName//返回空给前端
                    },

                    //2013年12月12日19:30:35 这里有个前提，CMS_HMNUM的CategoryID存储的一定是子类的CatgeoryID,绝无可能是大类的ID！
                    //再也不要相信这个verCD的假设了，在测试了的时候被提了100遍的BUG，一百遍啊一百遍！！！
                    Category = new WebPO_Category_V_Model
                    {
                        CategoryID = query.WebPO_Category_V == null ? 0 : query.WebPO_Category_V.CategoryID,
                        CategoryName = query.WebPO_Category_V == null ? "NONE" : query.WebPO_Category_V.CategoryName,
                        OrderIndex = query.WebPO_Category_V == null ? 0 : query.WebPO_Category_V.OrderIndex.ConvertToNotNull(),
                        ParentCategoryID = query.WebPO_Category_V == null ? 0 : query.WebPO_Category_V.ParentCategoryID.ConvertToNotNull(),
                        ParentCategoryName = query.WebPO_Category_V == null ? "NONE" : query.WebPO_Category_V.ParentCategoryName
                    },

                    HMColour = new CMS_HMNUM_Colour_Model
                    {
                        ColourID = query.WebPO_Colour_V == null ? -1 : query.WebPO_Colour_V.ColourID,
                        ColourName = query.WebPO_Colour_V == null ? "NONE" : query.WebPO_Colour_V.ColourDesc
                    },

                    HMMaterial = new CMS_HMNUM_Material_Model
                    {
                        MaterialID = query.WebPO_Material_V == null ? -1 : query.WebPO_Material_V.MaterialID,
                        MaterialName = query.WebPO_Material_V == null ? "NONE" : query.WebPO_Material_V.MaterialName
                    },

                    HMNUM = query.HMNUM,
                    HM_Costing = new CMS_HM_Costing_Model//价格
                    {
                        FirstCost = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.FirstCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                        LandedCost = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.LandedCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                        EstimateFreight = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.EstimateFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),

                        OceanFreight = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.OceanFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                        USAHandlingCharge = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.USAHandlingCharge.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                        Drayage = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.Drayage.ConvertToNotNull().ToString("C", new CultureInfo("en-US"))
                    },

                    CTNList = query.CMS_ProductCTN.Select(h => new CMS_ProductCTN_Model//箱柜
                    {
                        CTNID = h.CTNID,
                        CTNTitle = h.CTNTitle,
                        CTNHeight = h.CTNHeight.ConvertToNotNull(),//JUST Testing 2013年11月11日11:36:48
                        CTNLength = h.CTNLength.HasValue ? h.CTNLength.Value : 0,
                        CTNWidth = h.CTNWidth.HasValue ? h.CTNWidth.Value : 0,
                        CTNCube = h.CTNCube.HasValue ? h.CTNCube.Value : 0,
                        CTNComment = h.CTNComment,
                        CTNWeight = h.CTNWeight.HasValue ? h.CTNWeight.Value : 0
                    }).ToList(),
                    DimList = query.CMS_ProductDimension.Select(d => new CMS_ProductDimension_Model//尺寸
                    {
                        DimID = d.DimID,
                        DimTitle = d.DimTitle,
                        DimHeight = d.DimHeight.HasValue ? d.DimHeight.Value : 0,
                        DimLength = d.DimLength.HasValue ? d.DimLength.Value : 0,
                        DimWidth = d.DimWidth.HasValue ? d.DimWidth.Value : 0,
                        DimCube = d.DimCube.HasValue ? d.DimCube.Value : 0,
                        DimComment = d.DimComment
                    }).ToList(),

                    Children_CMS_HMNUM_List = query.CMS_HMGroup_Relation.Select(r => new CMS_HMNUM_Model
                    {
                        SellSets = r.SellSets,
                        ProductID = r.CMS_HMNUM_Children.ProductID,
                        MasterPack = r.CMS_HMNUM_Children.MasterPack,
                        HMNUM = r.CMS_HMNUM_Children.HMNUM,
                        ProductName = r.CMS_HMNUM_Children.ProductName,
                        StockKey = r.CMS_HMNUM_Children.StockKey,
                        Comments = r.CMS_HMNUM_Children.Comments,
                        HM_Costing = new CMS_HM_Costing_Model
                        {
                            FirstCost = r.CMS_HMNUM_Children.CMS_HM_Costing == null ? "$0.00" : r.CMS_HMNUM_Children.CMS_HM_Costing.FirstCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                            LandedCost = r.CMS_HMNUM_Children.CMS_HM_Costing == null ? "$0.00" : r.CMS_HMNUM_Children.CMS_HM_Costing.LandedCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                            EstimateFreight = r.CMS_HMNUM_Children.CMS_HM_Costing == null ? "$0.00" : r.CMS_HMNUM_Children.CMS_HM_Costing.EstimateFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),

                            OceanFreight = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.OceanFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                            USAHandlingCharge = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.USAHandlingCharge.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                            Drayage = query.CMS_HM_Costing == null ? "$0.00" : query.CMS_HM_Costing.Drayage.ConvertToNotNull().ToString("C", new CultureInfo("en-US"))
                        }
                    }).ToList(),

                    SKUList = query.SKU_HM_Relation.Select(r => new CMS_SKU_Model//和当前HM关联的SKUOrder
                    {
                        SKUID = r.SKUID,
                        SKU = r.CMS_SKU.SKU,
                        ProductName = r.CMS_SKU.ProductName,
                        ChannelID = r.CMS_SKU.ChannelID,
                        ChannelName = r.CMS_SKU.Channel.ChannelName,
                        BrandID = r.CMS_SKU.BrandID,
                        BrandName = r.CMS_SKU.Brand.BrandName
                    }).ToList(),

                    MediaList = query.CMS_StockKey.MediaLibrary.Select(r => new MediaLibrary_Model
                    { //2013年12月25日10:23:15
                        MediaID = r.MediaID,
                        ProductID = r.ProductID,
                        HMNUM = r.HMNUM,
                        ImgName = r.ImgName,
                        fileFormat = r.fileFormat,
                        MediaType = r.MediaType,
                        SerialNum = r.SerialNum,
                        IsPrimaryImages = r.PrimaryImage.HasValue ? r.PrimaryImage.Value : false,
                        fileHeight = r.fileHeight.ConvertToNotNull(),
                        fileSize = r.fileSize,
                        fileWidth = r.fileWidth.ConvertToNotNull(),
                        CloudStatusID = r.CloudStatusID,
                        strCreateOn = r.CreateOn.ToString("yyyy-MM-dd HH:mm:ss"),
                        CMS_SKU = r.SKU_Media_Relation.Where(k => k.MediaID == r.MediaID).Select(s => new CMS_SKU_Model
                        {
                            ChannelName = s.CMS_SKU.Channel.ChannelName,
                            ProductName = s.CMS_SKU.ProductName,
                            SKU = s.CMS_SKU.SKU
                        }).ToList()
                    }).ToList()
                };
                return rModel;
            }
        }



        /// <summary>
        /// 更新HMNUMCosting的信息，用于HMNUM Configuration页面的的Costing的编辑更新
        /// 需要注意的是每一次的跟新都将在库表新增一条价格信息，影响将来报表的生成。
        /// 虽然和HMNUMController页面的方法一样，但是还是分开维护，因为2个展示有可能不同，遇到需求变动会变得痛苦！
        /// </summary>
        /// <param name="model"></param>
        /// <param name="costing">输入输出参数，输入时候代表客户端需要更新的价格传递给服务器，输出代表价格跟新后在数据库的实际存储方式。比如，客户端输入10个小数点的数字后....</param>
        /// <param name="User_Account"></param>
        /// <returns></returns>
        public bool EditHMNUMCosting(CMS_HMNUM_Model model, ref CMS_HM_Costing_Model costing, string User_Account)
        {
            //逻辑：先讲当前最新的价格插入到Costing表（注意是新增不是编辑），然后更新当前HMNUM的Costing信息，取最新的那条。
            //EF本身自带有Transaction功能。
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                int retVal = 0;
                var newCosting = new CMS_HM_Costing
                {
                    CreateBy = User_Account,
                    CreateOn = DateTime.Now,
                    EffectiveDate = DateTime.Now,
                    HMNUM = costing.HMNUM, // out: use unsigned parameter costing....2013年11月14日11:25:25
                    FirstCost = Convert.ToDecimal(costing.FirstCost),
                    LandedCost = Convert.ToDecimal(costing.LandedCost),
                    EstimateFreight = Convert.ToDecimal(costing.EstimateFreight),

                    OceanFreight = Convert.ToDecimal(costing.OceanFreight),
                    USAHandlingCharge = Convert.ToDecimal(costing.USAHandlingCharge),
                    Drayage = Convert.ToDecimal(costing.Drayage)
                };

                var HMNUM = db.CMS_HMNUM.FirstOrDefault(h => h.ProductID == model.ProductID);
                HMNUM.CMS_HM_Costing = newCosting;


                HMNUM.SKU_HM_Relation.Select(r => r.CMS_SKU.CMS_Ecom_Sync == null ? r.CMS_SKU.CMS_Ecom_Sync = new CMS_Ecom_Sync
                {
                    StatusID = 1//这里可以不用做任何设置，因为后面那个操作会全面覆盖这个...
                } : r.CMS_SKU.CMS_Ecom_Sync).ForEach(k =>
                {
                    k.StatusID = 0;
                    k.StatusDesc = "NeedSend";
                    k.UpdateBy = User_Account;
                    k.UpdateOn = DateTime.Now;
                });

                StringBuilder sb = new StringBuilder();
                sb.Append(" <b> [FirstCost] </b> : old value =  <span style='color:red'>  " + HMNUM.CMS_HM_Costing.FirstCost + " </span> , new Value = <span style='color:red'>  " + newCosting.FirstCost + "  </span>");
                sb.Append("<br>");

                sb.Append(" <b> [LandedCost] </b> : old value =  <span style='color:red'>  " + HMNUM.CMS_HM_Costing.LandedCost + " </span> , new Value = <span style='color:red'>  " + newCosting.LandedCost + "  </span>");
                sb.Append("<br>");

                sb.Append(" <b> [EstimateFreight] </b> : old value =  <span style='color:red'>  " + HMNUM.CMS_HM_Costing.EstimateFreight + " </span> , new Value = <span style='color:red'>  " + newCosting.EstimateFreight + "  </span>");

                sb.Append(" <b> [OceanFreight] </b> : old value =  <span style='color:red'>  " + HMNUM.CMS_HM_Costing.OceanFreight + " </span> , new Value = <span style='color:red'>  " + newCosting.OceanFreight + "  </span>");

                sb.Append(" <b> [USAHandlingCharge] </b> : old value =  <span style='color:red'>  " + HMNUM.CMS_HM_Costing.USAHandlingCharge + " </span> , new Value = <span style='color:red'>  " + newCosting.USAHandlingCharge + "  </span>");

                sb.Append(" <b> [Drayage] </b> : old value =  <span style='color:red'>  " + HMNUM.CMS_HM_Costing.Drayage + " </span> , new Value = <span style='color:red'>  " + newCosting.Drayage + "  </span>");
                sb.Append("<br>");

                BllExtention.DbRecorder(new LogOfUserOperatingDetails
                {
                    ModelName = Model_Name,
                    ActionName = MethodBase.GetCurrentMethod().Name,
                    ActionType = LogActionTypeEnum.Inert.GetHashCode(),
                    ProductID = HMNUM.ProductID,
                    HMNUM = HMNUM.HMNUM,
                    Descriptions = sb.ToString(),
                    CreateBy = User_Account,
                    CreateOn = DateTime.Now
                });

                retVal = db.SaveChanges();


                costing.FirstCost = newCosting.FirstCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US"));
                costing.LandedCost = newCosting.LandedCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US"));
                costing.EstimateFreight = newCosting.EstimateFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US"));

                costing.OceanFreight = newCosting.OceanFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US"));
                costing.USAHandlingCharge = newCosting.USAHandlingCharge.ConvertToNotNull().ToString("C", new CultureInfo("en-US"));
                costing.Drayage = newCosting.Drayage.ConvertToNotNull().ToString("C", new CultureInfo("en-US"));

                return retVal > 0;
            }
        }


       /// <summary>
       /// 2014年3月14日
       /// </summary>
       /// <param name="ProductID"></param>
       /// <param name="ChildrenProductID"></param>
       /// <param name="SellSets"></param>
       /// <param name="user"></param>
       /// <returns></returns>
        public bool UpdateSellSets(long ProductID, long ChildrenProductID, int SellSets, User_Profile_Model user)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var relation = db.CMS_HMGroup_Relation.FirstOrDefault(h => h.ProductID == ProductID && h.ChildrenProductID == ChildrenProductID);
                if (relation == null)
                {
                    return false;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(" <b> [sub-HMNUM=" + relation.CMS_HMNUM_Children.HMNUM + ", SellSets:] </b> : old value =  <span style='color:red'>  " + relation.SellSets + " </span> , new Value = <span style='color:red'>  " + SellSets + "  </span>");
                sb.Append("<br>");

                 relation.SellSets = SellSets;

                 relation.CMS_HMNUM.SKU_HM_Relation.Select(r => r.CMS_SKU.CMS_Ecom_Sync == null ? r.CMS_SKU.CMS_Ecom_Sync = new CMS_Ecom_Sync
                 {
                     StatusID = 1//这里可以不用做任何设置，因为后面那个操作会全面覆盖这个...
                 } : r.CMS_SKU.CMS_Ecom_Sync).ForEach(k =>
                 {
                     k.StatusID = 0;
                     k.StatusDesc = "NeedSend";
                     k.UpdateBy = user.User_Account;
                     k.UpdateOn = DateTime.Now;
                 });


                BllExtention.DbRecorder(new LogOfUserOperatingDetails
                {
                    ModelName = Model_Name,
                    ActionName = MethodBase.GetCurrentMethod().Name,
                    ActionType = LogActionTypeEnum.Inert.GetHashCode(),
                    ProductID = relation.ProductID,
                    HMNUM = relation.CMS_HMNUM.HMNUM,
                    Descriptions = sb.ToString(),
                    CreateBy = user.User_Account,
                    CreateOn = DateTime.Now
                });

                return db.SaveChanges() > -1;
            }
        }
       
        /// <summary>
        /// 编辑HM#基本信息，用于HMNUM Configuration页面
        /// </summary>
        /// <param name="model"></param>
        /// <param name="User_Account"></param>
        /// <returns></returns>
        public bool EditHMBasicInfo(CMS_HMNUM_Model model, string User_Account)
        {

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var HMNUM = db.CMS_HMNUM.FirstOrDefault(h => h.ProductID == model.ProductID);
                HMNUM.StockKey = model.StockKey;
                HMNUM.Comments = model.Comments;


                HMNUM.SKU_HM_Relation.Select(r => r.CMS_SKU.CMS_Ecom_Sync == null ? r.CMS_SKU.CMS_Ecom_Sync = new CMS_Ecom_Sync
                {
                    StatusID = 1//这里可以不用做任何设置，因为后面那个操作会全面覆盖这个...
                } : r.CMS_SKU.CMS_Ecom_Sync).ForEach(k =>
                {
                    k.StatusID = 0;
                    k.StatusDesc = "NeedSend";
                    k.UpdateBy = User_Account;
                    k.UpdateOn = DateTime.Now;
                });


                var entry = db.Entry(HMNUM);
                BllExtention.DbRecorder(entry, new LogOfUserOperatingDetails
                {
                    ModelName = Model_Name,
                    ActionName = MethodBase.GetCurrentMethod().Name,
                    ActionType = LogActionTypeEnum.Update.GetHashCode(),
                    ProductID = HMNUM.ProductID,
                    HMNUM = HMNUM.HMNUM,
                    CreateBy = User_Account,
                    CreateOn = DateTime.Now
                });

                HMNUM.ModifyBy = User_Account;
                HMNUM.ModifyOn = DateTime.Now;

              

                return db.SaveChanges() > 0;
            }
        }
    }
}
