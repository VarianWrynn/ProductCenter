using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    public class CMS_HM_Costing_Model
    {
        public long HMCostID { get; set; }
        public string HMNUM { get; set; }

        /// <summary>
        /// 出厂成本价：WebPO的产品单价 – 取自WebPO  First Cost
        /// </summary>
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

        /// <summary>
        /// 海运费：产品通过海运到美国港口的参考运费 – 手动录入Ocean freight 2014年4月24日
        /// </summary>
        public string OceanFreight { get; set; }

        /// <summary>
        /// 手续费：产品从出厂到美国仓库期间所需要的参考手续费 – 手动录入USA Handling Charge
        /// </summary>
        public string USAHandlingCharge { get; set; }

        /// <summary>
        /// 美国本地陆运费：产品从港口运到美国仓库的参考运费 – 手动录入Drayage
        /// </summary>
        public string Drayage { get; set; }
    }
}
