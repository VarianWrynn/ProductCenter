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
    
    public partial class CMS_HM_Costing
    {
        public CMS_HM_Costing()
        {
            this.CMS_HMNUM = new HashSet<CMS_HMNUM>();
        }
    
        public long HMCostID { get; set; }
        public string StockKey { get; set; }
        public string HMNUM { get; set; }
        public Nullable<decimal> FirstCost { get; set; }
        public Nullable<decimal> LandedCost { get; set; }
        public Nullable<decimal> ManagementCost { get; set; }
        public Nullable<decimal> EstimateFreight { get; set; }
        public Nullable<decimal> ShippingCost { get; set; }
        public Nullable<decimal> Coupon { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public System.DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
        public long HisProductID { get; set; }
        public Nullable<decimal> OceanFreight { get; set; }
        public Nullable<decimal> USAHandlingCharge { get; set; }
        public Nullable<decimal> Drayage { get; set; }
    
        public virtual ICollection<CMS_HMNUM> CMS_HMNUM { get; set; }
    }
}
