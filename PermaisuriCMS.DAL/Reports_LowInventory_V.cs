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
    
    public partial class Reports_LowInventory_V
    {
        public string HMNUM { get; set; }
        public string SKU { get; set; }
        public string pImage { get; set; }
        public Nullable<int> StockByPcs { get; set; }
        public Nullable<int> Exp_Qty { get; set; }
        public Nullable<System.DateTime> RepDate { get; set; }
        public int ChannelID { get; set; }
        public string ChannelName { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
    }
}
