using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// 2014年3月21日
    /// </summary>
    public class CMS_StockKey_Model
    {
        public long StockKeyID { get; set; }
        public string StockKey { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateOn { get; set; }
        public System.DateTime UdateTime { get; set; }
        public string UpdateOn { get; set; }
        public string Remark { get; set; }

        public List<CMS_HMNUM_Model> CMS_HMNUM { get; set; }
        public List<MediaLibrary_Model> MediaLibrary { get; set; }
        public List<SKU_HM_Relation_Model> SKU_HM_Relation { get; set; }
    }
}
