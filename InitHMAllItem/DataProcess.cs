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


namespace InitHMAllItem
{
    /// <summary>
    /// 操作信息事件代理
    /// </summary>
    public delegate void OperateNotifyHandler(object sender, HMEventArgs e);

    public class DataProcess
    {
        /// <summary>
        /// 操作信息事件
        /// </summary>
        public event OperateNotifyHandler OperateNotify;

        private static Logger HMLog = LogManager.GetCurrentClassLogger(); 

        public static string path = string.Empty;
        //public static string connStr = string.Empty;
        public static string UpdateBy = "HM-ALL ITEM";

        public static string connStrHM = string.Empty;
        public static string connStrHMGroup = string.Empty;

        protected virtual void OnOperateNotify(object sender, HMEventArgs e)
        {
            if (OperateNotify != null)

                OperateNotify(sender, e);
        }


        public DataProcess(string strPath)
        {
            path = strPath;
            connStrHM = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;";
        }

        public DataProcess(string hmPath,string hmGroupPath)
        {
            connStrHM = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + hmPath + ";Extended Properties=Excel 12.0;";
            connStrHMGroup = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + hmGroupPath + ";Extended Properties=Excel 12.0;";
        }




        private DataSet loadingHMDataFromExcel()
        {
            //构建连接字符串
            OleDbConnection Conn = new OleDbConnection(connStrHM);
            Conn.Open();
            //填充数据
            string sql = string.Format("select * from [{0}$]", "simple");
            OleDbDataAdapter da = new OleDbDataAdapter(sql, connStrHM);
            DataSet ds = new DataSet();
            da.Fill(ds);
            Conn.Close();
            return ds;
        }


        private DataSet loadingHMGroupDataFromExcel()
        {
            //构建连接字符串
            OleDbConnection Conn = new OleDbConnection(connStrHMGroup);
            Conn.Open();
            //填充数据
            string sql = string.Format("select * from [{0}$]", "组合产品");
            OleDbDataAdapter da = new OleDbDataAdapter(sql, connStrHMGroup);
            DataSet ds = new DataSet();
            da.Fill(ds);
            Conn.Close();
            return ds;
        }


        //public void StartProcess()
        public void StartProcess()
        {
            DataSet ds = loadingHMDataFromExcel();
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        #region 提出Excel数据
                        string line = dr["line"].ToString();
                        OnOperateNotify(this, new HMEventArgs(String.Format(">>>>>>>>>>>>>.Start the Lien of {0} Item <<<<<<<<<<<<<<<<<<,,,", line)));
                        HMLog.Info(String.Format(">>>>>>>>>>>>>.Start the Number of {0} Item <<<<<<<<<<<<<<<<<<,,,", line));
                        string stockKey = dr["STOCK KEY-sales data"].ToString();
                        string HMNUM = dr["HMNUM-New"].ToString();
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


                        string strLoadability = dr["Loadability-webpo"].ToString();
                        decimal Loadability = 0;
                        decimal.TryParse(strLoadability, out Loadability);

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

                        string SKUORDER = dr["SKUORDER"].ToString();
                        string MERCHANTID = dr["MERCHANTID"].ToString().Trim();
                        string PrDescription = dr["Intro Sentence(s)"].ToString();
                        string Bullets = dr["Bullet Description"].ToString();
                        string UPC = String.Empty;
                        string BESTUPC = dr["BEST UPC"].ToString();
                        string CAUPC = dr["COSTCO/AMAZON UPC"].ToString();
                        string GUPC = dr["GROUPON UPC"].ToString();
                        string GDFUPC = dr["GDF UPC"].ToString();
                        if (!String.IsNullOrEmpty(BESTUPC))
                        {
                            UPC = BESTUPC;
                        }
                        else if (!String.IsNullOrEmpty(CAUPC))
                        {
                            UPC = CAUPC;
                        }
                        else if (!String.IsNullOrEmpty(GUPC))
                        {
                            UPC = GUPC;
                        }
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

                        #endregion


                        //根据HMNUM判断表内是否存在这条记录，不存在则插入对于的价格、箱柜尺寸、信息，否则不插入，直接插入SKU和HM-SKU关联
                        long ProductID = 0;
                        var curHM = db.CMS_HMNUM.FirstOrDefault(c => c.HMNUM == HMNUM);
                        if (curHM == null)
                        {
                            #region 插入HM的价格信息
                            var newHMCost = new CMS_HM_Costing
                            {
                                CreateBy = UpdateBy,
                                CreateOn = DateTime.Now,
                                EffectiveDate = DateTime.Now,
                                EstimateFreight = ShippingCost,
                                LandedCost = landedCost,
                                FirstCost = firstCost,
                                HMNUM = HMNUM,
                                HisProductID =0,
                            };
                            db.CMS_HM_Costing.Add(newHMCost);
                            #endregion


                            var HMCostID = newHMCost.HMCostID;

                            long HMCategoryID = 0; long HMColourID = 0; long HMMaterialID = 0;
                            string HMName = ProductName;
                            //long HMColorID = 0;从这张表里面获取 类别 颜色 和材料ID
                            var temObj =  db.WebPO_HM_Colour_Material_V.FirstOrDefault(v=>v.HMNUM==HMNUM);
                            if (temObj != null)
                            { 
                                HMCategoryID = temObj.CategoryID.ConvertToNotNull();
                                HMColourID = temObj.ColourID;
                                HMMaterialID = temObj.MaterialID;
                                HMName = temObj.ProductName;
                            }

                            #region 插入HM的基础信息
                            var newHM = new CMS_HMNUM
                            {
                                HMNUM = HMNUM,
                                // ProductName = ProductName,基础产品的Name应该要从WEBPO拿 而不是从Excel表单拿
                                ProductName = HMName,
                                StockKey = stockKey,
                                HMCostID = HMCostID,
                                CategoryID = HMCategoryID,
                                MaterialID = HMMaterialID,
                                ColourID = HMColourID,
                                SubCategoryID = 0,
                                IsGroup = false,
                                StatusID = 0,
                                Loadability = Loadability,
                                CreateOn = DateTime.Now,
                                CreateBy = UpdateBy,
                                ModifyOn = DateTime.Now,
                                ModifyBy = UpdateBy,
                                MasterPack =1//暂时设置为1，后面再手动跟新
                            };
                            db.CMS_HMNUM.Add(newHM);

                            db.SaveChanges();//顺序不能掉，否则 ProductID = newHM.ProductID; 取出来的ID还是0；
                            ProductID = newHM.ProductID;

                            newHMCost.HisProductID = ProductID;
                          
                            #endregion


                            #region 插入箱子基础信息
                            //插入尺寸
                            var newCTN = new CMS_ProductCTN
                            {
                                ProductID = ProductID,
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
                            db.CMS_ProductCTN.Add(newCTN);
                            #endregion

                            #region 插入尺寸基础信息
                            var newDim = new CMS_ProductDimension
                            {
                                ProductID = ProductID,
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
                            db.CMS_ProductDimension.Add(newDim);
                            #endregion
                        }
                        else
                        {
                            ProductID = curHM.ProductID;
                        }

                        db.SaveChanges();

                        #region 插入SKU价格信息
                        var newSKUCost = new CMS_SKU_Costing
                        {
                            CreateBy = UpdateBy,
                            CreateOn = DateTime.Now,
                            EffectiveDate = DateTime.Now,
                            SalePrice = Retail
                        };
                        db.CMS_SKU_Costing.Add(newSKUCost);
                        db.SaveChanges();
                        var SKUCostID = newSKUCost.SKUCostID;
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
                                MaterialID = newMaterial.MaterialID;
                            }
                            else
                            {
                                MaterialID = Mode.MaterialID;
                            }
                        }


                        CMS_SKU newProudct = new CMS_SKU
                        {
                            SKU = SKUORDER,
                            ProductName = ProductName == "" ? SKUORDER : ProductName,//Name为空则用SKUOrder代替
                            SKU_QTY = 0,
                            //Price = 0,
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
                            SKUCostID = SKUCostID,
                            CategoryID = 0,
                            ColourID = ColourID,   
                            MaterialID = MaterialID,
                            SubCategoryID= 0,
                             CreateBy = UpdateBy,
                            CreateOn= DateTime.Now,
                            UpdateBy= UpdateBy,
                            UpdateOn = DateTime.Now
                        };
                        db.CMS_SKU.Add(newProudct);

                        db.SaveChanges();
                        var SKUID = newProudct.SKUID;

                        newSKUCost.HisSKUID = SKUID;
                        #endregion


                        #region HM-SKU关联
                        //HM-SKU关联
                        var newRelation = new SKU_HM_Relation
                        {
                            CreateBy = UpdateBy,
                            CreateOn = DateTime.Now,
                            ProductID = ProductID,
                            R_QTY = 1,
                            SKUID = SKUID
                        };
                        db.SKU_HM_Relation.Add(newRelation);
                        #endregion
                        db.SaveChanges();

                        HMLog.Info(String.Format(">>>>>>>>>>>>>.End the Number of {0} Item <<<<<<<<<<<<<<<<<<", line));
                        OnOperateNotify(this, new HMEventArgs(String.Format(">>>>>>>>>>>>>.End the Lien of {0} Item <<<<<<<<<<<<<<<<<<,,,", line)));
                        OnOperateNotify(this, new HMEventArgs(String.Format("Line为{0}的数据成功插入到数据库", line)));
                        OnOperateNotify(this, new HMEventArgs(""));
                        OnOperateNotify(this, new HMEventArgs(""));
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
            }// end of  using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())

            if (ds != null) //release memory
            {
                ds.Dispose();
                ds = null;
            }

        }//end of StartProcess();





        /// <summary>
        /// 获取颜色列表信息，提交给Larfier筛选
        /// </summary>
        public void GetColourList()
        {
            try
            {
                OnOperateNotify(this, new HMEventArgs(String.Format("Started Exported...")));

                Logger ColourLog = LogManager.GetLogger("ColourLog");
                DataSet ds = loadingHMDataFromExcel();
                List<string> colorList = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string Color = dr["Color"].ToString().Trim();
                    if (Color != "" && !colorList.Contains(Color))//不为空，并且不在这个List里面，则插入
                    {
                        colorList.Add(Color);
                    }
                }
                DataSet ds2 = loadingHMGroupDataFromExcel();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string Color = dr["Color"].ToString().Trim();
                    if (Color != "" && !colorList.Contains(Color))//不为空，并且不在这个List里面，则插入
                    {
                        colorList.Add(Color);
                    }
                }

                foreach (string cname in colorList)
                {
                    ColourLog.Error(cname);
                }

                OnOperateNotify(this, new HMEventArgs(String.Format("Done")));

                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
            }
            catch (Exception ex)
            {
                HMLog.Fatal(ex.Message);
                HMLog.Fatal(ex.StackTrace);
            }
        }



        /// <summary>
        /// 获取颜色列表信息，提交给Larfier筛选
        /// </summary>
        public void GetMaterialList()
        {
            try
            {
                OnOperateNotify(this, new HMEventArgs(String.Format("Started Exported Material...")));

                Logger MaterialLog = LogManager.GetLogger("MaterialLog");
                DataSet ds = loadingHMDataFromExcel();
                List<string> colorList = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string Color = dr["Material"].ToString().Trim();
                    if (Color != "" && !colorList.Contains(Color))//不为空，并且不在这个List里面，则插入
                    {
                        colorList.Add(Color);
                    }
                }
                DataSet ds2 = loadingHMGroupDataFromExcel();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string Color = dr["Material"].ToString().Trim();
                    if (Color != "" && !colorList.Contains(Color))//不为空，并且不在这个List里面，则插入
                    {
                        colorList.Add(Color);
                    }
                }

                foreach (string cname in colorList)
                {
                    MaterialLog.Error(cname);
                }

                OnOperateNotify(this, new HMEventArgs(String.Format("Done--Material")));

                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
            }
            catch (Exception ex)
            {
                HMLog.Fatal(ex.Message);
                HMLog.Fatal(ex.StackTrace);
            }
        }


    }
}
