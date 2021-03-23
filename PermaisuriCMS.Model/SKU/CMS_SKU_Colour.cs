using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// HM#的颜色信息
    /// CreateDate:2014年1月10日18:13:49
    /// </summary>
    public class CMS_SKU_Colour_Model
    {
        public long ColourID { get; set; }
        public string ColourName { get; set; }
        public string ColourDesc { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateOn { get; set; }
        public System.DateTime ModifyOn { get; set; }
        public string ModifyBy { get; set; }
    }
}
