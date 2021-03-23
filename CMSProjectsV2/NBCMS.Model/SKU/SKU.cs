using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using PermaisuriCMS.Model.HMNUM;
using NBCMS.Model.Users;

namespace NBCMS.Model.SKU
{
    /// <summary>
    ///  为了排序和展示的方便，所有的价格都有2个字段，一个用来展示（String类型），一个用来排序（int,decimal,money类型）
    ///  Author Lee, Date:2013年10月15日16:10:49
    ///  Date:2013年11月7日17:51:00  大灾变啊...
    /// </summary>
    [Serializable]
    public partial class SKU_Model
    {
        public long SKUID { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }

        /// <summary>
        /// 当前SKUOrder的主图像 add by Lee 2013年10月15日16:30:58
        /// </summary>
        public String pImage { get; set; }

        public int SKU_QTY { get; set; }
        public decimal Price { get; set; }
        public string strPrice { get; set; }

        public int ChannelID { get; set; }
        public string ChannelName { get; set; }
        public string UPC { get; set; }

        public int Visibility { get; set; }
        public System.DateTime Modify_Date { get; set; }
        public string strModify_Date { get; set; }
        public string Modifier { get; set; }

        [AllowHtml]
        public string ProductDesc { get; set; }

        [AllowHtml]
        public string Specifications { get; set; }
        public string Keywords { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public decimal MSRP { get; set; }
        public string strMSRP { get; set; }

        public string URL { get; set; }

        public List<MediaLibrary_Model> channelMedias { get; set; }

        public int Status { get; set; }
        public string StatusName { get; set; }//for GetProductsDevList()
        
        public List<SKU_Status_Model> StatusList { get; set; }


        //public Nullable<long> MulPartID { get; set; }
         public bool IsGroup { get; set; }

        /// <summary>
        /// 用于组合产品信息的展示 2013/09/12
        /// 暂时作废 2013年11月21日16:42:04 
        /// </summary>
        //public List<CMS_HMNUM_Model> CMS_HMNUMList { get; set; }

        /// <summary>
        /// 改版后，由于CMS新增了组合产品的维护，所以一条SKU只对应一条一个HM#，如果在组合产品形式下，则需要通过父产品找到子产品以列表形式返回
        /// 这里的 CMS_HMNUM 既当做组合产品使用（当前SKU关联的是组合产品的时候），又当做基础产品使用（在关联为非组合产品的情况下）
        /// CreateDate:2013年11月21日16:42:11
        /// </summary>
        public CMS_HMNUM_Model CMS_HMNUM { get; set; }

        ///// <summary>
        ///// when:current item associated with a GroupHM,我们需要通过组合产品逆推后去找到其子产品的列表来展示其箱柜和尺寸信息
        ///// CreateDate:2013年11月21日16:46:02
        ///// </summary>
        //public List<CMS_HMNUM_Model> CMS_HMNUM_ChildrenList { get; set; }


        //实际库存值，用来给客户端做库存高中低的判断
        public long StockByPcs { get; set; }


        public CMS_SKU_Costing_Model SKU_Costing { get; set; }

        /// <summary>
        /// 为了做渠道控制，在这个对象里面新增了用户信息对象，用来给前端判断某些敏感字段，比如价格信息是否展示
        /// CreateDate:2014年1月8日9:52:25
        /// </summary>
        public User_Profile_Model userInfo { get; set; }


        /// <summary>
        /// 该字段是为了变更需求，把图像从本地切换到云端而设置的
        /// 为了适应Razor语法，把这些额外的信息全部包含在一个类，前端可以方便或者，这是我目前想到的最便捷的办法了。
        /// CreateDate:2014年1月8日9:54:27
        /// </summary>
        public String CMSImgUrl { get; set; }

        /// <summary>
        /// CreateDate:2014年1月10日17:06:35
        /// </summary>
        public long ColourID { get; set; }
        public long MaterialID { get; set; }
        public long CategoryID { get; set; }
        public long SubCategoryID { get; set; }

        /// <summary>
        /// CreateDate:2014年2月14日16:45:08
        /// </summary>
        public String UpdateOn { get; set; }

        /// <summary>
        /// CreateDate:2014年1月11日10:32:05
        /// </summary>
        public CMS_SKU_Colour_Model Colour { get; set; }
        public CMS_SKU_Material_Model Material { get; set; }
        public CMS_SKU_Category_Model Category { get; set; }
        public CMS_SKU_Category_Model SubCategory { get; set; }

        /// <summary>
        /// 使用枚举可以减少对数据库的查询...
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, int> GetProductVisibility()
        {
            Type pVisiblity = typeof(ProductVisibility);
            Array arrValues = Enum.GetValues(pVisiblity);
            String[] arrNames = Enum.GetNames(pVisiblity);
            Dictionary<String, int> dic = new Dictionary<string, int>();
            for (int i = 0; i < arrValues.Length; i++)
            {
                dic.Add(arrNames[i], Convert.ToInt32(arrValues.GetValue(i)));
            }
            return dic;
        }
    }

    ///// <summary>
    ///// 由于产品的状态是核心之一，从头到尾都要围绕产品状态对产品进行描述和维护，所有打算用一张表来替代这里的枚举！
    ///// 需要注意枚举的缺点是，描述无法用空格，包括2个字符串之间都无法用..
    ///// 2013年9月9日11:08:45 王力
    ///// 不设置0，为了查询方便...等于0代表查询所有状态
    ///// </summary>
    //public enum ProductStatus
    //{
    //    New = 1,
    //    MediaCreation = 2,
    //    MarketingDevelopment = 3,
    //    Complete = 4,
    //    Active = 5,
    //    Discontinued = 6,
    //    NewDuplicated = 7
    //}

    public enum ProductVisibility
    {
        Online = 0,
        Offline = 1,
        Discontinued = 2
    }
}
