//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PermaisuriCMS.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CMS_User_Profile_V
    {
        public CMS_User_Profile_V()
        {
            this.User_Channel_Relation = new HashSet<User_Channel_Relation>();
        }
    
        public System.Guid User_Guid { get; set; }
        public string User_Account { get; set; }
        public string UserPWD { get; set; }
        public Nullable<short> UserStatusID { get; set; }
        public string UserStatusName { get; set; }
        public string DisplayName { get; set; }
        public Nullable<System.DateTime> Last_Logon { get; set; }
        public string Primary_Email { get; set; }
        public Nullable<long> IsDisabled { get; set; }
        public Nullable<bool> IsChannelControl { get; set; }
    
        public virtual ICollection<User_Channel_Relation> User_Channel_Relation { get; set; }
    }
}