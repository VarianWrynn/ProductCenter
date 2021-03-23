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
    
    public partial class CMS_HMNUM
    {
        public CMS_HMNUM()
        {
            this.CMS_HMGroup_Relation = new HashSet<CMS_HMGroup_Relation>();
            this.CMS_HMGroup_Relation_SUB = new HashSet<CMS_HMGroup_Relation>();
            this.CMS_ProductCTN = new HashSet<CMS_ProductCTN>();
            this.CMS_ProductDimension = new HashSet<CMS_ProductDimension>();
            this.SKU_HM_Relation = new HashSet<SKU_HM_Relation>();
            this.Export2Excel = new HashSet<Export2Excel>();
        }
    
        public long ProductID { get; set; }
        public string HMNUM { get; set; }
        public string ProductName { get; set; }
        public long StockKeyID { get; set; }
        public string StockKey { get; set; }
        public long MasterPack { get; set; }
        public string Comments { get; set; }
        public long HMCostID { get; set; }
        public long CategoryID { get; set; }
        public Nullable<int> SubCategoryID { get; set; }
        public Nullable<bool> IsGroup { get; set; }
        public int StatusID { get; set; }
        public Nullable<decimal> Loadability { get; set; }
        public long ColourID { get; set; }
        public long MaterialID { get; set; }
        public System.DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime ModifyOn { get; set; }
        public string ModifyBy { get; set; }
        public string WebPOColourName { get; set; }
        public string WebPOMaterialName { get; set; }
        public Nullable<decimal> NetWeight { get; set; }
        public Nullable<bool> IsCloud { get; set; }
        public Nullable<int> ShipViaTypeID { get; set; }
    
        public virtual CMS_HM_Costing CMS_HM_Costing { get; set; }
        public virtual ICollection<CMS_HMGroup_Relation> CMS_HMGroup_Relation { get; set; }
        public virtual ICollection<CMS_HMGroup_Relation> CMS_HMGroup_Relation_SUB { get; set; }
        public virtual ICollection<CMS_ProductCTN> CMS_ProductCTN { get; set; }
        public virtual ICollection<CMS_ProductDimension> CMS_ProductDimension { get; set; }
        public virtual WebPO_Category_V WebPO_Category_V { get; set; }
        public virtual WebPO_Colour_V WebPO_Colour_V { get; set; }
        public virtual WebPO_Material_V WebPO_Material_V { get; set; }
        public virtual CMS_HM_Inventory CMS_HM_Inventory { get; set; }
        public virtual ICollection<SKU_HM_Relation> SKU_HM_Relation { get; set; }
        public virtual CMS_StockKey CMS_StockKey { get; set; }
        public virtual ICollection<Export2Excel> Export2Excel { get; set; }
        public virtual CMS_HMNUM_Status CMS_HMNUM_Status { get; set; }
        public virtual CMS_ShipViaType CMS_ShipViaType { get; set; }
    }
}