using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model.SKU
{
    /// <summary>
    /// SKU关联的价格，每次改动该价格都需要新增一条记录到DB
    /// CreateDate:2013年11月22日18:12:55
    /// </summary>
    public class CMS_SKU_Costing_Model
    {
        public long SKUCostID { get; set; }
        public string RetailPrice { get; set; }
    }
}
