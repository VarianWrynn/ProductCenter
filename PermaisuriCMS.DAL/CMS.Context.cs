﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class CMSEntities : DbContext
    {
        public CMSEntities()
            : base("name=CMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Brand> Brand { get; set; }
        public DbSet<Channel> Channel { get; set; }
        public DbSet<ChannelProperty> ChannelProperty { get; set; }
        public DbSet<CMS_Ecom_Sync_Result> CMS_Ecom_Sync_Result { get; set; }
        public DbSet<CMS_HM_Costing> CMS_HM_Costing { get; set; }
        public DbSet<CMS_HMGroup_Relation> CMS_HMGroup_Relation { get; set; }
        public DbSet<CMS_HMNUM> CMS_HMNUM { get; set; }
        public DbSet<CMS_ProductCTN> CMS_ProductCTN { get; set; }
        public DbSet<CMS_ProductDimension> CMS_ProductDimension { get; set; }
        public DbSet<CMS_SKU> CMS_SKU { get; set; }
        public DbSet<CMS_SKU_Category> CMS_SKU_Category { get; set; }
        public DbSet<CMS_SKU_Colour> CMS_SKU_Colour { get; set; }
        public DbSet<CMS_SKU_Costing> CMS_SKU_Costing { get; set; }
        public DbSet<CMS_SKU_Material> CMS_SKU_Material { get; set; }
        public DbSet<CMS_StockKey> CMS_StockKey { get; set; }
        public DbSet<LogOfUserLogin> LogOfUserLogin { get; set; }
        public DbSet<LogOfUserOperating> LogOfUserOperating { get; set; }
        public DbSet<LogOfUserOperatingDetails> LogOfUserOperatingDetails { get; set; }
        public DbSet<MediaLibrary> MediaLibrary { get; set; }
        public DbSet<Menu_Resource> Menu_Resource { get; set; }
        public DbSet<PropertyValues> PropertyValues { get; set; }
        public DbSet<Role_Menu_Relation> Role_Menu_Relation { get; set; }
        public DbSet<Security_Role> Security_Role { get; set; }
        public DbSet<SKU_HM_Relation> SKU_HM_Relation { get; set; }
        public DbSet<SKU_Media_Relation> SKU_Media_Relation { get; set; }
        public DbSet<SKU_Status> SKU_Status { get; set; }
        public DbSet<sysdiagrams> sysdiagrams { get; set; }
        public DbSet<User_Channel_Relation> User_Channel_Relation { get; set; }
        public DbSet<User_Profile> User_Profile { get; set; }
        public DbSet<User_Profile_Ext> User_Profile_Ext { get; set; }
        public DbSet<User_Role_Relation> User_Role_Relation { get; set; }
        public DbSet<User_Status> User_Status { get; set; }
        public DbSet<CMS_HM_Inventory> CMS_HM_Inventory { get; set; }
        public DbSet<CMS_User_Profile_V> CMS_User_Profile_V { get; set; }
        public DbSet<Ecom_ImageUrls_V> Ecom_ImageUrls_V { get; set; }
        public DbSet<Ecom_ProductCartons_V> Ecom_ProductCartons_V { get; set; }
        public DbSet<HMGroup_Inventory_V> HMGroup_Inventory_V { get; set; }
        public DbSet<PO_Order_V> PO_Order_V { get; set; }
        public DbSet<Reports_LowInventory_V> Reports_LowInventory_V { get; set; }
        public DbSet<Reports_Margin_By_Product_V> Reports_Margin_By_Product_V { get; set; }
        public DbSet<SKU_Channel_Brand_Info_V> SKU_Channel_Brand_Info_V { get; set; }
        public DbSet<SKU_LowestInventory_V> SKU_LowestInventory_V { get; set; }
        public DbSet<SKUInfo_V> SKUInfo_V { get; set; }
        public DbSet<SKUInfo2_V> SKUInfo2_V { get; set; }
        public DbSet<User_Role_Menu_V> User_Role_Menu_V { get; set; }
        public DbSet<WebPO_Category_V> WebPO_Category_V { get; set; }
        public DbSet<WebPO_Colour_V> WebPO_Colour_V { get; set; }
        public DbSet<WebPO_HM_Colour_Material_V> WebPO_HM_Colour_Material_V { get; set; }
        public DbSet<WebPO_ImageUrls_V> WebPO_ImageUrls_V { get; set; }
        public DbSet<WebPO_Material_V> WebPO_Material_V { get; set; }
        public DbSet<WebPo_NewDataFromPOD_V> WebPo_NewDataFromPOD_V { get; set; }
        public DbSet<WebPo_NewHM_V> WebPo_NewHM_V { get; set; }
        public DbSet<WebPO_StockKeyInventory_V> WebPO_StockKeyInventory_V { get; set; }
        public DbSet<WebPO_Users_V> WebPO_Users_V { get; set; }
    
        public virtual int Ecom_AvgOrder_SP(string sTime, string eTime, ObjectParameter avgOrder)
        {
            var sTimeParameter = sTime != null ?
                new ObjectParameter("sTime", sTime) :
                new ObjectParameter("sTime", typeof(string));
    
            var eTimeParameter = eTime != null ?
                new ObjectParameter("eTime", eTime) :
                new ObjectParameter("eTime", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Ecom_AvgOrder_SP", sTimeParameter, eTimeParameter, avgOrder);
        }
    
        public virtual int Ecom_AvgOrderAmount_SP(string sTime, string eTime)
        {
            var sTimeParameter = sTime != null ?
                new ObjectParameter("sTime", sTime) :
                new ObjectParameter("sTime", typeof(string));
    
            var eTimeParameter = eTime != null ?
                new ObjectParameter("eTime", eTime) :
                new ObjectParameter("eTime", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Ecom_AvgOrderAmount_SP", sTimeParameter, eTimeParameter);
        }
    
        public virtual int Ecom_Order_SP(string sTime, string eTime)
        {
            var sTimeParameter = sTime != null ?
                new ObjectParameter("sTime", sTime) :
                new ObjectParameter("sTime", typeof(string));
    
            var eTimeParameter = eTime != null ?
                new ObjectParameter("eTime", eTime) :
                new ObjectParameter("eTime", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Ecom_Order_SP", sTimeParameter, eTimeParameter);
        }
    
        public virtual int Ecom_ReportByChannel_SP(Nullable<int> page, Nullable<int> rows, Nullable<int> channelID, Nullable<int> brandID, Nullable<System.DateTime> startTime, Nullable<System.DateTime> endTime, string orderBy, string orderType, ObjectParameter count, ObjectParameter sumSalesTotal, ObjectParameter sumUnitsSold, ObjectParameter sumAvgAmont)
        {
            var pageParameter = page.HasValue ?
                new ObjectParameter("page", page) :
                new ObjectParameter("page", typeof(int));
    
            var rowsParameter = rows.HasValue ?
                new ObjectParameter("rows", rows) :
                new ObjectParameter("rows", typeof(int));
    
            var channelIDParameter = channelID.HasValue ?
                new ObjectParameter("ChannelID", channelID) :
                new ObjectParameter("ChannelID", typeof(int));
    
            var brandIDParameter = brandID.HasValue ?
                new ObjectParameter("BrandID", brandID) :
                new ObjectParameter("BrandID", typeof(int));
    
            var startTimeParameter = startTime.HasValue ?
                new ObjectParameter("StartTime", startTime) :
                new ObjectParameter("StartTime", typeof(System.DateTime));
    
            var endTimeParameter = endTime.HasValue ?
                new ObjectParameter("EndTime", endTime) :
                new ObjectParameter("EndTime", typeof(System.DateTime));
    
            var orderByParameter = orderBy != null ?
                new ObjectParameter("OrderBy", orderBy) :
                new ObjectParameter("OrderBy", typeof(string));
    
            var orderTypeParameter = orderType != null ?
                new ObjectParameter("OrderType", orderType) :
                new ObjectParameter("OrderType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Ecom_ReportByChannel_SP", pageParameter, rowsParameter, channelIDParameter, brandIDParameter, startTimeParameter, endTimeParameter, orderByParameter, orderTypeParameter, count, sumSalesTotal, sumUnitsSold, sumAvgAmont);
        }
    
        public virtual int Ecom_ReportByHM_SP(Nullable<int> page, Nullable<int> rows, string hMNUM, string hMName, Nullable<int> channelID, Nullable<int> brandID, Nullable<System.DateTime> startTime, Nullable<System.DateTime> endTime, string orderBy, string orderType, ObjectParameter count, ObjectParameter sumSalesTotal, ObjectParameter sumUnitsSold, ObjectParameter sumAvgAmont)
        {
            var pageParameter = page.HasValue ?
                new ObjectParameter("page", page) :
                new ObjectParameter("page", typeof(int));
    
            var rowsParameter = rows.HasValue ?
                new ObjectParameter("rows", rows) :
                new ObjectParameter("rows", typeof(int));
    
            var hMNUMParameter = hMNUM != null ?
                new ObjectParameter("HMNUM", hMNUM) :
                new ObjectParameter("HMNUM", typeof(string));
    
            var hMNameParameter = hMName != null ?
                new ObjectParameter("HMName", hMName) :
                new ObjectParameter("HMName", typeof(string));
    
            var channelIDParameter = channelID.HasValue ?
                new ObjectParameter("ChannelID", channelID) :
                new ObjectParameter("ChannelID", typeof(int));
    
            var brandIDParameter = brandID.HasValue ?
                new ObjectParameter("BrandID", brandID) :
                new ObjectParameter("BrandID", typeof(int));
    
            var startTimeParameter = startTime.HasValue ?
                new ObjectParameter("StartTime", startTime) :
                new ObjectParameter("StartTime", typeof(System.DateTime));
    
            var endTimeParameter = endTime.HasValue ?
                new ObjectParameter("EndTime", endTime) :
                new ObjectParameter("EndTime", typeof(System.DateTime));
    
            var orderByParameter = orderBy != null ?
                new ObjectParameter("OrderBy", orderBy) :
                new ObjectParameter("OrderBy", typeof(string));
    
            var orderTypeParameter = orderType != null ?
                new ObjectParameter("OrderType", orderType) :
                new ObjectParameter("OrderType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Ecom_ReportByHM_SP", pageParameter, rowsParameter, hMNUMParameter, hMNameParameter, channelIDParameter, brandIDParameter, startTimeParameter, endTimeParameter, orderByParameter, orderTypeParameter, count, sumSalesTotal, sumUnitsSold, sumAvgAmont);
        }
    
        public virtual int Ecom_ReportByProduct_SP(Nullable<int> page, Nullable<int> rows, string sKUOrder, string productName, Nullable<int> channelID, Nullable<int> brandID, Nullable<System.DateTime> startTime, Nullable<System.DateTime> endTime, string orderBy, string orderType, ObjectParameter count, ObjectParameter sumSalesTotal, ObjectParameter sumUnitsSold, ObjectParameter sumAvgAmont)
        {
            var pageParameter = page.HasValue ?
                new ObjectParameter("page", page) :
                new ObjectParameter("page", typeof(int));
    
            var rowsParameter = rows.HasValue ?
                new ObjectParameter("rows", rows) :
                new ObjectParameter("rows", typeof(int));
    
            var sKUOrderParameter = sKUOrder != null ?
                new ObjectParameter("SKUOrder", sKUOrder) :
                new ObjectParameter("SKUOrder", typeof(string));
    
            var productNameParameter = productName != null ?
                new ObjectParameter("ProductName", productName) :
                new ObjectParameter("ProductName", typeof(string));
    
            var channelIDParameter = channelID.HasValue ?
                new ObjectParameter("ChannelID", channelID) :
                new ObjectParameter("ChannelID", typeof(int));
    
            var brandIDParameter = brandID.HasValue ?
                new ObjectParameter("BrandID", brandID) :
                new ObjectParameter("BrandID", typeof(int));
    
            var startTimeParameter = startTime.HasValue ?
                new ObjectParameter("StartTime", startTime) :
                new ObjectParameter("StartTime", typeof(System.DateTime));
    
            var endTimeParameter = endTime.HasValue ?
                new ObjectParameter("EndTime", endTime) :
                new ObjectParameter("EndTime", typeof(System.DateTime));
    
            var orderByParameter = orderBy != null ?
                new ObjectParameter("OrderBy", orderBy) :
                new ObjectParameter("OrderBy", typeof(string));
    
            var orderTypeParameter = orderType != null ?
                new ObjectParameter("OrderType", orderType) :
                new ObjectParameter("OrderType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Ecom_ReportByProduct_SP", pageParameter, rowsParameter, sKUOrderParameter, productNameParameter, channelIDParameter, brandIDParameter, startTimeParameter, endTimeParameter, orderByParameter, orderTypeParameter, count, sumSalesTotal, sumUnitsSold, sumAvgAmont);
        }
    
        public virtual int Excel_ReportByChannel_SP(Nullable<int> channelID, Nullable<int> brandID, Nullable<System.DateTime> startTime, Nullable<System.DateTime> endTime, string orderBy, string orderType)
        {
            var channelIDParameter = channelID.HasValue ?
                new ObjectParameter("ChannelID", channelID) :
                new ObjectParameter("ChannelID", typeof(int));
    
            var brandIDParameter = brandID.HasValue ?
                new ObjectParameter("BrandID", brandID) :
                new ObjectParameter("BrandID", typeof(int));
    
            var startTimeParameter = startTime.HasValue ?
                new ObjectParameter("StartTime", startTime) :
                new ObjectParameter("StartTime", typeof(System.DateTime));
    
            var endTimeParameter = endTime.HasValue ?
                new ObjectParameter("EndTime", endTime) :
                new ObjectParameter("EndTime", typeof(System.DateTime));
    
            var orderByParameter = orderBy != null ?
                new ObjectParameter("OrderBy", orderBy) :
                new ObjectParameter("OrderBy", typeof(string));
    
            var orderTypeParameter = orderType != null ?
                new ObjectParameter("OrderType", orderType) :
                new ObjectParameter("OrderType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Excel_ReportByChannel_SP", channelIDParameter, brandIDParameter, startTimeParameter, endTimeParameter, orderByParameter, orderTypeParameter);
        }
    
        public virtual int Excel_ReportByHM_SP(string hMNUM, string hMName, Nullable<int> channelID, Nullable<int> brandID, Nullable<System.DateTime> startTime, Nullable<System.DateTime> endTime, string orderBy, string orderType)
        {
            var hMNUMParameter = hMNUM != null ?
                new ObjectParameter("HMNUM", hMNUM) :
                new ObjectParameter("HMNUM", typeof(string));
    
            var hMNameParameter = hMName != null ?
                new ObjectParameter("HMName", hMName) :
                new ObjectParameter("HMName", typeof(string));
    
            var channelIDParameter = channelID.HasValue ?
                new ObjectParameter("ChannelID", channelID) :
                new ObjectParameter("ChannelID", typeof(int));
    
            var brandIDParameter = brandID.HasValue ?
                new ObjectParameter("BrandID", brandID) :
                new ObjectParameter("BrandID", typeof(int));
    
            var startTimeParameter = startTime.HasValue ?
                new ObjectParameter("StartTime", startTime) :
                new ObjectParameter("StartTime", typeof(System.DateTime));
    
            var endTimeParameter = endTime.HasValue ?
                new ObjectParameter("EndTime", endTime) :
                new ObjectParameter("EndTime", typeof(System.DateTime));
    
            var orderByParameter = orderBy != null ?
                new ObjectParameter("OrderBy", orderBy) :
                new ObjectParameter("OrderBy", typeof(string));
    
            var orderTypeParameter = orderType != null ?
                new ObjectParameter("OrderType", orderType) :
                new ObjectParameter("OrderType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Excel_ReportByHM_SP", hMNUMParameter, hMNameParameter, channelIDParameter, brandIDParameter, startTimeParameter, endTimeParameter, orderByParameter, orderTypeParameter);
        }
    
        public virtual int Excel_ReportByProduct_SP(string sKUOrder, string productName, Nullable<int> channelID, Nullable<int> brandID, Nullable<System.DateTime> startTime, Nullable<System.DateTime> endTime, string orderBy, string orderType)
        {
            var sKUOrderParameter = sKUOrder != null ?
                new ObjectParameter("SKUOrder", sKUOrder) :
                new ObjectParameter("SKUOrder", typeof(string));
    
            var productNameParameter = productName != null ?
                new ObjectParameter("ProductName", productName) :
                new ObjectParameter("ProductName", typeof(string));
    
            var channelIDParameter = channelID.HasValue ?
                new ObjectParameter("ChannelID", channelID) :
                new ObjectParameter("ChannelID", typeof(int));
    
            var brandIDParameter = brandID.HasValue ?
                new ObjectParameter("BrandID", brandID) :
                new ObjectParameter("BrandID", typeof(int));
    
            var startTimeParameter = startTime.HasValue ?
                new ObjectParameter("StartTime", startTime) :
                new ObjectParameter("StartTime", typeof(System.DateTime));
    
            var endTimeParameter = endTime.HasValue ?
                new ObjectParameter("EndTime", endTime) :
                new ObjectParameter("EndTime", typeof(System.DateTime));
    
            var orderByParameter = orderBy != null ?
                new ObjectParameter("OrderBy", orderBy) :
                new ObjectParameter("OrderBy", typeof(string));
    
            var orderTypeParameter = orderType != null ?
                new ObjectParameter("OrderType", orderType) :
                new ObjectParameter("OrderType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Excel_ReportByProduct_SP", sKUOrderParameter, productNameParameter, channelIDParameter, brandIDParameter, startTimeParameter, endTimeParameter, orderByParameter, orderTypeParameter);
        }
    
        public virtual int SynchData_NewHM_SP()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SynchData_NewHM_SP");
        }
    
        public virtual int SynchData_UsersFromWEBPO_SP(string creator, ObjectParameter affectedRows)
        {
            var creatorParameter = creator != null ?
                new ObjectParameter("Creator", creator) :
                new ObjectParameter("Creator", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SynchData_UsersFromWEBPO_SP", creatorParameter, affectedRows);
        }
    }
}
