using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Permaisuri.Controllers.ControllerExt;
using Permaisuri.Filters;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    /// <summary>
    /// 所有可以换成的下拉单数据 都可以缓存在这里，但是需要注意如果数据太多，应该采用AutoCompleted的方式去实现
    /// CreateDate:2014年1月11日11:34:33
    /// </summary>
// ReSharper disable InconsistentNaming

    [NbcmsExceptions]
    [Record("创建", "张子阳", "", Memo = "这个类仅供演示")]   
    public class CMSCacheableDataController : Controller
// ReSharper restore InconsistentNaming
    {
        //
        // GET: /CMSCacheableData/

        /// <summary>
        /// CMS的类别下拉单列表,不能在这里传递可变参数（比如，SelectedItem,否则有几个值就要缓存多少次，似乎没意义）
        /// </summary>
        /// <returns></returns>
        //public ActionResult SKUCategoryList(Nullable<bool> isNeedAll)
        //{
        //    CMSCacheableDataServices ccs = new CMSCacheableDataServices();

        //    List<CMS_SKU_Category_Model> list = ccs.GetSKUCategoryList();

        //    return PartialView("_SKUOrder", list);
        //}

        [OutputCache(CacheProfile = "CMSCacheableDataProfile")]
        public ActionResult SkuCategoryList(bool isNeedAll)
        {
            var ccs = new CMSCacheableDataServices();
            var list = ccs.GetSKUCategoryList(0);
            if (isNeedAll)
            {
                list.Insert(0, new CMS_SKU_Category_Model//插入第一个位置
                {
                    CategoryID = 0,
                    CategoryName = "All"
                });
            }
            return Json(list);
        }

         [OutputCache(CacheProfile = "CMSCacheableDataProfile")]
        public ActionResult SKUSubCategoryList(long ParentID)
        {
            var ccs = new CMSCacheableDataServices();
            var list = ccs.GetSKUCategoryList(ParentID);
            return Json(list);
        }

        /// <summary>
        /// 获取SKU的材料类别，目前用户ProductConfiguraton页面使用
        /// </summary>
        /// <returns></returns>
         [OutputCache(CacheProfile = "CMSCacheableDataProfile")]
        public ActionResult SkuMaterialList(bool isNeedAll)
        {
            var ccs = new CMSCacheableDataServices();
            var list = ccs.SKUMaterialList();
            if (isNeedAll)
            {
                list.Insert(0, new CMS_SKU_Material_Model//插入第一个位置
                {
                    MaterialID = 0,
                    MaterialName = "All"
                });
            }
            return Json(list);
        }


        /// <summary>
        /// 获取SKU的颜色类别，目前用户ProductConfiguraton页面使用
        /// </summary>
        /// <returns></returns>
         [OutputCache(CacheProfile = "CMSCacheableDataProfile")]
        public ActionResult SKUColourList(bool isNeedAll)
        {
            var ccs = new CMSCacheableDataServices();
            var list = ccs.SKUColourList();
            if (isNeedAll)
            {
                list.Insert(0, new CMS_SKU_Colour_Model//插入第一个位置
                {
                    ColourID = 0,
                    ColourName = "All"
                });
            }
            return Json(list);
        }


        /// <summary>
        /// 注意这个方法不控制渠道控制！所以没意义，注释掉，放到CommonController里面实现一个类似的方法
        /// </summary>
        /// <param name="isNeedAll"></param>
        /// <returns></returns>
        //[OutputCache(CacheProfile = "CMSCacheableDataProfile")]
        //public ActionResult ChannelList(bool isNeedAll)
        //{
        //    CMSCacheableDataServices ccs = new CMSCacheableDataServices();
        //    List<Channel_Model> list = ccs.ChannelList();
        //    if (isNeedAll)
        //    {
        //        list.Insert(0, new Channel_Model//插入第一个位置
        //        {
        //            ChannelID = 0,
        //            ChannelName = "All"
        //        });
        //    }
        //    return Json(list);
        //}

       

        [OutputCache(CacheProfile = "CMSCacheableDataProfile")]
         public async Task<ActionResult> BrandList(bool isNeedAll)
        {
            var ccs = new CMSCacheableDataServices();
            var list = await ccs.BrandListAsync();
            if (isNeedAll)
            {
                list.Insert(0, new Brands_Info_Model//插入第一个位置
                {
                    Brand_Id = 0,
                    Brand_Name = "All"
                });
            }
            return Json(list);
        }

        [OutputCache(CacheProfile = "CMSCacheableDataProfile")]
        public ActionResult SKUStatusList(bool isNeedAll)
        {
            var ccs = new CMSCacheableDataServices();
            var list = ccs.SKUStatusList();
            if (!isNeedAll) return Json(list);
            list.Insert(0, new SKU_Status_Model//插入第一个位置
            {
                StatusID = 0,
                StatusName = "All",
            });

            /*Items requiring attention是一个复合状态，本身不应该直接出现在数据库，所以在这里用代码方式嵌入，所谓的复合的状态是指：
                 * 冲突的状态：例如，那些有库存但是却处于离线，或者那些已经脱销没库存了，却任然在线的SKU产品。
                 The intention behind "Items Requiring Attention" was to display the number of products in a 
                 * "conflicted status". For example, items in stock but not online, or items out of stock but still online.
                 * Is it possible to aggregate that data into a single figure? If not, we can find a substitute metric for
                 * this area.*/
            list.Insert(list.Count, new SKU_Status_Model//插入最后一个位置
            {
                StatusID = 10,
                StatusName = "Items requiring attention",
            });
            return Json(list);
        }

        /// <summary>
        /// 缓存6个小时，这个List返回的是强类型，而不是JSON对象，用于给SKUConfigutation页面显示状态使用 2014年5月13日10:57:00
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 60 * 60 * 60 * 6)]
        public List<SKU_Status_Model> SkuStatusListObject()
        {
            var ccs = new CMSCacheableDataServices();

            return ccs.SKUStatusList();
        }

        /// <summary>
        /// 获取HMNUM的状态 2014年5月7日16:13:06
        /// 默认进来HMNUM是 =unaudited 未审核审核过后为= audited 禁用掉HMNUM =  disabled 
        /// </summary>
        /// <param name="isNeedAll"></param>
        /// <returns></returns>
        [OutputCache(CacheProfile = "CMSCacheableDataProfile")]
        public ActionResult HmnumStatusList(bool isNeedAll)
        {
            var ccs = new CMSCacheableDataServices();
            var list = ccs.HMNUMStatusList();
            if (isNeedAll)
            {
                list.Insert(0, new CMS_HMNUM_Status_Model//插入第一个位置
                {
                    StatusID = 0,
                    StatusName = "All",
                });
            }
            return Json(list);
        }



        [OutputCache(CacheProfile = "CMSCacheableDataProfile")]
        public ActionResult ShipViaTypeList(bool isNeedAll)
        {
            var ccs = new CMSCacheableDataServices();
            var list = ccs.ShipViaTypeList();
            if (isNeedAll)
            {
                list.Insert(0, new CMS_ShipViaType_Model//插入第一个位置
                {
                    ShipViaTypeID = 0,
                    ShipViaTypeName = "All"
                });
            }
            return Json(list);
        }

        /// <summary>
        /// 从WEBPO获取组合产品的下拉单，仅公于创建组合产品的时候调用。缓存一小时
        /// CreateDate:2014年2月19日15:57:28
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 60 * 60)]
        public ActionResult GetGroupCategoryFromWebpo()
        {
            var ccs = new CMSCacheableDataServices();
            return Json(ccs.GetGroupCategoryFromWEBPO());
        }


        [OutputCache(CacheProfile = "CMSCacheableDataProfile")]
        public ActionResult CloudStatusList(bool isNeedAll)
        {
            var ccs = new CMSCacheableDataServices();
            var list = ccs.CloudStatusList();
            if (isNeedAll)
            {
                list.Insert(0, new MediaCloudStatus_Model//插入第一个位置
                {
                    CloudStatusId = 0,
                    CloudStatusName = "All"
                });
            }
            return Json(list);
        }
    }
}
