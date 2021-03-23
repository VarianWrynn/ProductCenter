using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// HM#的颜色信息
    /// CreateDate:2014年1月8日16:00:13
    /// </summary>
    public class CMS_HMNUM_Colour_Model
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
