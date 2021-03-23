using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using System.Globalization;

namespace PermaisuriCMS.BLL
{
    /// <summary>
    /// 该类作废，新的类使用SKUCreateServices.
    /// </summary>
    public class ProductCreateServices
    {
        /// <summary>
        /// 新增SKU产品
        /// CrateDate:2013年11月24日17:40:08
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Modifier"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long AddProduct(CMS_SKU_Model model, MCCC mcModel, String Modifier, out string msg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                msg = "";
                var query = db.CMS_SKU.Where(w => w.SKU == model.SKU && w.ChannelID == model.ChannelID).FirstOrDefault();
                if (query != null)
                {
                    msg = string.Format("this item is already exist");
                    return -1;
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

                db.SaveChanges();

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
                }

                var newCosting = new CMS_SKU_Costing
                {
                    SalePrice = 0,
                    EstimateFreight = 0,
                    EffectiveDate = DateTime.Now,
                    CreateBy = Modifier,
                    CreateOn = DateTime.Now
                };
                //db.CMS_SKU_Costing.Add(newCosting);



                CMS_SKU newProduct = new CMS_SKU
                {
                    //SKUCostID = 0, //default...为0 会导致后期查询的如果不做NULL排除，会得到意想不到的错误！
                    CMS_SKU_Costing = newCosting,
                    SKU = model.SKU,
                    ProductName = model.ProductName,
                    SKU_QTY = model.SKU_QTY,
                    ChannelID = model.ChannelID,
                    UPC = model.UPC,
                    StatusID = 1,//2014年3月5日 New
                    Visibility = model.Visibility,
                    ProductDesc = model.ProductDesc,
                    Specifications = model.Specifications,
                    Keywords = model.Keywords,
                    BrandID = model.BrandID,
                    RetailPrice = model.RetailPrice,
                    URL = model.URL,
                    MaterialID = mat.MaterialID,
                    ColourID = col.ColourID,
                    CategoryID = cat.CategoryID,
                    SubCategoryID = subCat.CategoryID,
                    CMS_Ecom_Sync = new CMS_Ecom_Sync
                   {
                       //SKUID = query.SKUID,坑爹啊 害死人，对象空引用！2014年4月9日
                       StatusID = 0,
                       StatusDesc = "NeedSend",
                       UpdateBy = Modifier,
                       UpdateOn = DateTime.Now
                   },

                    CreateBy = Modifier,
                    CreateOn = DateTime.Now,
                    UpdateBy = Modifier,
                    UpdateOn = DateTime.Now,

                    ShipViaTypeID = model.ShipViaTypeID
                };
                db.CMS_SKU.Add(newProduct);

                db.SaveChanges();

                newCosting.HisSKUID = newProduct.SKUID;//经测试：如果让这个生效，一定要在此之前先发生一次db.SaveChanges(),即使是使用强类型嵌套插入也需要这样save 2 次....2014年3月10日

                db.SaveChanges();
                return newProduct.SKUID;
            }
        }


        /// <summary>
        /// 算法：根据组合产品的ProductID和SKUID唯一确定一条记录，删除再插入。删除插入一并做，会大大简化程序和客户端的各种判断。
        /// CreateDate: 2013年11月19日11:43:22
        /// </summary>
        /// <param name="rModel"></param>
        /// <param name="User_Account"></param>
        /// <returns>如果新增成功，返回新的ID</returns>
        public Boolean AddNewHM4SKU(SKU_HM_Relation_Model rModel, String User_Account)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                //这种方法如果表结构改动，会大大增加维护成本。根据实际情况，可能组合产品的新增操作不是很频繁，并不会很大程度上影响性能，所以还是用EF来做
                //using (TransactionScope transaction = new TransactionScope())
                //{  
                //  //db.Database.ExecuteSqlCommand("delete from CMS_HMGroup_Relation where ProductID = @ProductID and ChildrenProductID", new SqlParameter("@ProductID", rModel.ProductID));
                //}
                var query = db.SKU_HM_Relation.FirstOrDefault(r => r.SKUID == rModel.SKUID);
                if (query == null)
                {

                    var newModel = new SKU_HM_Relation
                    {
                        ProductID = rModel.ProductID,
                        SKUID = rModel.SKUID,
                        R_QTY = rModel.R_QTY,
                        StockKeyID = rModel.StockKeyID,
                        CreateBy = User_Account,
                        CreateOn = DateTime.Now,
                        UpdateBy = User_Account,
                        UpdateOn = DateTime.Now
                    };
                    db.SKU_HM_Relation.Add(newModel);
                }
                else
                {
                    query.StockKeyID = rModel.StockKeyID;
                    query.R_QTY = rModel.R_QTY;
                    query.ProductID = rModel.ProductID;
                    query.UpdateBy = User_Account;
                    query.UpdateOn = DateTime.Now;
                }
                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 放在using里面可以减少一次数据库打开、关闭的操作，但是增加耦合度 并且如果查询价格失败也会导致加入回滚，这让我很纠结...
        /// 如果可以，应该传递 PermaisuriCMSEntities 给该方法...
        /// CreateDate:2013年11月24日19:25:07
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SKU对于的Costing永远只有一个，之所以以List形式出现，是为了以后。。。扩展 以及各个页面的方法兼容</returns>
        public List<CMS_HM_Costing_Model> GetHMCosting(SKU_HM_Relation_Model model)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_HMNUM.FirstOrDefault(c => c.ProductID == model.ProductID);
                if (query == null)
                {
                    return null;
                }
                var costing = new CMS_HM_Costing_Model
                {
                    FirstCost = query.CMS_HM_Costing.FirstCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    LandedCost = query.CMS_HM_Costing.LandedCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    EstimateFreight = query.CMS_HM_Costing.EstimateFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    OceanFreight = query.CMS_HM_Costing.OceanFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    USAHandlingCharge = query.CMS_HM_Costing.USAHandlingCharge.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    Drayage = query.CMS_HM_Costing.Drayage.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    SellSets = model.R_QTY //这里的SellSets不应该是组合产品的SellSets了..而应该是当前HM和SKU关联的R_QTY....
                };
                List<CMS_HM_Costing_Model> list = new List<CMS_HM_Costing_Model>();
                list.Add(costing);
                return list;
            }
        }
    }
}
