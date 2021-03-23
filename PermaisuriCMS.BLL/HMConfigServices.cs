using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using System.Data.Entity;
using System.Reflection;

namespace PermaisuriCMS.BLL
{
    public class HmConfigServices
    {
        //private string _zeroMoeny = 0.ToString("C", new CultureInfo("en-US"));
        private const string ModelName = "HMNUM Configuration";

        public CMS_HMNUM_Model GetSingleHm(CMS_HMNUM_Model qModel)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_HMNUM.Include(h=>h.CMS_ProductCTN).Include(h=>h.CMS_ProductDimension)
                    .FirstOrDefault(h => h.ProductID == qModel.ProductID);
                if (query == null)
                {
                    return null;
                }
                var rModel = new CMS_HMNUM_Model
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
                    //2013年12月12日18:21:54 这里有个前提，CMS_HMNUM的CategoryID存储的一定是子类的CatgeoryID,绝无可能是大类的ID！
                    // 再也不要相信这个verCD的假设了，在测试了的时候被提了100遍的BUG，一百遍啊一百遍！！！ 2014年2月19日15:31:03
                    Category = new WebPO_Category_V_Model
                    {
                        CategoryID = query.WebPO_Category_V == null ? 0 : query.WebPO_Category_V.CategoryID,
                        CategoryName = query.WebPO_Category_V == null ? "NONE" : query.WebPO_Category_V.CategoryName,
                        OrderIndex = query.WebPO_Category_V == null ? 0 : query.WebPO_Category_V.OrderIndex.ConvertToNotNull(),
                        ParentCategoryID = query.WebPO_Category_V == null ? 0 : query.WebPO_Category_V.ParentCategoryID.ConvertToNotNull(),
                        ParentCategoryName = query.WebPO_Category_V == null ? "NONE" : query.WebPO_Category_V.ParentCategoryName
                    },
                    //CategoryID = query.WebPO_Category_V.CategoryID,
                    //CategoryName = query.WebPO_Category_V.CategoryName,
                    //SubCategoryID = query.CategoryID,
                    //SubCategoryName =query.WebPO_Category_V.SubCategoryName,

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

                    Parents_CMS_HMNUM_List = query.CMS_HMGroup_Relation_SUB.Select(r => new CMS_HMNUM_Model
                    {
                        ProductID = r.CMS_HMNUM.ProductID,
                        MasterPack = r.CMS_HMNUM.MasterPack,
                        HMNUM = r.CMS_HMNUM.HMNUM,
                        ProductName = r.CMS_HMNUM.ProductName,
                        StockKey = r.CMS_HMNUM.StockKey,
                        StockKeyQTY = r.CMS_HMNUM.CMS_HM_Inventory == null ? 0 : r.CMS_HMNUM.CMS_HM_Inventory.StockkeyQTY.ConvertToNotNull()
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
                        IsPrimaryImages = r.PrimaryImage.HasValue && r.PrimaryImage.Value,
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
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public bool EditHmnumCosting(CMS_HMNUM_Model model, ref CMS_HM_Costing_Model costing, string userAccount)
        {
            //逻辑：先讲当前最新的价格插入到Costing表（注意是新增不是编辑），然后更新当前HMNUM的Costing信息，取最新的那条。
            //EF本身自带有Transaction功能。
            using (var db = new PermaisuriCMSEntities())
            {
                var newCosting = new CMS_HM_Costing
                {
                    CreateBy = userAccount,
                    CreateOn = DateTime.Now,
                    EffectiveDate = DateTime.Now,
                    HMNUM = costing.HMNUM, // out: use unsigned parameter costing....2013年11月14日11:25:25
                    FirstCost = Convert.ToDecimal(costing.FirstCost),
                    LandedCost = Convert.ToDecimal(costing.LandedCost),
                    EstimateFreight = Convert.ToDecimal(costing.EstimateFreight),

                    OceanFreight = Convert.ToDecimal(costing.OceanFreight),
                    USAHandlingCharge = Convert.ToDecimal(costing.USAHandlingCharge),
                    Drayage = Convert.ToDecimal(costing.Drayage),
                };

                //db.CMS_HM_Costing.Add(newCosting);
                //long newCostID = newCosting.HMCostID;

                var hmnum = db.CMS_HMNUM.FirstOrDefault(h => h.ProductID == model.ProductID);

                //HMNUM.HMCostID = newCostID;
                if (hmnum != null)
                {
                    hmnum.CMS_HM_Costing = newCosting;

                    hmnum.SKU_HM_Relation.Select(r => r.CMS_SKU.CMS_Ecom_Sync ?? (r.CMS_SKU.CMS_Ecom_Sync=new CMS_Ecom_Sync{
                        StatusID = 1//这里可以不用做任何设置，因为后面那个操作会全面覆盖这个...
                    })).ForEach(k=> 
                    {
                        k.StatusID = 0;
                        k.StatusDesc = "NeedSend";
                        k.UpdateBy = userAccount;
                        k.UpdateOn = DateTime.Now;
                    });
                

                    var sb = new StringBuilder();
                    sb.Append(" <b> [FirstCost] </b> : old value =  <span style='color:red'>  " + hmnum.CMS_HM_Costing.FirstCost + " </span> , new Value = <span style='color:red'>  " + newCosting.FirstCost + "  </span>");
                    sb.Append("<br>");

                    sb.Append(" <b> [LandedCost] </b> : old value =  <span style='color:red'>  " + hmnum.CMS_HM_Costing.LandedCost + " </span> , new Value = <span style='color:red'>  " + newCosting.LandedCost + "  </span>");
                    sb.Append("<br>");

                    sb.Append(" <b> [EstimateFreight] </b> : old value =  <span style='color:red'>  " + hmnum.CMS_HM_Costing.EstimateFreight + " </span> , new Value = <span style='color:red'>  " + newCosting.EstimateFreight + "  </span>");
                
                    sb.Append(" <b> [OceanFreight] </b> : old value =  <span style='color:red'>  " + hmnum.CMS_HM_Costing.OceanFreight + " </span> , new Value = <span style='color:red'>  " + newCosting.OceanFreight + "  </span>");

                    sb.Append(" <b> [USAHandlingCharge] </b> : old value =  <span style='color:red'>  " + hmnum.CMS_HM_Costing.USAHandlingCharge + " </span> , new Value = <span style='color:red'>  " + newCosting.USAHandlingCharge + "  </span>");

                    sb.Append(" <b> [Drayage] </b> : old value =  <span style='color:red'>  " + hmnum.CMS_HM_Costing.Drayage + " </span> , new Value = <span style='color:red'>  " + newCosting.Drayage + "  </span>");
                    sb.Append("<br>");

                    BllExtention.DbRecorder(new LogOfUserOperatingDetails
                    {
                        ModelName = ModelName,
                        ActionName = MethodBase.GetCurrentMethod().Name,
                        ActionType = LogActionTypeEnum.Inert.GetHashCode(),
                        ProductID = hmnum.ProductID,
                        HMNUM = hmnum.HMNUM,
                        Descriptions = sb.ToString(),
                        CreateBy = userAccount,
                        CreateOn = DateTime.Now
                    });

                    db.SaveChanges();

                    newCosting.HisProductID = hmnum.ProductID;
                }
                var retVal = db.SaveChanges();
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
        /// 编辑HM#基本信息，用于HMNUM Configuration页面
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public bool EditHmBasicInfo(CMS_HMNUM_Model model, string userAccount)
        {

            using (var db = new PermaisuriCMSEntities())
            {
                var hmnum = db.CMS_HMNUM.FirstOrDefault(h => h.ProductID == model.ProductID);
                if (hmnum == null) return db.SaveChanges() > 0;
                hmnum.StockKey = model.StockKey;
                hmnum.Comments = model.Comments;
                hmnum.StatusID = model.StatusID;
                hmnum.ShipViaTypeID = model.ShipViaTypeID;
                hmnum.NetWeight = model.NetWeight;
                hmnum.SKU_HM_Relation.Select(r => r.CMS_SKU.CMS_Ecom_Sync ?? (r.CMS_SKU.CMS_Ecom_Sync = new CMS_Ecom_Sync
                {
                    StatusID = 1//这里可以不用做任何设置，因为后面那个操作会全面覆盖这个...
                })).ForEach(k =>
                {
                    k.StatusID = 0;
                    k.StatusDesc = "NeedSend";
                    k.UpdateBy = userAccount;
                    k.UpdateOn = DateTime.Now;
                });

                var entry = db.Entry(hmnum);
                BllExtention.DbRecorder(entry, new LogOfUserOperatingDetails
                {
                    ModelName = ModelName,
                    ActionName = MethodBase.GetCurrentMethod().Name,
                    ActionType = LogActionTypeEnum.Update.GetHashCode(),
                    ProductID = hmnum.ProductID,
                    HMNUM = hmnum.HMNUM,
                    CreateBy = userAccount,
                    CreateOn = DateTime.Now
                });

                hmnum.ModifyBy = userAccount;
                hmnum.ModifyOn = DateTime.Now;


                return db.SaveChanges() > 0;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctnModel"></param>
        /// <param name="userAccount"></param>
        /// <param name="ctnid"></param>
        /// <returns></returns>
        public bool AddHmCarton(CMS_ProductCTN_Model ctnModel, string userAccount,out long ctnid)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var hmnum = db.CMS_HMNUM.FirstOrDefault(h => h.ProductID == ctnModel.ProductID);
                if (hmnum == null)
                {
                    ctnid = 0;
                    return false;
                }

                var newModel = new CMS_ProductCTN
                {
                    CTNComment = ctnModel.CTNComment,
                    CTNCube = ctnModel.CTNCube,
                    CTNHeight = ctnModel.CTNHeight,
                    CTNLength = ctnModel.CTNLength,
                    CTNTitle = ctnModel.CTNTitle,
                    CTNWidth = ctnModel.CTNWidth,
                    HMNUM = ctnModel.HMNUM,
                    ProductID = ctnModel.ProductID,
                    UpdateBy = userAccount,
                    UpdateOn = DateTime.Now,
                    CreateOn = DateTime.Now,
                    CTNWeight = ctnModel.CTNWeight
                };
                
                db.CMS_ProductCTN.Add(newModel);

                hmnum.SKU_HM_Relation.Select(r => r.CMS_SKU.CMS_Ecom_Sync ?? (r.CMS_SKU.CMS_Ecom_Sync = new CMS_Ecom_Sync
                {
                    StatusID = 1//这里可以不用做任何设置，因为后面那个操作会全面覆盖这个...
                })).ForEach(k =>
                {
                    k.StatusID = 0;
                    k.StatusDesc = "NeedSend";
                    k.UpdateBy = userAccount;
                    k.UpdateOn = DateTime.Now;
                });
              
                BllExtention.DbRecorder(new LogOfUserOperatingDetails
                {
                    ModelName = ModelName,
                    ActionName = MethodBase.GetCurrentMethod().Name,
                    ActionType = LogActionTypeEnum.Inert.GetHashCode(),
                    ProductID = hmnum.ProductID,
                    HMNUM = hmnum.HMNUM,
                    Descriptions = "Add new carton  [ <b>" + newModel.CTNTitle + " </b>] for " + hmnum.HMNUM,
                    CreateBy = userAccount,
                    CreateOn = DateTime.Now
                });


                var retVal = db.SaveChanges();
                ctnid = newModel.CTNID;
                return retVal > 0;
            }
        }


        public bool DeleteCarton(long ctnid)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                //这样做的好处在于能直接删除一个对象，而不需要先从数据库中提取数据，创建实体对象，再查找并删除之，从而能有效地提升效率
                var ctn = new CMS_ProductCTN { CTNID = ctnid };
                db.Set<CMS_ProductCTN>().Attach(ctn);
                db.CMS_ProductCTN.Remove(ctn);
                return db.SaveChanges() > 0;
            }
        }

        public bool AddDimension(CMS_ProductDimension_Model dimModel, string userAccount, out long dimId)
        {
            using (var db = new PermaisuriCMSEntities())
            {

                var hmnum = db.CMS_HMNUM.FirstOrDefault(h => h.ProductID == dimModel.ProductID);

                var newModel = new CMS_ProductDimension
                {
                    CreateOn = DateTime.Now,
                    DimComment = dimModel.DimComment,
                    DimCube = dimModel.DimCube,
                    DimHeight = dimModel.DimHeight,
                    DimLength = dimModel.DimLength,
                    DimTitle = dimModel.DimTitle,
                    DimWidth = dimModel.DimWidth,
                    HMNUM = dimModel.HMNUM,
                    ProductID = dimModel.ProductID,
                    UpdateBy = userAccount,
                    UpdateOn = DateTime.Now
                };
                db.CMS_ProductDimension.Add(newModel);

                if (hmnum != null)
                {
                    hmnum.SKU_HM_Relation.Select(r => r.CMS_SKU.CMS_Ecom_Sync ?? (r.CMS_SKU.CMS_Ecom_Sync = new CMS_Ecom_Sync
                    {
                        StatusID = 1//这里可以不用做任何设置，因为后面那个操作会全面覆盖这个...
                    })).ForEach(k =>
                    {
                        k.StatusID = 0;
                        k.StatusDesc = "NeedSend";
                        k.UpdateBy = userAccount;
                        k.UpdateOn = DateTime.Now;
                    });

                    BllExtention.DbRecorder(new LogOfUserOperatingDetails
                    {
                        ModelName = ModelName,
                        ActionName = MethodBase.GetCurrentMethod().Name,
                        ActionType = LogActionTypeEnum.Inert.GetHashCode(),
                        ProductID = hmnum.ProductID,
                        HMNUM = hmnum.HMNUM,
                        Descriptions = "Add new dimension  [ <b>" + newModel.DimTitle + " </b>] for " + hmnum.HMNUM,
                        CreateBy = userAccount,
                        CreateOn = DateTime.Now
                    });
                }

                var retVal = db.SaveChanges();
                dimId = newModel.DimID;
                return retVal > 0;
            }
        }

        public bool DeleteDimension(long dimId)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                //这样做的好处在于能直接删除一个对象，而不需要先从数据库中提取数据，创建实体对象，再查找并删除之，从而能有效地提升效率
                var dim = new CMS_ProductDimension { DimID  = dimId };
                db.Set<CMS_ProductDimension>().Attach(dim);
                db.CMS_ProductDimension.Remove(dim);
                return db.SaveChanges() > 0;
            }
        }

        public bool EditHmCartons(IEnumerable<CMS_ProductCTN_Model> ctnList, string userAccount)
        {
            if (ctnList == null) throw new ArgumentNullException("ctnList");

            using (var db = new PermaisuriCMSEntities())
            {
                var retVal = 0;
                foreach (var model in ctnList)
                {
                    var ctn = db.CMS_ProductCTN.FirstOrDefault(c => c.CTNID == model.CTNID);
                    if (ctn != null)
                    {
                        ctn.CTNCube = model.CTNCube;
                        ctn.CTNLength = model.CTNLength;
                        ctn.CTNWidth = model.CTNWidth;
                        ctn.CTNHeight = model.CTNHeight;
                        ctn.CTNComment = model.CTNComment;
                        ctn.UpdateBy = userAccount;
                        ctn.CTNWeight = model.CTNWeight;//新增重量信息 2014年3月6日


                        ctn.CMS_HMNUM.SKU_HM_Relation.Select(r => r.CMS_SKU.CMS_Ecom_Sync ?? (r.CMS_SKU.CMS_Ecom_Sync = new CMS_Ecom_Sync
                        {
                            StatusID = 1//这里可以不用做任何设置，因为后面那个操作会全面覆盖这个...
                        })).ForEach(k =>
                        {
                            k.StatusID = 0;
                            k.StatusDesc = "NeedSend";
                            k.UpdateBy = userAccount;
                            k.UpdateOn = DateTime.Now;
                        });

                        var entry = db.Entry(ctn);
                        BllExtention.DbRecorder(entry, new LogOfUserOperatingDetails
                        {
                            ModelName = ModelName,
                            ActionName = MethodBase.GetCurrentMethod().Name,
                            ActionType = LogActionTypeEnum.Update.GetHashCode(),
                            ProductID = ctn.ProductID,
                            HMNUM = ctn.HMNUM,
                            CreateBy = userAccount,
                            CreateOn = DateTime.Now
                        });


                        ctn.UpdateOn = DateTime.Now;
                    }


                    retVal = db.SaveChanges();//如果上面的几个Value都没有改变，则返回的值还是0 2013年11月15日17:20:03
                }
                return retVal > 0;
            }
        }


        public bool EditHmDimensions(IEnumerable<CMS_ProductDimension_Model> ctnList, string userAccount)
        {

            using (var db = new PermaisuriCMSEntities())
            {
                var retVal = 0;
                foreach (var model in ctnList)
                {
                    var ctn = db.CMS_ProductDimension.FirstOrDefault(c => c.DimID == model.DimID);
                    if (ctn == null) throw new ArgumentNullException("ctnList");
                    ctn.DimCube = model.DimCube;
                    ctn.DimLength = model.DimLength;
                    ctn.DimWidth = model.DimWidth;
                    ctn.DimHeight = model.DimHeight;
                    ctn.DimComment = model.DimComment;


                    ctn.CMS_HMNUM.SKU_HM_Relation.Select(r => r.CMS_SKU.CMS_Ecom_Sync ?? (r.CMS_SKU.CMS_Ecom_Sync = new CMS_Ecom_Sync
                    {
                        StatusID = 1//这里可以不用做任何设置，因为后面那个操作会全面覆盖这个...
                    })).ForEach(k =>
                    {
                        k.StatusID = 0;
                        k.StatusDesc = "NeedSend";
                        k.UpdateBy = userAccount;
                        k.UpdateOn = DateTime.Now;
                    });

                    var entry = db.Entry(ctn);
                    BllExtention.DbRecorder(entry, new LogOfUserOperatingDetails
                    {
                        ModelName = ModelName,
                        ActionName = MethodBase.GetCurrentMethod().Name,
                        ActionType = LogActionTypeEnum.Update.GetHashCode(),
                        ProductID = ctn.ProductID,
                        HMNUM = ctn.HMNUM,
                        CreateBy = userAccount,
                        CreateOn = DateTime.Now
                    });

                    ctn.UpdateBy = userAccount;
                    ctn.UpdateOn = DateTime.Now;


                    retVal = db.SaveChanges();//如果上面的几个Value都没有改变，则返回的值还是0 2013年11月15日17:20:03
                }
                return retVal > 0;
            }
        }
    }
}
