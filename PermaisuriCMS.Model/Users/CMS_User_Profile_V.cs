using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// Lee, 2013年10月22日16:39:03
    /// </summary>
    [Serializable]
    public class CMS_User_Profile_V_Model
    {
        public System.Guid User_Guid { get; set; }
        public string User_Account { get; set; }
        public string UserPWD { get; set; }
        public int UserStatusID { get; set; }
        public string UserStatusName { get; set; }
        public string Display_Name { get; set; }
        public string Last_Logon { get; set; }
        public string Primary_Email { get; set; }
        public long IsDisabled { get; set; }
        public bool IsChannelControl { get; set; }
        /// <summary>
        /// 用来获取当前用户和哪些渠道关联，展示报表所用
        /// Author:Lee ,Date:2013年11月1日15:00:14
        /// </summary>
        public List<Channel_Model> Channels { get; set; }


    }

    /// <summary>
    /// 查询模式，初始化前端和后台之间的查询模型
    /// Lee,2013年10月22日16:38:51
    /// </summary>
    public class User_Profile_QueryModel
    {
        public int page { get; set; }
        public int rows { get; set; }
        public string User_Account { get; set; }
        public string UserPWD { get; set; }
        public int UserStatusID { get; set; }
        public string Display_Name { get; set; }
        public string Primary_Email { get; set; }
        public long IsDisabled { get; set; }
    }
    
}
