using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// 渠道对于的特殊属性
    /// </summary>
    public class ChannelProperty_Model
    {
        public int PropertyID { get; set; }
        public string PropertyName { get; set; }
        public int ChannelID { get; set; }
        public int OrderID { get; set; }
        public string Comments { get; set; }
        public System.DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime ModifyOn { get; set; }
        public string ModifyBy { get; set; }
        public int SelectedValue { get; set; }
        public virtual List<PropertyValues_Model> PropertyValues { get; set; }
    }
}
