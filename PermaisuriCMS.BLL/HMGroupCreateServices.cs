using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;

namespace PermaisuriCMS.BLL
{
    public class HMGroupCreateServices
    {
        /// <summary>
        /// 获取HM产品Category的下拉单
        /// 2013年11月16日11:59:36
        /// </summary>
        /// <param name="ParentCategoryID"></param>
        /// <returns></returns>
        public List<WebPO_Category_V_Model> GetWebPO_CategoryList(long ParentCategoryID)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                List<WebPO_Category_V_Model> list = new List<WebPO_Category_V_Model>();
                var query = db.WebPO_Category_V.Where(v => v.ParentCategoryID == ParentCategoryID);
                foreach (WebPO_Category_V c in query)
                {
                    list.Add(new WebPO_Category_V_Model { 
                       CategoryID = c.CategoryID,
                       CategoryName =c.CategoryName,
                       OrderIndex =c.OrderIndex.ConvertToNotNull(),
                       ParentCategoryID = c.ParentCategoryID.ConvertToNotNull(),
                       ParentCategoryName =c.ParentCategoryName
                    });
                }
                return list;
            } 
        }

        /// <summary>
        /// 组合产品的基础信息添加，用于Create New Product Group 页面的第一阶段
        /// Change:增加一个重复HMNUM插入的判断，如果已经存在则不插入，返回错误提示。2013年11月19日11:08:15
        /// Change2:新增StockKey关联表，所以在插入之前需要新增插入StockKey表 2014年3月25日
        /// </summary>
        /// <param name="gpModel"></param>
        /// <param name="User_Account"></param>
        /// <returns></returns>
        public long HMGroupBaseInfoAdd(CMS_HMNUM_Model gpModel, String User_Account, ref string errMsg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                errMsg = string.Empty;
                var query = db.CMS_HMNUM.Where(c => c.HMNUM == gpModel.HMNUM).FirstOrDefault();
                if (query != null)
                {
                    errMsg = String.Format("this item (HMNUM={0}) has been existed!", gpModel.HMNUM);
                    return 0;
                }

                var newCosting = new CMS_HM_Costing
                {
                    CreateBy = User_Account,
                    CreateOn = DateTime.Now,
                    EffectiveDate = DateTime.Now,
                    HMNUM = gpModel.HMNUM,
                    FirstCost = 0,
                    LandedCost = 0,
                    EstimateFreight = 0,
                    HisProductID = 0
                };

                var newStockkey = new CMS_StockKey
                {
                    StockKey = gpModel.HMNUM,//stockKey拿HMNUM 2014年2月18日15:05:25
                    CreateOn = User_Account,
                    CreateTime = DateTime.Now,
                    UpdateOn = User_Account,
                    UdateTime = DateTime.Now
                };

                var newModel = new CMS_HMNUM
                {
                    HMNUM = gpModel.HMNUM,
                    ProductName = gpModel.ProductName,
                    Comments = gpModel.Comments,
                    HMCostID = gpModel.HMCostID,
                    CategoryID = gpModel.CategoryID,
                    ColourID = 0,
                    MaterialID = 0,
                    IsGroup = true,
                    StatusID = gpModel.StatusID,
                    ShipViaTypeID = gpModel.ShipViaTypeID,
                    NetWeight = gpModel.NetWeight,
                    CreateOn = DateTime.Now,
                    CreateBy = User_Account,
                    ModifyOn = DateTime.Now,
                    ModifyBy = User_Account,
                    MasterPack = 1,//组合产品默认设置为1 Lee 2014年2月12日16:13:12 2014年3月25日
                    //StockKey = gpModel.HMNUM,//stockKey拿HMNUM 2014年2月18日15:05:25
                    CMS_StockKey = newStockkey,
                    StockKey = newStockkey.StockKey,
                    CMS_HM_Costing = newCosting//2014年3月18日
                };
                db.CMS_HMNUM.Add(newModel);
                db.SaveChanges();
                newCosting.HisProductID = newModel.ProductID;
                db.SaveChanges();
                return newModel.ProductID;
            }
        }


        /// <summary>
        /// 算法：根据组合产品的ProductID和其子产品的ChildrenProductID唯一确定一条记录，删除再插入。删除插入一并做，会大大简化程序和客户端的各种判断。
        /// CreateDate: 2013年11月19日11:43:22
        /// </summary>
        /// <param name="rModel"></param>
        /// <param name="User_Account"></param>
        /// <returns>如果新增成功，返回新的ID</returns>
        public long AddNewHM4Group(CMS_HMGroup_Relation_Model rModel, String User_Account)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                //这种方法如果表结构改动，会大大增加维护成本。根据实际情况，可能组合产品的新增操作不是很频繁，并不会很大程度上影响性能，所以还是用EF来做
                //using (TransactionScope transaction = new TransactionScope())
                //{  
                //  //db.Database.ExecuteSqlCommand("delete from CMS_HMGroup_Relation where ProductID = @ProductID and ChildrenProductID", new SqlParameter("@ProductID", rModel.ProductID));
                //}
                //if (rModel.RID > 0)//说明是在原有的基础上编辑而不是新增，直接删除了吧...2013年11月20日14:42:23
                //{
                //    var relation = new CMS_HMGroup_Relation { RID = rModel.RID };
                //    db.Set<CMS_HMGroup_Relation>().Attach(relation);
                //    db.CMS_HMGroup_Relation.Remove(relation);
                //}

                //var query = db.CMS_HMGroup_Relation.Where(r => r.ProductID == rModel.ProductID && r.ChildrenProductID == rModel.ChildrenProductID).FirstOrDefault();
                //if (query != null)
                //{
                //    //db.Set<CMS_HMGroup_Relation>().Attach(query);
                //    db.CMS_HMGroup_Relation.Remove(query);
                //}

                //var rel = db.CMS_HMGroup_Relation.FirstOrDefault(r => r.RID == rModel.RID);
                /*使用RID来做判断，在2014年4月28日 下发生产环境的时候遇到很多问题，最典型的一个就是前端使用复制HMNUM，
                 * 然后选择下拉单，然后离开鼠标，客户端触发了2次RID为0的AJAX数据，造成该条数据重复插入两次。经过调试发现，ProductID是可以当成一个KEY读取，比如一个组合产品的ProductID
                 * 只会有一个，即使下一次再Create一个同样名称的组合产品，由于库表是自动增长的ID，所以名称一样没关系，ID不一样 2014年4月29日9:26:55
                 */
                var rel = db.CMS_HMGroup_Relation.FirstOrDefault(r => r.ProductID == rModel.ProductID && r.ChildrenProductID == rModel.ChildrenProductID);
                if (rel == null)
                {
                    var newModel = new CMS_HMGroup_Relation
                    {
                        ProductID = rModel.ProductID,
                        ChildrenProductID = rModel.ChildrenProductID,
                        SellSets = rModel.SellSets,
                        CreateBy = User_Account,
                        CreateOn = DateTime.Now
                    };
                    db.CMS_HMGroup_Relation.Add(newModel);
                    db.SaveChanges();
                    return newModel.RID;
                }
                else
                { 
                    rel.ProductID = rModel.ProductID;
                    rel.SellSets = rModel.SellSets;
                    rel.ChildrenProductID = rModel.ChildrenProductID;
                    db.SaveChanges();
                    return rel.RID;
                }
             
            }
        }

        /// <summary>
        /// 通过组合产品的ProductID来获取其子产品的各个价格，用于创建组合产品的时候，成功添加组合产品之后需要返回价格信息给前端展示使用
        /// CreateDate:2013年11月19日17:53:40
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<CMS_HM_Costing_Model> GetChildrenCost(CMS_HMGroup_Relation_Model model)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_HMNUM.FirstOrDefault(c => c.ProductID == model.ProductID);
                if (query == null)
                {
                    return null;
                }
                var costing = query.CMS_HMGroup_Relation.Select(r => new CMS_HM_Costing_Model
                {
                    HMNUM = r.CMS_HMNUM_Children.CMS_HM_Costing.HMNUM,
                    FirstCost = r.CMS_HMNUM_Children.CMS_HM_Costing.FirstCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    LandedCost = r.CMS_HMNUM_Children.CMS_HM_Costing.LandedCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    EstimateFreight = r.CMS_HMNUM_Children.CMS_HM_Costing.EstimateFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),

                    OceanFreight = r.CMS_HMNUM_Children.CMS_HM_Costing == null ? "$0.00" : r.CMS_HMNUM_Children.CMS_HM_Costing.OceanFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    USAHandlingCharge = r.CMS_HMNUM_Children.CMS_HM_Costing == null ? "$0.00" : r.CMS_HMNUM_Children.CMS_HM_Costing.USAHandlingCharge.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    Drayage = r.CMS_HMNUM_Children.CMS_HM_Costing == null ? "$0.00" : r.CMS_HMNUM_Children.CMS_HM_Costing.Drayage.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                    SellSets = r.SellSets
                }).ToList();
                return costing;
            }
        }

        /// <summary>
        /// 删除组合产品中的某一个子产品
        /// </summary>
        /// <param name="rModel"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool DeleteChildrenHM(CMS_HMGroup_Relation_Model rModel,ref string errMsg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {

                var query = db.CMS_HMGroup_Relation.FirstOrDefault(r => r.RID == rModel.RID);
                if (query == null)
                {
                    errMsg = "This item does not exist";
                    return  false;
                }
                db.CMS_HMGroup_Relation.Remove(query);
                return  db.SaveChanges() >0;
            }
        }

        /// <summary>
        /// 根据传递进来的ProductID,到组合产品关系表里面查询出该组合产品旗下所有的子产品的ID，
        /// 这个方法用于创建组合产品的时候，下拉单查询列表里，排除掉那些已经被选中的HMNUM，防止重复选择。
        /// CreatedDate:2014年3月18日10:57:03
        /// </summary>
        /// <param name="ParentProductID"></param>
        /// <returns></returns>
        public List<long> GetChildrenProductID(long ParentProductID)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                return db.CMS_HMGroup_Relation.Where(g => g.ProductID == ParentProductID)
                    .Select(r => r.ChildrenProductID).ToList();
            }
        }
    }
}
