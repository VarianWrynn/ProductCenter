using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using NLog;
using System.Data.Entity.Validation;
using System.Configuration;


namespace InitHMAllItem
{
    /// <summary>
    /// 操作信息事件代理
    /// </summary>
    //public delegate void OperateNotifyHandler(object sender, HMEventArgs e);

    public class GroupDataProcess
    {
        /// <summary>
        /// 操作信息事件
        /// </summary>
        public event OperateNotifyHandler OperateNotify;

        private static Logger HMLog = LogManager.GetCurrentClassLogger();

        public static string path = string.Empty;
        public static string connStr = string.Empty;
        public static string UpdateBy = "HM-ALL ITEM";

        protected virtual void OnOperateNotify(object sender, HMEventArgs e)
        {
            if (OperateNotify != null)

                OperateNotify(sender, e);
        }


        public GroupDataProcess(string strPath)
        {
            path = strPath;
            connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;";
        }

       
        //public void StartProcess()
        public void StartProcess()
        {
            //构建连接字符串
            OleDbConnection Conn = new OleDbConnection(connStr);
            Conn.Open();
            //填充数据

            String SheetName = ConfigurationManager.AppSettings["SheetName"];

            string sql = string.Format("select * from [{0}$]", SheetName);
            OleDbDataAdapter da = new OleDbDataAdapter(sql, connStr);
            DataSet ds = new DataSet();
            da.Fill(ds);
            Conn.Close();
            //StringBuilder sb = new StringBuilder();
            long groupProductID = 0;
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                int iCount = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (iCount > 1659)//发现无限循环下去，如果大于这个数据，就退出
                    {
                        HMLog.Info("已经全部导入数据库，可以退出当前程序");
                        OnOperateNotify(this, new HMEventArgs("已经全部导入数据库，可以退出当前程序"));
                        break;
                    }
                    iCount++;
                    try
                    {
                        #region 提出Excel数据
                        string lineHMType = dr["Group"].ToString();
                        OnOperateNotify(this, new HMEventArgs(String.Format(">>>>>>>>>>>>>.Start the Lien of {0} Item <<<<<<<<<<<<<<<<<<,,,", iCount)));
                        HMLog.Info(String.Format(">>>>>>>>>>>>>.Start the Number of {0} Item <<<<<<<<<<<<<<<<<<,,,", iCount));
                        string stockKey = dr["STOCK KEY-sales data"].ToString();
                        String HMNUM = string.Empty;
                        string BasicHMNUM = dr["HMNUM-New"].ToString();
                        string GroupHMNUM = dr["WEPO HM#"].ToString();
                        //IsGroup = lineHMType == "group" ? true : false,
                        //if (lineHMType == "group")
                        if (string.Compare(lineHMType, "Group", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            HMNUM = GroupHMNUM;
                        }
                        else
                        {
                            HMNUM = BasicHMNUM;
                        }

                        string ProductName = dr["Product Name"].ToString();
                        string Category = dr["Category"].ToString();
                        string SubCategory = dr["SubCategory"].ToString();
                        string strFirstCost = dr["first cost"].ToString();
                        string strLandedCost = dr["landed cost"].ToString();
                        string strShippingCost = dr[" shipping cost"].ToString();
                        decimal landedCost = 0;
                        decimal firstCost = 0;
                        decimal ShippingCost = 0;
                        decimal.TryParse(strFirstCost, out firstCost);
                        decimal.TryParse(strLandedCost, out landedCost);
                        decimal.TryParse(strShippingCost, out ShippingCost);
                        string strBoxWeight = dr["Box Weight"].ToString();
                        string strCTNLength = dr["Box Length"].ToString();
                        string strCTNWidth = dr["Box Width"].ToString();
                        string strCTNHeight = dr["Box Height"].ToString();

                        decimal CTNLength = 0;
                        decimal.TryParse(strCTNLength, out CTNLength);
                        decimal CTNWidth = 0;
                        decimal.TryParse(strCTNWidth, out CTNWidth);
                        decimal CTNHeight = 0;
                        decimal.TryParse(strCTNHeight, out CTNHeight);
                        decimal CTNWeight = 0;
                        decimal.TryParse(strBoxWeight, out CTNWeight);

                        string prDimensions = dr["Product Dimension"].ToString();
                        decimal DimLength = 0;
                        decimal DimWidth = 0;
                        decimal DimHeight = 0;
                        if (!String.IsNullOrEmpty(prDimensions))
                        {
                            var dims = prDimensions.Split('x');
                            if (dims.Length > 3)//不规范 以后处理
                            {
                                decimal.TryParse(dims[0], out DimLength);
                                decimal.TryParse(dims[1], out DimWidth);
                                decimal.TryParse(dims[2], out DimHeight);
                            }
                        }

                        string strLoadability = dr["Loadability-webpo"].ToString();
                        decimal Loadability = 0;
                        decimal.TryParse(strLoadability, out Loadability);


                        string SKUORDER = dr["SKUORDER"].ToString();
                        string MERCHANTID = dr["MERCHANTID"].ToString().Trim();
                        string PrDescription = dr["Intro Sentence(s)"].ToString();
                        string Bullets = dr["Bullet Description"].ToString();
                        string UPC = String.Empty;
                        string BESTUPC = dr["BEST UPC"].ToString();
                        // string CAUPC = dr["COSTCO UPC"].ToString(); COSTCO/AMAZON UPC
                        string CAUPC = dr["COSTCO/AMAZON UPC"].ToString();
                        // string GUPC = dr["GROUPON UPC"].ToString();
                        string GDFUPC = dr["GDF UPC"].ToString();
                        if (!String.IsNullOrEmpty(BESTUPC))
                        {
                            UPC = BESTUPC;
                        }
                        else if (!String.IsNullOrEmpty(CAUPC))
                        {
                            UPC = CAUPC;
                        }
                        //else if (!String.IsNullOrEmpty(GUPC))
                        //{
                        //    UPC = GUPC;
                        //}
                        else if (!String.IsNullOrEmpty(GDFUPC))
                        {
                            UPC = GDFUPC;
                        }

                        //normalselling是我们和网站的价钱，retail price是网站和终端客人的价钱 Boonie
                        string strRetail = dr["Normallselling"].ToString();
                        string strSalePrice = dr["retail price"].ToString();
                        decimal Retail = 0;
                        decimal.TryParse(strRetail, out Retail);
                        decimal SalePrice = 0;
                        decimal.TryParse(strSalePrice, out SalePrice);


                        //string StrSkuColor = dr["New Color"].ToString();
                        //long SKUColorID = 0;
                        //long.TryParse(StrSkuColor, out SKUColorID);

                        //string StrMaterialColor = dr["New Material"].ToString();
                        //long SKUMaterialID = 0;
                        //long.TryParse(StrMaterialColor, out SKUMaterialID);

                        #endregion

                        #region HM的价格信息
                        var newHMCost = new CMS_HM_Costing
                        {
                            CreateBy = UpdateBy,
                            CreateOn = DateTime.Now,
                            EffectiveDate = DateTime.Now,
                            EstimateFreight = ShippingCost,
                            LandedCost = landedCost,
                            FirstCost = firstCost,
                            HMNUM = HMNUM
                        };
                        #endregion HM的价格信息

                        #region HM的类别ID


                        long HMCategoryID = 0; long HMColourID = 0; long HMMaterialID = 0; long MasterPack = 1;
                        //long HMColorID = 0;从这张表里面获取 类别 颜色 和材料ID
                        var temObj = db.WebPO_HM_Colour_Material_V.FirstOrDefault(v => v.HMNUM == HMNUM);
                        string HMName = "";//如果为空...
                        if (temObj != null)
                        {
                            HMCategoryID = temObj.CategoryID.ConvertToNotNull();
                            HMColourID = temObj.ColourID;
                            HMMaterialID = temObj.MaterialID;
                            HMName = temObj.ProductName;
                            MasterPack = temObj.MasterPack;
                        }

                        #endregion  #region HM的类别ID

                        #region HM的基础信息
                        var newHM = new CMS_HMNUM
                        {
                            HMNUM = HMNUM,
                            MasterPack = MasterPack,
                            ProductName = lineHMType == "Group" ? ProductName : HMName,//如果是组合产品，则不取HMNUM的名称（因为全是空）2014年3月19日
                            //StockKey = lineHMType == "group" ? "0" : stockKey,
                            StockKey = lineHMType == "Group" ? HMNUM : stockKey,
                            CategoryID = HMCategoryID,
                            MaterialID = HMMaterialID,
                            ColourID = HMColourID,
                            SubCategoryID = 0,
                            IsGroup = lineHMType == "Group" ? true : false,
                            StatusID = 0,
                            Loadability = Loadability,
                            CreateOn = DateTime.Now,
                            CreateBy = UpdateBy,
                            ModifyOn = DateTime.Now,
                            ModifyBy = UpdateBy
                        };
                        #endregion

                        #region 插入箱子基础信息
                        //插入尺寸
                        var newCTN = new CMS_ProductCTN
                        {
                            HMNUM = HMNUM,
                            CTNTitle = "S/1",
                            CTNLength = CTNLength,
                            CTNWidth = CTNWidth,
                            CTNHeight = CTNHeight,
                            CTNWeight = CTNWeight,
                            CTNCube = 0,
                            CreateOn = DateTime.Now,
                            UpdateOn = DateTime.Now,
                            UpdateBy = UpdateBy
                        };
                        #endregion

                        #region 插入尺寸基础信息
                        var newDim = new CMS_ProductDimension
                        {
                            HMNUM = HMNUM,
                            DimTitle = "S/1",
                            DimLength = DimLength,
                            DimWidth = DimWidth,
                            DimHeight = DimHeight,
                            DimCube = 0,
                            CreateOn = DateTime.Now,
                            UpdateOn = DateTime.Now,
                            UpdateBy = UpdateBy
                        };
                        #endregion

                        #region 插入SKU价格信息
                        var newSKUCost = new CMS_SKU_Costing
                        {
                            CreateBy = UpdateBy,
                            CreateOn = DateTime.Now,
                            EffectiveDate = DateTime.Now,
                            SalePrice = Retail
                        };
                        #endregion

                        #region 插入SKU基础信息
                        //插入SKUOrder
                        var ChannelObj = db.Channel.FirstOrDefault(c => c.ChannelName == MERCHANTID);
                        var ChannelID = 0;
                        if (ChannelObj != null)
                        {
                            ChannelID = ChannelObj.ChannelID;
                        }


                        long ColourID = 0;
                        string skuColor = dr["Color"].ToString();
                        string skuMaterial = dr["Material"].ToString();
                        if (!string.IsNullOrEmpty(skuColor))
                        {
                            var ColorMode = db.CMS_SKU_Colour.FirstOrDefault(c => c.ColourName == skuColor);
                            if (ColorMode == null)
                            {
                                var newColour = new CMS_SKU_Colour
                                {
                                    ColourName = skuColor,
                                    CreateBy = UpdateBy,
                                    CreateOn = DateTime.Now,
                                    ModifyBy = UpdateBy,
                                    ModifyOn = DateTime.Now
                                };
                                db.CMS_SKU_Colour.Add(newColour);
                                db.SaveChanges();
                                ColourID = newColour.ColourID;
                            }
                            else
                            {
                                ColourID = ColorMode.ColourID;
                            }
                        }

                        long MaterialID = 0;
                        if (!string.IsNullOrEmpty(skuMaterial))
                        {
                            var Mode = db.CMS_SKU_Material.FirstOrDefault(m => m.MaterialName == skuMaterial);
                            if (Mode == null)
                            {
                                var newMaterial = new CMS_SKU_Material
                                {
                                    MaterialName = skuMaterial,
                                    CreateBy = UpdateBy,
                                    CreateOn = DateTime.Now,
                                    ModifyBy = UpdateBy,
                                    ModifyOn = DateTime.Now
                                };
                                db.CMS_SKU_Material.Add(newMaterial);
                                db.SaveChanges();
                               // ColourID = newMaterial.MaterialID;
                                MaterialID = newMaterial.MaterialID;//Copy害死人啊 囧 2014年3月7日17:51:03
                            }
                            else
                            {
                                MaterialID = Mode.MaterialID;
                            }
                        }



                        CMS_SKU newProudct = new CMS_SKU
                        {
                            SKU = SKUORDER,
                            //ProductName = ProductName == "" ? SKUORDER : ProductName,//Name为空则用SKUOrder代替
                            ProductName = ProductName,
                            SKU_QTY = 0,
                            ChannelID = ChannelID,
                            UPC = UPC,
                            StatusID = 4,//Compelted
                            Visibility = 1,//---报表必须为1才有效  2013年12月14日10:08:27
                            ProductDesc = PrDescription,
                            Specifications = Bullets,
                            Keywords = "",
                            BrandID = 2,//defult
                            RetailPrice = SalePrice,
                            URL = "",
                            CategoryID = 0,//暂时不整理，稍后让Melissa整理2014年1月27日10:10:57,
                            SubCategoryID = 0,//同上
                            ColourID = ColourID,
                            MaterialID = MaterialID,
                            UpdateBy = UpdateBy,
                            UpdateOn = DateTime.Now,
                            CreateBy = UpdateBy,
                            CreateOn = DateTime.Now
                        };
                        #endregion

                        #region HM-SKU关联

                        #endregion

                        //if (lineHMType == "Group")
                        if (string.Compare(lineHMType, "Group", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            groupProductID = InsertSKUAndGroupHM(db, newHMCost, newHM, newSKUCost, newProudct);//取到最新一次更新GroupHM的ID
                        }
                        // else if (lineHMType == "individual")
                        else if (string.Compare(lineHMType, "individual", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            string strUnit = dr["PCS"].ToString();
                            int unit = 0;
                            int.TryParse(strUnit, out unit);
                            InsertBasicHM(db, groupProductID, newHMCost, newHM, newCTN, newDim, unit);
                        }
                        else
                        {
                            HMLog.Error(String.Format(">>>>>>>>>>>>>.warning!! the Number of {0} Item Type can not be determined <<<<<<<<<<<<<<<<<<", iCount));
                            OnOperateNotify(this, new HMEventArgs(String.Format("Line为{0} 类型无法识别，警告！！！！！！！！！！！", iCount)));
                        }

                        HMLog.Info(String.Format(">>>>>>>>>>>>>.End the Number of {0} Item <<<<<<<<<<<<<<<<<<", iCount));
                        OnOperateNotify(this, new HMEventArgs(String.Format(">>>>>>>>>>>>>.End the Lien of {0} Item <<<<<<<<<<<<<<<<<<", iCount)));
                        OnOperateNotify(this, new HMEventArgs(String.Format("Line为{0}的数据成功插入到数据库", iCount)));
                        OnOperateNotify(this, new HMEventArgs(""));
                        HMLog.Info("");
                        // db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        OnOperateNotify(this, new HMEventArgs("Error!"));
                        OnOperateNotify(this, new HMEventArgs("出错啦！!"));
                        HMLog.Error("");
                        HMLog.Error("");
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            HMLog.Error("");
                            HMLog.Error("");
                            //HMLog.Error("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                HMLog.Error("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                                OnOperateNotify(this, new HMEventArgs(String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage)));
                            }
                            HMLog.Error("");
                        }
                        HMLog.Error("");
                        HMLog.Error("");
                        OnOperateNotify(this, new HMEventArgs(""));
                        OnOperateNotify(this, new HMEventArgs(""));

                    }
                    catch (Exception ex)
                    {
                        OnOperateNotify(this, new HMEventArgs("Error~~~~~~~~~~~~~~~"));
                        OnOperateNotify(this, new HMEventArgs("出错啦~~~~~~~~~~~~~~"));
                        OnOperateNotify(this, new HMEventArgs(ex.Message));
                        HMLog.Error("");
                        HMLog.Error("Exception Started");
                        HMLog.Error(ex.Message);
                        HMLog.Error(ex.Source);
                        HMLog.Error(ex.StackTrace);
                        HMLog.Error("Exception End");
                        HMLog.Error("");
                        OnOperateNotify(this, new HMEventArgs(""));
                        OnOperateNotify(this, new HMEventArgs(""));
                    }
                }
            }
        }


        /// <summary>
        ///  插入组合产品的以及组合产品对于的SKU 以及他们之间的关联信息到CMS中
        ///  CreateDate:2013年12月31日11:47:55
        /// </summary>
        /// <param name="db"></param>
        /// <param name="newHMCost"></param>
        /// <param name="CMS_HMNUM"></param>
        /// <param name="newSKUCost"></param>
        /// <param name="newProudct"></param>
        /// <returns>返回新插入的组合产品的ID</returns>
        public long InsertSKUAndGroupHM(PermaisuriCMSEntities db, CMS_HM_Costing newHMCost, CMS_HMNUM CMS_HMNUM,
            CMS_SKU_Costing newSKUCost, CMS_SKU newProudct)
        {
            //先判断当前的HMNUM在库表是否存在了，防止重复插入
            long ProductID = 0;
            var isExistHM = db.CMS_HMNUM.FirstOrDefault(h => h.HMNUM == CMS_HMNUM.HMNUM);
            if (isExistHM == null)
            {
                db.CMS_HM_Costing.Add(newHMCost);
                var newHMCostID = newHMCost.HMCostID;

                CMS_HMNUM.HMCostID = newHMCostID;//设置当前HMNUM的价格指向刚刚插入的价格表
                db.CMS_HMNUM.Add(CMS_HMNUM);
                db.SaveChanges();

                ProductID = CMS_HMNUM.ProductID;
                newHMCost.HisProductID = CMS_HMNUM.ProductID;

            }
            else
            {
                OnOperateNotify(this, new HMEventArgs(String.Format("CMS_HMNUM表已经存在HMNUM={0}的数据，忽略过不插入", CMS_HMNUM.HMNUM)));
                HMLog.Info(String.Format("CMS_HMNUM表已经存在HMNUM={0}的数据，忽略过不插入", CMS_HMNUM.HMNUM));
                ProductID = isExistHM.ProductID;
            }

            //插入SKU
            long SKUID = 0;
            var isExistSKU = db.CMS_SKU.FirstOrDefault(w => w.SKU == newProudct.SKU && w.ChannelID == newProudct.ChannelID);
            if (isExistSKU == null)
            {
                long newSKUCostID = 0;
                db.CMS_SKU_Costing.Add(newSKUCost);
                newSKUCostID = newSKUCost.SKUCostID;

                newProudct.SKUCostID = newSKUCostID;
                db.CMS_SKU.Add(newProudct);

                db.SaveChanges();

                newSKUCost.HisSKUID = newProudct.SKUID;
                SKUID = newProudct.SKUID;

            }
            else
            {
                OnOperateNotify(this, new HMEventArgs(String.Format("WebsiteProduct表已经存在SKU={0},Channel={1}的数据，忽略过不插入", newProudct.SKU, newProudct.ChannelID)));
                HMLog.Info(String.Format("WebsiteProduct表已经存在SKU={0},Channel={1}的数据，忽略过不插入", newProudct.SKU, newProudct.ChannelID));
                SKUID = isExistSKU.SKUID;
            }

            //插入HMNUM Group ----SKU的关系
            var newRelation = db.SKU_HM_Relation.FirstOrDefault(r => r.SKUID == SKUID && r.ProductID == ProductID);
            if (newRelation == null)
            {
                var hm_sku = new SKU_HM_Relation
                {
                    SKUID = SKUID,
                    ProductID = ProductID,
                    R_QTY = 1,
                    CreateBy = UpdateBy,
                    CreateOn = DateTime.Now
                };

                db.SKU_HM_Relation.Add(hm_sku);
            }
            else
            {
                OnOperateNotify(this, new HMEventArgs(String.Format("SKU_HM_Relation表已经存在SKUID={0},ProductID={1}的数据，忽略过不插入", SKUID, ProductID)));
                HMLog.Info(String.Format("SKU_HM_Relation表已经存在SKUID={0},ProductID={1}的数据，忽略过不插入", SKUID, ProductID));
            }
            db.SaveChanges();//为了避免出现 ProductID =0的情况
            if (ProductID == 0)
            {
                HMLog.Info(String.Format("InsertSKUAndGroupHM：警告！！！！！！！！！！！！！HMNUM={0}的ID查询出来是0！", CMS_HMNUM.HMNUM));
            }    
            return ProductID;
        }

        /// <summary>
        /// 插入基础产品的HMNUM
        /// </summary>
        /// <param name="db"></param>
        /// <param name="groupProductID">组合产品的ID</param>
        /// <param name="newHMCost"></param>
        /// <param name="newHM"></param>
        /// <param name="newCTN"></param>
        /// <param name="newDim"></param>
        /// <param name="SellSets">...</param>
        public void InsertBasicHM(PermaisuriCMSEntities db, long groupProductID, CMS_HM_Costing newHMCost, CMS_HMNUM newHM,
            CMS_ProductCTN newCTN, CMS_ProductDimension newDim,int SellSets)
        {
            //先判断是否存在这个基础HMNUM
            long basicProductID = 0;
            var isExistHM = db.CMS_HMNUM.FirstOrDefault(h=>h.HMNUM==newHM.HMNUM);
            if (isExistHM == null)
            {
                db.CMS_HM_Costing.Add(newHMCost);
                long newHMCostID = newHMCost.HMCostID;

                newHM.HMCostID = newHMCostID;
                db.CMS_HMNUM.Add(newHM);
                db.SaveChanges();
                basicProductID = newHM.ProductID;
                newHMCost.HisProductID = newHM.ProductID;

                //尺寸箱柜
                newCTN.ProductID = basicProductID;
                newDim.ProductID = basicProductID;
                db.CMS_ProductCTN.Add(newCTN);
                db.CMS_ProductDimension.Add(newDim);
            }
            else
            {
                OnOperateNotify(this, new HMEventArgs(String.Format("CMS_HMNUM(基础数据）表已经存在HMNUM={0}的数据，忽略过不插入", newHM.HMNUM)));
                HMLog.Info(String.Format("CMS_HMNUM(基础数据）表已经存在HMNUM={0}的数据，忽略过不插入", newHM.HMNUM));
                basicProductID = isExistHM.ProductID;
            }

            var newRelation = db.CMS_HMGroup_Relation.FirstOrDefault(r => r.ProductID == groupProductID && r.ChildrenProductID == basicProductID);
            if (newRelation == null)
            {
                //组合产品--基础产品--的关系表
                var newRel = new CMS_HMGroup_Relation
                {
                    ChildrenProductID = basicProductID,
                    ProductID = groupProductID,
                    SellSets = SellSets,
                    CreateBy = UpdateBy,
                    CreateOn = DateTime.Now
                };
                db.CMS_HMGroup_Relation.Add(newRel);
            }
            else
            {
                OnOperateNotify(this, new HMEventArgs(String.Format("CMS_HMGroup_Relation表已经存在ProductID={0},ChildrenProductID={1}的数据，忽略过不插入", groupProductID, basicProductID)));
                HMLog.Info(String.Format("CMS_HMGroup_Relation表已经存在ProductID={0},ChildrenProductID={1}的数据，忽略过不插入", groupProductID, basicProductID));
            }
            db.SaveChanges();
        }
    }
}
