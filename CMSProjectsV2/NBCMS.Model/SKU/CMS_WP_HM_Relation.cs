using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.Model.HMNUM;

namespace NBCMS.Model.SKU
{
    public class SKU_HM_Relation_Model
    {
        public long RID { get; set; }
        public long SKUID { get; set; }
        public long ProductID { get; set; }
        public int R_QTY { get; set; }

        public SKU_Model SKU { get; set; }
        public CMS_HMNUM_Model CMS_HMNUM { get; set; }
    }
}
