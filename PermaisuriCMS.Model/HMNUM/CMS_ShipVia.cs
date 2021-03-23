using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// 2014年5月7日15:47:10
    /// </summary>
    public class CMS_ShipVia_Model
    {
        public int page { get; set; }
        public int rows { get; set; }

        public int SHIPVIAID { get; set; }
        public int ShipViaTypeID { get; set; }
        public string SHIPVIA { get; set; }
        public bool IsDefaultShipVia { get; set; }

        /// <summary>
        /// -1:All, 0:true,1:false 
        /// 2014年5月12日14:59:18
        /// </summary>
        public int IsDefaultShipViaInd { get; set; }
        public string CarrierRouting { get; set; }
        public string CarrierCode { get; set; }
        public string ExpressMethod { get; set; }
        public int ExpressNumLength { get; set; }
        public string ExpressRule { get; set; }
        public System.DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
        public string UpdateOn { get; set; }
        public string UpdateBy { get; set; }

        public CMS_ShipViaType_Model CMS_ShipViaType { get; set; }
    }
}
