using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.Common;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using System.Data;
using System.Configuration;
using System.Data.Entity.Infrastructure;

namespace PermaisuriCMS.BLL
{
    public class HMNUMServices
    {
        /// <summary>
        /// 根据各种条件查询出HMNUM的信息
        /// 2013年11月12日15:50:27 Lee
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public List<CMS_HMNUM_Model> GetHMNUMList(CMS_HMNUM_Model model, int page, int rows,out int count)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                count = 0;
                List<CMS_HMNUM_Model> list = new List<CMS_HMNUM_Model>();
                var query = db.CMS_HMNUM.Include(H => H.CMS_HM_Costing).AsQueryable();//include减少了10次查询，底层EF这增加了很多SQL语句 2013年11月20日16:46:00
                if (!String.IsNullOrEmpty(model.HMNUM))
                {
                    query = query.Where(H => H.HMNUM.StartsWith(model.HMNUM));
                }
                if (!String.IsNullOrEmpty(model.ProductName))
                {
                    query = query.Where(H => H.ProductName.Contains(model.ProductName));
                }
                if (!String.IsNullOrEmpty(model.StockKey))
                {
                    query = query.Where(H => H.StockKey.Contains(model.StockKey));
                }
                if (model.queryIsGroup > 0)
                {
                    if (model.queryIsGroup == 1)// <option value="1">HM#</option>
                    {
                        query = query.Where(H => H.IsGroup == false || H.IsGroup==null);
                    }
                    else
                    {
                        query = query.Where(H => H.IsGroup == true);
                    }
                }
                if (model.ISOrphan)//是否只查询还未和SKU关联的HM# 2013年12月2日16:14:22
                {
                    //query = query.Where(H => H.SKU_HM_Relation.Count < 1);
                    query = query.Except(query.Where(H => H.SKU_HM_Relation.Count>0));
                }
                if (model.IsExcludedSubHMNUM)
                {
                    //query = query.Except(query.Where(H => H.CMS_HMGroup_Relation.Count > 0));
                    query = query.Except(query.Where(H => H.CMS_HMGroup_Relation_SUB.Count > 0));
                }
                if (model.StatusID > 0)//-1代表All 2014年1月10日15:20:26，注意，这里假设数据库没有存在Null的数据
                {
                    query = query.Where(H => H.StatusID == model.StatusID);
                }
                if (model.RequestType > 0)
                {
                    switch (model.RequestType)
                    {
                        case 1:
                            query = query.Where(H => H.StatusID == 1);
                            break;
                        case 2:
                            query = query.Where(H => H.StatusID == 2 || H.StatusID == 3);
                            break;
                        default:
                            break;
                    }
                }
                if (model.ShipViaTypeID > 0)
                {
                    switch (model.ShipViaTypeID)
                    {
                        case 1:
                            query = query.Where(H => H.ShipViaTypeID == model.ShipViaTypeID || H.ShipViaTypeID == null);
                            break;
                        default:
                            query = query.Where(H => H.ShipViaTypeID == model.ShipViaTypeID);
                            break;
                    }
                }
                count = query.Count();
                query = query.OrderByDescending(H => H.ProductID).Skip((page - 1) * rows).Take(rows);

                foreach (var H in query)
                {
                    list.Add(new CMS_HMNUM_Model
                    {
                        ProductID = H.ProductID,
                        HMNUM = H.HMNUM,
                        ProductName = H.ProductName,
                        StockKey = H.StockKey,
                        MasterPack = H.MasterPack,
                        IsGroup = H.IsGroup.HasValue ? H.IsGroup.Value : false,
                        CMS_HMNUM_Status = new CMS_HMNUM_Status_Model
                        {
                            StatusID = H.CMS_HMNUM_Status.StatusID,
                            StatusName = H.CMS_HMNUM_Status.StatusName
                        },
                        HM_Costing = new CMS_HM_Costing_Model
                        {
                            FirstCost = H.CMS_HM_Costing.FirstCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                            LandedCost = H.CMS_HM_Costing.LandedCost.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                            EstimateFreight = H.CMS_HM_Costing.EstimateFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                            OceanFreight = H.CMS_HM_Costing.OceanFreight.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                            USAHandlingCharge = H.CMS_HM_Costing.USAHandlingCharge.ConvertToNotNull().ToString("C", new CultureInfo("en-US")),
                            Drayage = H.CMS_HM_Costing.Drayage.ConvertToNotNull().ToString("C", new CultureInfo("en-US"))
                        }
                    });
                }
                return list;
            }
        }

        /// <summary>
        /// 更新HMNUMCosting的信息，用于HMNUM Management页面的的inline-edit的编辑更新
        /// 需要注意的是每一次的跟新都将在库表新增一条价格信息，影响将来报表的生成。
        /// CreateDate:2013年11月13日6:00:34
        /// </summary>
        /// <param name="model"></param>
        /// <param name="costing"></param>
        /// <returns></returns>
        public bool EditHMNUMCosting(CMS_HMNUM_Model model, CMS_HM_Costing_Model costing, string User_Account)
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

                    HMNUM = costing.HMNUM,
                    FirstCost = Convert.ToDecimal(costing.FirstCost),
                    LandedCost = Convert.ToDecimal(costing.LandedCost),
                    EstimateFreight = Convert.ToDecimal(costing.EstimateFreight),

                    OceanFreight = Convert.ToDecimal(costing.OceanFreight),
                    USAHandlingCharge = Convert.ToDecimal(costing.USAHandlingCharge),
                    Drayage = Convert.ToDecimal(costing.Drayage),
                };
                db.CMS_HM_Costing.Add(newCosting);
                long newCostID = newCosting.HMCostID;
                var HMNUM = db.CMS_HMNUM.FirstOrDefault(h => h.ProductID == model.ProductID);
                HMNUM.HMCostID = newCostID;
                retVal = db.SaveChanges();

                newCosting.HisProductID = HMNUM.ProductID;
                retVal = db.SaveChanges();
                return retVal > 0;
            }
        }


        /// <summary>
        /// 用户在CMS页面（HMNUM Search) 手工点击同步最新的HN数据
        /// </summary>
        /// <returns></returns>
        public int SynchHMNewData()
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var timeOut = ConfigurationManager.AppSettings["HMSynchTimeOut"];
                var adapter = (IObjectContextAdapter)db;
                var objectContext = adapter.ObjectContext;
                int duration = 3;//默认三分钟
                if (!Int32.TryParse(timeOut, out duration))
                {
                    duration = 3;
                };
                objectContext.CommandTimeout = duration * 60; // value in seconds

                int retVal = db.SynchData_NewHM_SP();
                return retVal;
            }
        }

        /// <summary>
        /// 通过ID获取当个HMNUM的信息，方法只要包含了当前HMNUM在WEBPO对应的图像的信息。主要给AddMedia页面调用。
        /// Created:2014年1月24日15:30:55.
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public CMS_HMNUM_Model GetSingleHMNUMByID(long ProductID)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var p = db.CMS_HMNUM.FirstOrDefault(c => c.ProductID == ProductID);
                if (p == null)
                {
                    return null;
                }
                var img = db.WebPO_ImageUrls_V.FirstOrDefault(v => v.HMNUM == p.HMNUM && !String.IsNullOrEmpty(v.SmallPic) && !String.IsNullOrEmpty(v.Pic));
                String webPoUrl = ConfigurationManager.AppSettings["WebPOProductImageUrl"];
                String webpoRelStr = "../../../";//替换掉webPO数据库提取出来的路径前缀
                var newHMNUM = new CMS_HMNUM_Model
                    {
                        ProductID = p.ProductID,
                        HMNUM = p.HMNUM,
                        ProductName = p.ProductName,
                        StockKey = p.StockKey,
                        StockKeyQTY = p.CMS_HM_Inventory.StockkeyQTY.ConvertToNotNull(),// 库存 2013年11月24日18:17:06
                        MaxImaSeq = p.CMS_StockKey.MediaLibrary.FirstOrDefault() == null ? 0 : p.CMS_StockKey.MediaLibrary.OrderByDescending(r => r.SerialNum).FirstOrDefault().SerialNum,
                        webSystemImage = img == null ? null : new OtherSystemImages
                        {
                            SmallPic = webPoUrl + img.SmallPic.Replace(webpoRelStr, ""),
                            Pic = webPoUrl + img.Pic.Replace(webpoRelStr, ""),
                            SystemName = "WebPO System"
                        }
                    };
                return newHMNUM;
            }
        }

    }
}
