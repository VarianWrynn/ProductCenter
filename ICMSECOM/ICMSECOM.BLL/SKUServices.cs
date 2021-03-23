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


namespace ICMSECOM.BLL
{
    public class SKUServices
    {
        /// <summary>
        /// Ecom中的定义： 
        /// Sellset，组合产品中的定义，在Group中，用于标识子产品在组合产品中的PCS数量
        /// Sellpack，销售数量定义，表示网站订单1QTY对应系统中销售N个产品的意思 
        /// boxnum就是sellpack,一个货号我卖6个，装3箱,则：pcs 6 mp2 sellpack 3 Add Remark:2014年4月15日15:09:08
        /// </summary>
        /// <param name="WPModel"></param>
        /// <param name="db"></param>
        public void SKU_Action(CMS_SKU_Model WPModel, EcomEntities db)
        {

            if (WPModel.CMS_ShipViaType == null)//HMModel.CMS_ShipVia_Type.CMS_ShipVia_Default.SHIPVIA
            {
                throw new Exception("This SKU does not set ShipVia Type");
            }
            if (WPModel.CMS_ShipViaType.CMS_ShipVia_Default == null)
            {
                throw new Exception("This SKU does not set ShipVia");
            }

            var query = db.SKU.FirstOrDefault(s => s.SKUOrder == WPModel.SKU && s.MerchantID == WPModel.ChannelName);
            if (query == null)
            {
                var newSKU = new SKU
                {
                    MerchantID = WPModel.ChannelName,
                    HMNUM = WPModel.SKU_HM_Relation.CMS_HMNUM.HMNUM,
                    SKUOrder = WPModel.SKU,
                    SKUBest = WPModel.SKU,
                    SellPack = WPModel.SKU_HM_Relation.R_QTY / Convert.ToInt32(WPModel.SKU_HM_Relation.CMS_HMNUM.MasterPack),
                    //Description = WPModel.ProductName, eCom的Description其实是CMS的ProductName, 2014年4月9日
                    Description = WPModel.ProductName,
                    URL = WPModel.URL,
                    UPC = WPModel.UPC,
                    SHIPVIA =WPModel.CMS_ShipViaType.CMS_ShipVia_Default.SHIPVIA,
                    Status = WPModel.StatusName
                };
                db.SKU.Add(newSKU);

                db.SaveChanges();//!! 这里如果不保存，则SKUID =0 ！！！

                //新增：SKU对应的Costing表
                db.Costing.Add(new Costing
                {
                    HMNUM = newSKU.HMNUM,
                    MerchantID = newSKU.MerchantID,
                    SKUID = newSKU.SKUID,
                    SKUOrder = newSKU.SKUOrder,
                    EffectiveDate = WPModel.SKU_Costing.EffectiveDate,
                    Cost = decimal.Parse(WPModel.SKU_Costing.SalePrice, NumberStyles.Currency, new CultureInfo("en-US")),
                    Coupon = 0,//以后再做，Promo这一块 2014年4月24日11:53:47 （Boonie)
                    Retail = WPModel.RetailPrice,
                    Freight = decimal.Parse(WPModel.SKU_Costing.EstimateFreight, NumberStyles.Currency, new CultureInfo("en-US")),
                    MerchantCoupon = 0
                });
            }
            else
            {
                query.MerchantID = WPModel.ChannelName;
                query.HMNUM = WPModel.SKU_HM_Relation.CMS_HMNUM.HMNUM;
                query.SKUOrder = WPModel.SKU;
                query.SellPack = WPModel.SKU_HM_Relation.R_QTY / Convert.ToInt32(WPModel.SKU_HM_Relation.CMS_HMNUM.MasterPack);
                //query.SellPack = WPModel.SKU_HM_Relation.CMS_HMNUM.IsGroup ? 1 : WPModel.SKU_HM_Relation.R_QTY;
                //query.Description = WPModel.ProductDesc;eCom的Description其实是CMS的ProductName, 2014年4月9日
                query.Description = WPModel.ProductName;
                query.URL = WPModel.URL;
                query.UPC = WPModel.UPC;
                //query.SHIPVIA = WPModel.CMS_ShipViaType.CMS_ShipVia_Default.SHIPVIA;

                //取出当前eCom.dbo.Costing表的数据
                var eComCostings = db.Costing.Where(s => s.SKUOrder == WPModel.SKU && s.MerchantID == WPModel.ChannelName && s.EffectiveDate != null);
                if (eComCostings.FirstOrDefault() == null)
                {
                    db.Costing.Add(new Costing
                    {
                        HMNUM = query.HMNUM,
                        MerchantID = query.MerchantID,
                        SKUID = query.SKUID,
                        SKUOrder = query.SKUOrder,
                        EffectiveDate = WPModel.SKU_Costing.EffectiveDate,
                        Cost = decimal.Parse(WPModel.SKU_Costing.SalePrice, NumberStyles.Currency, new CultureInfo("en-US")),
                        Coupon = 0,//以后再做，Promo这一块 2014年4月24日11:53:47 （Boonie)
                        Retail = WPModel.RetailPrice,
                        Freight = decimal.Parse(WPModel.SKU_Costing.EstimateFreight, NumberStyles.Currency, new CultureInfo("en-US")),
                        MerchantCoupon = 0
                    });
                }
                else
                {
                    //如果已经存在并且不止一列,取出最大的那个时间点做比较
                    if (WPModel.SKU_Costing.EffectiveDate == eComCostings.Max(s => s.EffectiveDate).Value)//前面过滤了null值，所以这里不会出现null值了
                    {
                        //相等说明CMS没有做价格变动
                    }
                    else
                    {
                        //插入新的Costing
                        db.Costing.Add(new Costing
                        {
                            HMNUM = query.HMNUM,
                            MerchantID = query.MerchantID,
                            SKUID = query.SKUID,
                            SKUOrder = query.SKUOrder,
                            EffectiveDate = WPModel.SKU_Costing.EffectiveDate,
                            Cost = decimal.Parse(WPModel.SKU_Costing.SalePrice, NumberStyles.Currency, new CultureInfo("en-US")),
                            Coupon = 0,//以后再做，Promo这一块 2014年4月24日11:53:47 （Boonie)
                            Retail = WPModel.RetailPrice,
                            Freight = decimal.Parse(WPModel.SKU_Costing.EstimateFreight, NumberStyles.Currency, new CultureInfo("en-US")),
                            MerchantCoupon = 0
                        });
                    }
                }

                //db.SaveChanges();为什么要在这里Save导致不能做transaction？2014年4月24日10:41:57
            }
        }

        public void SKUURL_Action(CMS_SKU_Model WPModel, EcomEntities db)
        {
            if (String.IsNullOrEmpty(WPModel.URL))
            {
                return;
            }
            var query = db.SKUURL.FirstOrDefault(s => s.SKUOrder == WPModel.SKU && s.MerchantID == WPModel.ChannelName && s.URL == WPModel.URL);
            if (query == null)
            {
                db.SKUURL.Add(new SKUURL
                {
                    MerchantID = WPModel.ChannelName,
                    SKUOrder = WPModel.SKU,
                    URL = WPModel.URL
                });
            }
        }
    }
}
