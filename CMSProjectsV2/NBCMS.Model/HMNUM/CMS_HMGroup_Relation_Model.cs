using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model.HMNUM
{
    public class CMS_HMGroup_Relation_Model
    {
        public long RID { get; set; }
        public long ProductID { get; set; }
        public long ChildrenProductID { get; set; }
        public int SellSets { get; set; }
        public System.DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
    }
}
