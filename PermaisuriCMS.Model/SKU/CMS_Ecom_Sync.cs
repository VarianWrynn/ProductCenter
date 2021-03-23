using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// ...
    /// CreateDate:2014年3月28日
    /// </summary>
    public class CMS_Ecom_Sync_Model
    {
        public long SKUID { get; set; }
        public int StatusID { get; set; }
        public string StatusDesc { get; set; }
        public DateTime UpdateOn { get; set; }    
        public string UpdateBy { get; set; }
        public string Comments { get; set; }

        public CMS_SKU_Model CMS_SKU { get; set; }

        public string strUpdateOn { get; set; }
    }
}
