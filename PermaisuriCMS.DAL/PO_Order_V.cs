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
    
    public partial class PO_Order_V
    {
        public Nullable<int> Orders { get; set; }
        public Nullable<decimal> Prices { get; set; }
        public int OrderYear { get; set; }
        public int OrderMonth { get; set; }
        public string MerchantID { get; set; }
        public int ChannelID { get; set; }
    }
}