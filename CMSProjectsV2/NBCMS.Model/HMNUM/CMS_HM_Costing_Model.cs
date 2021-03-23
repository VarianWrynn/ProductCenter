using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model.HMNUM
{
    public class CMS_HM_Costing_Model
    {
        public long HMCostID { get; set; }
        public string HMNUM { get; set; }
        public string FirstCost { get; set; }
        public string LandedCost { get; set; }
        public string ManagementCost { get; set; }
        public string EstimateFreight { get; set; }
        public string ShippingCost { get; set; }
        public string Coupon { get; set; }
        public DateTime EffectiveDate { get; set; }
        public System.DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
        public string StockKey { get; set; }

        /// <summary>
        /// 这个字段，仅仅用于创建组合产品页面，添加子产品后，发挥当前组合产品下所有的价格的时候标记数量用的
        /// 2013年11月19日18:26:22
        /// </summary>
        public int SellSets { get; set; }
    }
}
