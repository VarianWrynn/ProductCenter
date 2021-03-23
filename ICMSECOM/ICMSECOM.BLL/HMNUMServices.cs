//*****************************************************************************************************************************************************************************************
//											Modification history
//*****************************************************************************************************************************************************************************************
// C/A/D Change No   Author     Date        Description 
//	C	WL-1		Lee		    28/05/2014	 eCom HMNUM运费 1）：编辑情况下改字段不做任何变动  2)：新增（HMNUM）情况下，运费默认赋予当前SKU的运费（如果当前SKU运维为0，则赋予1），同时新增的HMNUM进入审核队列
//  
//*****************************************************************************************************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.Model;
using ICMSECOM.DAL;
using EntityFramework;
using EntityFramework.Extensions;
using System.Globalization;
using PermaisuriCMS.Common;
using System.Configuration;
using System.IO;

namespace ICMSECOM.BLL
{
    public class HMNUMServices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="HMModel"></param>
        /// <param name="db"></param>
        /// <param name="UnitQTY">用于给eCom前端展示及套件，比如4张椅子1个座子，就是S/5，5件套</param>
        /// <param name="imagePath">源文件路径 Formate: D:\\CMS\\Files\\123.jpg</param>
        /// <param name="SKUFreight">SKU的运费，用于给eCom同步的2014年5月28日</param>
        public void HMNUM_Action(CMS_HMNUM_Model HMModel, EcomEntities db, int UnitQTY, string imagePath, decimal SKUFreight)
        {
            if (HMModel.CMS_ShipVia_Type == null)//HMModel.CMS_ShipVia_Type.CMS_ShipVia_Default.SHIPVIA
            {
                throw new Exception("This HMNUM does not set ShipVia Type");
            }
            if (HMModel.CMS_ShipVia_Type.CMS_ShipVia_Default == null)
            {
                throw new Exception("This HMNUM does not set ShipVia");
            }
            var query = db.Product.FirstOrDefault(p => p.HMNUM == HMModel.HMNUM);
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                //如果不存在，需要增加图片拷贝和略缩图生成的步骤的步骤
                var eComImageStoragePath = ConfigurationManager.AppSettings["eComImageStoragePath"];
                var strNewPath = Path.Combine(eComImageStoragePath, HMModel.HMNUM);

                //先判断Width长还是height长

                CMSImageTools.SmallImageGenerator(imagePath, strNewPath + "_320.jpg", 320);

                CMSImageTools.SmallImageGenerator(imagePath, strNewPath + "_80.jpg", 80);
            }
            if (query == null)//不存在插入Product
            {
                db.Product.Add(new Product
                {
                    HMNUM = HMModel.HMNUM,
                    StockID = 0,
                    SKUBest = HMModel.StockKey,
                    TAGNUM = string.Empty,
                    Location = string.Empty,
                    BarUPC = string.Empty,//暂时为空
                    ProductPicture = imagePath == "" ? null : HMModel.HMNUM + "_80.jpg",
                    Unit = HMModel.IsGroup == true ? "S/" + UnitQTY : "S/1",//【注意：】这里不应该拿SellSets,应该拿Pieces!
                    Colour = HMModel.HMColour == null ? null : HMModel.HMColour.ColourName,
                    Weight = Convert.ToDouble(HMModel.NetWeight),
                    MasterPack = Convert.ToInt16(HMModel.MasterPack),
                    //SellSets = HMModel.SellSets,
                    SellSets = HMModel.IsGroup == true ? 1 : HMModel.SellSets,//eComd的组合产品也有SellSets这个概念,默认设置为12014年5月6日17:59:05
                    Boxes = 1,
                    //Freight = HMModel.HM_Costing == null ? 0 : decimal.Parse(HMModel.HM_Costing.EstimateFreight, NumberStyles.Currency, new CultureInfo("en-US")),//wl -1
                    Freight = SKUFreight == 0 ? 1 : SKUFreight,//wl -1
                    CostHM = HMModel.HM_Costing == null ? 0 : decimal.Parse(HMModel.HM_Costing.FirstCost, NumberStyles.Currency, new CultureInfo("en-US")),
                    Status = "Pending",//让当前新增的HMNUM进入eCom的审核列表！
                    Comment = HMModel.Comments,
                    DescriptionHM = HMModel.ProductName,
                    SHIPVIA = HMModel.CMS_ShipVia_Type.CMS_ShipVia_Default.SHIPVIA,//2014年5月14日10:42:29
                    ColourHM = HMModel.HMColour == null ? null : HMModel.HMColour.ColourName,
                    Category = HMModel.Category == null ? null : HMModel.Category.ParentCategoryName,
                    SubCategory = HMModel.Category == null ? null : HMModel.Category.CategoryName,
                    IsGroup = HMModel.IsGroup,
                    LowStock = 10

                });
            }
            else//存在跟新Product
            {
                query.ProductPicture = imagePath == "" ? query.ProductPicture : HMModel.HMNUM + "_80.jpg";
                query.SKUBest = HMModel.StockKey;
                query.Comment = HMModel.Comments;
                query.Weight = Convert.ToDouble(HMModel.NetWeight);
                query.MasterPack = Convert.ToInt16(HMModel.MasterPack);
                query.DescriptionHM = HMModel.ProductName;
                //query.SellSets = HMModel.SellSets;
                query.SellSets = HMModel.IsGroup == true ? 1 : HMModel.SellSets;//eComd的组合产品也有SellSets这个概念
                query.Category = HMModel.Category == null ? null : HMModel.Category.ParentCategoryName;
                query.SubCategory = HMModel.Category == null ? null : HMModel.Category.CategoryName;
                query.CostHM = HMModel.HM_Costing == null ? 0 : decimal.Parse(HMModel.HM_Costing.FirstCost, NumberStyles.Currency, new CultureInfo("en-US"));
                //query.Freight = decimal.Parse(HMModel.HM_Costing.EstimateFreight, NumberStyles.Currency, new CultureInfo("en-US")); //wl-1

                //query.SHIPVIA = HMModel.CMS_ShipVia_Type.CMS_ShipVia_Default.SHIPVIA;先不同步！ 2014年6月9日17:29:22
            }
        }

        /// <summary>
        /// 组合产品的和基础产品的关联表，存在基础产品就更新，不存在就插入
        /// </summary>
        /// <param name="HMModel"></param>
        /// <param name="db"></param>
        public void HMNUMGroup_Action(CMS_SKU_Model skuModel, EcomEntities db, ref int pieces)
        {
            CMS_HMNUM_Model HMModel = skuModel.SKU_HM_Relation.CMS_HMNUM;
            pieces = 0;
            if (!HMModel.IsGroup)
            {
                return;
            }
            foreach (var subHM in HMModel.Children_CMS_HMNUM_List)
            {
                var subImgPath = string.Empty;
                var hmPImgObj = subHM.MediaList.FirstOrDefault(m=>m.IsPrimaryImages==true);
                if (hmPImgObj != null)
                {
                    subImgPath = Path.Combine(skuModel.CMSPhysicalPath, hmPImgObj.HMNUM + "\\" + hmPImgObj.ImgName + hmPImgObj.fileFormat);
                }
                this.HMNUM_Action(subHM, db, 1, subImgPath, decimal.Parse(skuModel.SKU_Costing.EstimateFreight, NumberStyles.Currency, new CultureInfo("en-US")));//调用内部方法，插入or更新HMNUM 2014年4月22日16:17:23
                pieces += subHM.SellSets;
                var query = db.ProductGroup.FirstOrDefault(g => g.HMNUM == subHM.HMNUM && g.HMNUMParent == HMModel.HMNUM);
                if (query == null)//【不】存在基础产品-组合产品的一条记录
                {
                    db.ProductGroup.Add(new ProductGroup
                    {
                        HMNUMParent = HMModel.HMNUM,
                        HMNUM = subHM.HMNUM,
                        Description = subHM.ProductName,
                        SellSets = subHM.SellSets
                    });
                }
                else//存在基础产品-组合产品的一条记录
                {
                    query.SellSets = subHM.SellSets;
                    query.Description = subHM.ProductName;
                }
            }
        }

        /// <summary>
        /// 对于箱柜尺寸的处理，就是先根据HMNUM删除所有信息，再重新插入
        /// </summary>
        /// <param name="HMModel"></param>
        /// <param name="db"></param>
        public void Carton_Action(CMS_HMNUM_Model HMModel, EcomEntities db)
        {
            var IsExistCTN = db.ProductCartons.FirstOrDefault(c => c.HMNUM == HMModel.HMNUM);//存在，则删之
            if (IsExistCTN != null)
            {
                //由于CMS整理出来的HM-ALL item的箱柜尺寸的信息不是非常的规范，所以暂时对已经存在于eCOM的这些信息不做任何处理
                //db.ProductCartons.Delete(p => p.HMNUM == HMModel.HMNUM);  
                return;
            }

            if (HMModel.IsGroup)//如果箱柜为空，则拿CMS的数据插入
            {

                foreach (var subHM in HMModel.Children_CMS_HMNUM_List)
                {
                    int ctnNum = 1;
                    foreach (var ctn in subHM.CTNList)//一个HMNUM可以对应多个HMNUM
                    {
                        db.ProductCartons.Add(new ProductCartons
                        {
                            HMNUM = HMModel.HMNUM,//这里取的不是基础产品，而是组合产品！
                            CartonNumber = ctnNum++,
                            Description = subHM.ProductName,
                            CL = Convert.ToDouble(ctn.CTNLength),
                            CW = Convert.ToDouble(ctn.CTNWidth),
                            CH = Convert.ToDouble(ctn.CTNHeight),

                            PL = Convert.ToDouble(subHM.DimList[0].DimLength),//默认取第一个，原因在Dimesion这个类的注释里面做说明了
                            PW = Convert.ToDouble(subHM.DimList[0].DimWidth),
                            PH = Convert.ToDouble(subHM.DimList[0].DimHeight),

                            //WEIGHTOFPRODUCT = 0,
                            WEIGHTOFPRODUCT = Convert.ToInt32(HMModel.NetWeight),//产品净重量
                            WEIGHTOFSHIPMENT = Convert.ToInt32(ctn.CTNWeight),//产品+箱子重量
                            COMMENTS = ctn.CTNComment,
                            SubHMNUM = subHM.HMNUM
                        });
                    }
                }
            }
            else
            {
                int ctnNum = 1;
                foreach (var ctn in HMModel.CTNList)
                {
                    db.ProductCartons.Add(new ProductCartons
                    {
                        HMNUM = HMModel.HMNUM,
                        CartonNumber = ctnNum++,
                        Description = HMModel.ProductName,
                        CL = Convert.ToDouble(ctn.CTNLength),
                        CW = Convert.ToDouble(ctn.CTNWidth),
                        CH = Convert.ToDouble(ctn.CTNHeight),

                        PL = Convert.ToDouble(HMModel.DimList[0].DimLength),//默认取第一个，原因在Dimesion这个类的注释里面做说明了
                        PW = Convert.ToDouble(HMModel.DimList[0].DimWidth),
                        PH = Convert.ToDouble(HMModel.DimList[0].DimHeight),

                        WEIGHTOFPRODUCT = Convert.ToInt32(HMModel.NetWeight),//产品净重量
                        WEIGHTOFSHIPMENT = Convert.ToInt32(ctn.CTNWeight),
                        COMMENTS = ctn.CTNComment,
                        SubHMNUM = string.Empty
                    });
                }
            }
        }
    }
}
