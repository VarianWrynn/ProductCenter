//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ICMSECOM.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class SKU
    {
        public int SKUID { get; set; }
        public string MerchantID { get; set; }
        public string HMNUM { get; set; }
        public string SKUOrder { get; set; }
        public string SKUBest { get; set; }
        public string SKURef { get; set; }
        public Nullable<int> SellPack { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DateOnline { get; set; }
        public string URL { get; set; }
        public Nullable<int> Stock { get; set; }
        public Nullable<System.DateTime> GetLifeCycleDate { get; set; }
        public string UPC { get; set; }
        public string SHIPVIA { get; set; }
        public string Status { get; set; }
    }
}
