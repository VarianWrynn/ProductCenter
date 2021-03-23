using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// SKU类别类
    /// CreateDate:2014年1月10日18:13:49
    /// </summary>
    public class CMS_SKU_Category_Model
    {
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDesc { get; set; }
        public long ParentID { get; set; }
        public long OrderIndex { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime UpdateOn { get; set; }
        public String UpdateBy { get; set; }
        public String CreateBy { get; set; }
    }
}
