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
    
    public partial class Reports_Margin_By_Product_V
    {
        public string SKUOrder { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string HMNUM { get; set; }
        public string ProductName { get; set; }
        public Nullable<bool> IsGroup { get; set; }
        public string ChannelName { get; set; }
        public int ChannelID { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public int Inventory { get; set; }
        public decimal NormalSelling { get; set; }
        public Nullable<decimal> COG { get; set; }
        public Nullable<decimal> Margin { get; set; }
    }
}
