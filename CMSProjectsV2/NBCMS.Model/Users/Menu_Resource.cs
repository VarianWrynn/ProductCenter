using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model.Users
{
    /// <summary>
    /// Menu_Resource:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Menu_Resource_Model
    {
        public int MR_ID { get; set; }
        public string MenuID { get; set; }
        public string ParentMenuID { get; set; }
        public string MenuName { get; set; }
        public int SortNo { get; set; }
        public string MenuUrl { get; set; }
        public string Icon { get; set; }
        public bool Visible { get; set; }
        public string Memo { get; set; }
        public string Created_By { get; set; }
        public System.DateTime Created_On { get; set; }
        public string Modified_By { get; set; }
        public Nullable<System.DateTime> Modified_On { get; set; }


        //以下字段用于前端逻辑使用，与DB结构无关
        public int page { get; set; }
        public int rows { get; set; }
        public String iconSkin { get; set; }
        public bool Role_Checked { get; set; }
        /// <summary>
        /// z-tree这个插件无法灵活配置名称字段，必须交name，而且必须小写
        /// </summary>
        public string name { get; set; }
    }
}
