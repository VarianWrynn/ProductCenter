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
    
    public partial class SKU_Status
    {
        public SKU_Status()
        {
            this.CMS_SKU = new HashSet<CMS_SKU>();
        }
    
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string Remark { get; set; }
    
        public virtual ICollection<CMS_SKU> CMS_SKU { get; set; }
    }
}