using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// 用户模型
    /// </summary>
    [Serializable]
    public partial class User_Profile_Model
    {
        public System.Guid User_Guid { get; set; }
        public string User_Account { get; set; }
        public string User_Pwd { get; set; }
        public short UserStatusID { get; set; }
        public string UserStatusName { get; set; }
        public DateTime? Last_Logon { get; set; }
        public string Display_Name { get; set; }
        public string Primary_Email { get; set; }
        public string Mobile_Phone { get; set; }
        public string Created_By { get; set; }
        public System.DateTime Created_On { get; set; }
        public string Modified_By { get; set; }
        public DateTime? Modified_On { get; set; }
        public bool IsChannelControl { get; set; }
    }
}
