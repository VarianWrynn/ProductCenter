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
    
    public partial class Costing
    {
        public int CostId { get; set; }
        public string HMNUM { get; set; }
        public string MerchantID { get; set; }
        public Nullable<int> SKUID { get; set; }
        public string SKUOrder { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> Coupon { get; set; }
        public Nullable<decimal> Retail { get; set; }
        public Nullable<decimal> Freight { get; set; }
        public Nullable<decimal> MerchantCoupon { get; set; }
        public Nullable<double> CommissionPercent { get; set; }
        public string AdjustReason { get; set; }
    }
}
