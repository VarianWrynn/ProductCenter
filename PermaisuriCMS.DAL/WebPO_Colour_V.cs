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
    
    public partial class WebPO_Colour_V
    {
        public WebPO_Colour_V()
        {
            this.CMS_HMNUM = new HashSet<CMS_HMNUM>();
        }
    
        public long ColourID { get; set; }
        public string ColourDesc { get; set; }
    
        public virtual ICollection<CMS_HMNUM> CMS_HMNUM { get; set; }
    }
}
