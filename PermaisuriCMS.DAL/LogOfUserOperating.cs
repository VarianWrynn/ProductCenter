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
    
    public partial class LogOfUserOperating
    {
        public long ID { get; set; }
        public string User_Account { get; set; }
        public string Display_Name { get; set; }
        public string Model_Name { get; set; }
        public string Action_Name { get; set; }
        public Nullable<int> Operating_Type { get; set; }
        public string OldData { get; set; }
        public string NewDate { get; set; }
        public string IP_Address { get; set; }
        public System.DateTime Operating_Date { get; set; }
    }
}
