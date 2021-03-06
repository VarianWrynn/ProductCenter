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
    
    public partial class CMS_SKU
    {
        public CMS_SKU()
        {
            this.SKU_Media_Relation = new HashSet<SKU_Media_Relation>();
        }
    
        public long SKUID { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public int ChannelID { get; set; }
        public int BrandID { get; set; }
        public int SKU_QTY { get; set; }
        public string UPC { get; set; }
        public int StatusID { get; set; }
        public Nullable<int> Visibility { get; set; }
        public string ProductDesc { get; set; }
        public string Specifications { get; set; }
        public string Keywords { get; set; }
        public decimal RetailPrice { get; set; }
        public string URL { get; set; }
        public long SKUCostID { get; set; }
        public long ColourID { get; set; }
        public long MaterialID { get; set; }
        public long CategoryID { get; set; }
        public long SubCategoryID { get; set; }
        public Nullable<System.DateTime> CreateOn { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<int> ShipViaTypeID { get; set; }
    
        public virtual Brand Brand { get; set; }
        public virtual Channel Channel { get; set; }
        public virtual CMS_SKU_Costing CMS_SKU_Costing { get; set; }
        public virtual SKU_Status SKU_Status { get; set; }
        public virtual ICollection<SKU_Media_Relation> SKU_Media_Relation { get; set; }
        public virtual CMS_SKU_Colour CMS_SKU_Colour { get; set; }
        public virtual CMS_SKU_Material CMS_SKU_Material { get; set; }
        public virtual CMS_SKU_Category CMS_SKU_Category { get; set; }
        public virtual CMS_SKU_Category CMS_SKU_Category_Sub { get; set; }
        public virtual SKU_HM_Relation SKU_HM_Relation { get; set; }
        public virtual CMS_Ecom_Sync CMS_Ecom_Sync { get; set; }
        public virtual CMS_ShipViaType CMS_ShipViaType { get; set; }
    }
}
