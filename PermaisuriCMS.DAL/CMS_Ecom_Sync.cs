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
    
    public partial class CMS_Ecom_Sync
    {
        public CMS_Ecom_Sync()
        {
            this.Export2Excel = new HashSet<Export2Excel>();
        }
    
        public long SKUID { get; set; }
        public int StatusID { get; set; }
        public string StatusDesc { get; set; }
        public System.DateTime UpdateOn { get; set; }
        public string UpdateBy { get; set; }
        public string Comments { get; set; }
    
        public virtual CMS_SKU CMS_SKU { get; set; }
        public virtual ICollection<Export2Excel> Export2Excel { get; set; }
    }
}
