using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// 运输类型，指示当前的SKU或者HMNUM的运输类型为物流或者快递 注意关系表是nullable! 2014年5月7日15:47:35。
    /// </summary>
    public class CMS_ShipViaType_Model
    {
        public int ShipViaTypeID { get; set; }
        public string ShipViaTypeName { get; set; }

        public List<CMS_ShipVia_Model> CMS_ShipVia { get; set; }

        /// <summary>
        /// ShiViaType大类型下的默认的一个子类型，该类型用于给eCom同步使用。 2014年5月12日16:57:27
        /// </summary>
        public CMS_ShipVia_Model CMS_ShipVia_Default { get; set; }
    }
}
