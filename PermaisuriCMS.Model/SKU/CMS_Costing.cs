using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// SKU关联的价格，每次改动该价格都需要新增一条记录到DB
    /// CreateDate:2013年11月22日18:12:55
    /// </summary>
    public class CMS_SKU_Costing_Model
    {
        public long SKUCostID { get; set; }
        //public string RetailPrice { get; set; }
        public string SalePrice { get; set; } //2014年3月10日

        /// <summary>
        /// 新需求，SKUOrder须有自己的价格属性 2014年4月23日
        /// </summary>
        public string EstimateFreight { get; set; }

        /// <summary>
        /// 同步eCom.dbo.Costing表的是的Key值 2014年4月24日
        /// </summary>
        public DateTime EffectiveDate { get; set; }
    }
}
