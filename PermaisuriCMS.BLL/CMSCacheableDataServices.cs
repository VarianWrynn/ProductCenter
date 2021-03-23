using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using System.Globalization;

namespace PermaisuriCMS.BLL
{
    public class CMSCacheableDataServices
    {
        /// <summary>
        /// 获取SKU相关联的那个类别信息下拉单
        /// Create:2014年1月11日14:53:03
        /// </summary>
        /// <returns></returns>
        public List<CMS_SKU_Category_Model> GetSKUCategoryList(long ParentID)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                return db.CMS_SKU_Category.Where(c => c.ParentID == ParentID).Select(c => new CMS_SKU_Category_Model
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName
                }).ToList();

            }
        }

        /// <summary>
        /// 获取和SKU相关的那个材料
        /// </summary>
        /// <returns></returns>
        public List<CMS_SKU_Material_Model> SKUMaterialList()
        {
            using (var db = new PermaisuriCMSEntities())
            {
                return db.CMS_SKU_Material.Select(m => new CMS_SKU_Material_Model
                {
                    MaterialID = m.MaterialID,
                    MaterialName = m.MaterialName
                }).ToList();
            }
        }


        /// <summary>
        /// 获取和SKU相关的那个颜色
        /// </summary>
        /// <returns></returns>
        public List<CMS_SKU_Colour_Model> SKUColourList()
        {
            using (var db = new PermaisuriCMSEntities())
            {
                return db.CMS_SKU_Colour.Select(c => new CMS_SKU_Colour_Model
                {
                    ColourID = c.ColourID,
                    ColourName = c.ColourName
                }).ToList();
            }
        }


        public List<Channel_Model> ChannelList()
        {
            using (var db = new PermaisuriCMSEntities())
            {
                return db.Channel.Select(c => new Channel_Model
                {
                    ChannelID = c.ChannelID,
                    ChannelName = c.ChannelName
                }).ToList();
            }
        }

        public List<Brands_Info_Model> BrandList()
        {
            using (var db = new PermaisuriCMSEntities())
            {
                return db.Brand.Select(c => new Brands_Info_Model
                {
                    Brand_Id = c.BrandID,
                    Brand_Name = c.BrandName
                }).ToList();
            }
        }

        /// <summary>
        /// 异步获取 Lee
        /// </summary>
        /// <returns></returns>
        public async Task<List<Brands_Info_Model>> BrandListAsync()
        {
            using (var db = new PermaisuriCMSEntities())
            {

                var brandList = await db.Brand.Select(c => new Brands_Info_Model
                  {
                      Brand_Id = c.BrandID,
                      Brand_Name = c.BrandName
                  }).ToListAsync();

                return brandList;
            }
        }

        public List<SKU_Status_Model> SKUStatusList()
        {

            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.SKU_Status.Select(s => new SKU_Status_Model
                    {
                        StatusID = s.StatusID,
                        StatusName = s.StatusName,
                        Remark = s.Remark
                    }).ToList();
                return query;
            }
        }

        /// <summary>
        /// 默认进来HMNUM是 =unaudited 未审核审核过后为= audited 禁用掉HMNUM =  disabled
        /// 2014年5月7日16:15:38
        /// </summary>
        /// <returns></returns>
        public List<CMS_HMNUM_Status_Model> HMNUMStatusList()
        {

            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_HMNUM_Status.Select(s => new CMS_HMNUM_Status_Model
                {
                    StatusID = s.StatusID,
                    StatusName = s.StatusName,
                    Remark = s.Remark
                }).ToList();
                return query;
            }
        }

        public List<CMS_ShipViaType_Model> ShipViaTypeList()
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_ShipViaType.Select(s => new CMS_ShipViaType_Model
                {
                    ShipViaTypeID = s.ShipViaTypeID,
                    ShipViaTypeName = s.ShipViaTypeName
                }).ToList();
                return query;
            }
        }


        /// <summary>
        ///  从WEBPO获取组合产品的下拉单，仅公于创建组合产品的时候调用。
        ///  CreateDate:2014年2月19日15:57:28
        /// </summary>
        /// <returns></returns>
        public List<WebPO_Category_V_Model> GetGroupCategoryFromWEBPO()
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.WebPO_Category_V.Where(v => v.CategoryName == "Sectional Group")
                    .Select(v => new WebPO_Category_V_Model
                    {
                        CategoryID = v.CategoryID,
                        CategoryName = v.CategoryName,
                        OrderIndex = v.OrderIndex.HasValue ? v.OrderIndex.Value : 0,
                        ParentCategoryID = v.ParentCategoryID.HasValue ? v.ParentCategoryID.Value : 0,
                        ParentCategoryName = v.ParentCategoryName
                    });
                return query.ToList();
            }
        }

        /// <summary>
        /// 2014年4月8日
        /// </summary>
        /// <returns></returns>
        public List<MediaCloudStatus_Model> CloudStatusList()
        {
            using (var db = new PermaisuriCMSEntities())
            {
                return db.MediaCloudStatus.Select(m => new MediaCloudStatus_Model
                {
                    CloudStatusId = m.CloudStatusId,
                    CloudStatusName = m.CloudStatusName
                }).ToList();
            }
        }


    }
}
