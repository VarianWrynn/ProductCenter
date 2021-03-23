using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model
{
    /// <summary>
    /// 渠道特殊属性对于的属性的值，比如Cosco渠道有个Color属性，这个属性的值包含了1:red,2:green,3:blue等等
    /// CreateDate:2013年12月2日14:57:55
    /// </summary>
    public class PropertyValues_Model
    {
        public int ValueID { get; set; }
        public string ValueName { get; set; }
        public int PropertyID { get; set; }
        public int OrderID { get; set; }
        public string Comments { get; set; }
        public System.DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime ModifyOn { get; set; }
        public string ModifyBy { get; set; }
    }
}
