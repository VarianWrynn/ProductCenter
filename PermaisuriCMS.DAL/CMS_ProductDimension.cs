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
    
    public partial class CMS_ProductDimension
    {
        public long DimID { get; set; }
        public long ProductID { get; set; }
        public string HMNUM { get; set; }
        public string DimTitle { get; set; }
        public Nullable<decimal> DimLength { get; set; }
        public Nullable<decimal> DimWidth { get; set; }
        public Nullable<decimal> DimHeight { get; set; }
        public Nullable<decimal> DimCube { get; set; }
        public string DimComment { get; set; }
        public Nullable<System.DateTime> CreateOn { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string UpdateBy { get; set; }
    
        public virtual CMS_HMNUM CMS_HMNUM { get; set; }
    }
}
