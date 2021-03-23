using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model.Users
{
    /// <summary>
    /// Security_Role:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Security_Role_Model
    {
        public System.Guid Role_GUID { get; set; }
        public string Role_Name { get; set; }
        public string Role_Desc { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_On { get; set; }
        public string Modified_By { get; set; }
        public DateTime Modified_On { get; set; }

        /// <summary>
        /// 指示器：指示当前用户是否拥有/选择了该权限，用于用户权限的判断
        /// </summary>
        public bool User_Checked { get; set; }

        public int page { get; set; }
        public int rows { get; set; }
    }
}
