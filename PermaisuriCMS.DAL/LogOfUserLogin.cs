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
    
    public partial class LogOfUserLogin
    {
        public long ID { get; set; }
        public string User_Account { get; set; }
        public string Display_Name { get; set; }
        public string Logging_IP { get; set; }
        public string Machine_Name { get; set; }
        public string Logging_Location { get; set; }
        public bool LoggingStatue { get; set; }
        public System.DateTime Logging_Date { get; set; }
        public string Remark { get; set; }
    }
}