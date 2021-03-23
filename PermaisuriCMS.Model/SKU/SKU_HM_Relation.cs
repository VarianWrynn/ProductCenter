using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    public class SKU_HM_Relation_Model
    {

        public long SKUID { get; set; }
        public long ProductID { get; set; }
        public long StockKeyID { get; set; }
        public int R_QTY { get; set; }

        public CMS_SKU_Model CMS_SKU { get; set; }
        public CMS_HMNUM_Model CMS_HMNUM { get; set; }
        public CMS_StockKey_Model CMS_StockKey { get; set; }
    }
}
