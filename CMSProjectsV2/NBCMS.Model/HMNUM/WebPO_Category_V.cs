using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model.HMNUM
{
    public class WebPO_Category_V_Model
    {
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }
        public long ParentCategoryID { get; set; }
        public string ParentCategoryName { get; set; }
        public long OrderIndex { get; set; }
    }
}
