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
    
    public partial class WebPo_NewDataFromPOD_V
    {
        public long ProductID { get; set; }
        public string HMNUM { get; set; }
        public string ProductName { get; set; }
        public long MasterPack { get; set; }
        public decimal grossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public Nullable<long> ColourID { get; set; }
        public Nullable<long> MaterialID { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<long> CategoryID { get; set; }
    }
}
