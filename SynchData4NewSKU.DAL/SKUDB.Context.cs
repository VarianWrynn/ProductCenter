﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SynchData4NewSKU.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Core.Objects.DataClasses;
    
    public partial class PermaisuriCMSNewSKUEntities : DbContext
    {
        public PermaisuriCMSNewSKUEntities()
            : base("name=PermaisuriCMSNewSKUEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    
        public virtual int SynchData_NewSKUData_SP()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SynchData_NewHM_SP");
        }
    }
}
