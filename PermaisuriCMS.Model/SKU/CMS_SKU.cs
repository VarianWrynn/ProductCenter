using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;

namespace PermaisuriCMS.Model
{
    /// <summary>
    ///  为了排序和展示的方便，所有的价格都有2个字段，一个用来展示（String类型），一个用来排序（int,decimal,money类型）
    ///  Author Lee, Date:2013年10月15日16:10:49
    ///  Date:2013年11月7日17:51:00  大灾变啊...
    /// </summary>
    [Serializable]
    public partial class CMS_SKU_Model
    {
        public long SKUID { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }

        /// <summary>
        /// 当前SKUOrder的主图像 add by Lee 2013年10月15日16:30:58
        /// </summary>
        public String pImage { get; set; }

        /// <summary>
        /// pImage无法满足很多场合下图片的构造（比如需要知道图片格式，HMNUM等信息），每一次重构都是进步的来源之一。
        /// 2014年3月31日
        /// </summary>
        public MediaLibrary_Model pMedia { get; set; }

        /// <summary>
        /// 这个字段用于CMS-eCom同步的时候发送的图像的物理路径，比如 D:\\image\123.jpg.
        /// 按照严格的流程，图像应该要通过二进制传输，但是这个传输占据资源和带宽，先不做。
        /// CreateDate:2014年3月5日10:10:14
        /// </summary>
        public String Send2eComPath { get; set; }

        /// <summary>
        /// CMS的物理地址。Keep your mind,eCom同步本该是CMS发送到Ecom，余下部分由eCom来完成的，所以必须和CMS剥离出去。
        /// 2014年4月22日
        /// </summary>
        public string CMSPhysicalPath { get; set; }

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

        /// <summary>
        /// 网站卖给终端客户的价格，是NobleHosue公司无法控制的价格，通过API获取到提供公司报表做参考使用
        /// CreateDate:2014年3月10日9:45:51
        /// </summary>
        public decimal RetailPrice { get; set; }

        /// <summary>
        /// 网站卖给终端客户的价格，是NobleHosue公司无法控制的价格，通过API获取到提供公司报表做参考使用
        /// CreateDate:2014年3月10日9:45:51
        /// </summary>
        public string strRetailPrice { get; set; }

        //public decimal MSRP { get; set; }
        //public string strMSRP { get; set; }

        public string URL { get; set; }

        public List<MediaLibrary_Model> channelMedias { get; set; }

        public int StatusID { get; set; }
        public string StatusName { get; set; }//for GetProductsDevList()
        
        public List<SKU_Status_Model> StatusList { get; set; }


        //public Nullable<long> MulPartID { get; set; }
         public bool IsGroup { get; set; }

        /// <summary>
        /// 用于组合产品信息的展示 2013/09/12
        /// 暂时作废 2013年11月21日16:42:04 
        /// </summary>
        //public List<CMS_HMNUM_Model> CMS_HMNUMList { get; set; }



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

        public SKU_HM_Relation_Model SKU_HM_Relation { get; set; }

        /// <summary>
        /// 2014年3月28日
        /// </summary>
        public CMS_Ecom_Sync_Model CMS_Ecom_Sync { get; set; }

        /// <summary>
        /// 当前SKU--HMNUM--SKUs 2014年3月24日
        /// </summary>
        public List<CMS_SKU_Model> RelatedProducts { get; set; }

        /// <summary>
        ///  接受前端传递上来的ID直接插入数据库
        /// </summary>
        public int ShipViaTypeID { get; set; }

        /// <summary>
        /// CMS的ShipVa。物流还是快递？卡车还是Fexdex?2014年5月7日15:52:37,注意关系表是nullable!
        /// </summary>
        public CMS_ShipViaType_Model CMS_ShipViaType { get; set; }

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

    public static partial class EnumExtensions
    {
        public static string ToText(this ProductVisibility value)
        {
            switch (value)
            {
                case ProductVisibility.Discontinued:
                    return "Discontinued";
                case ProductVisibility.Offline:
                    return "Offline";
                case ProductVisibility.Online:
                    return "Online";
                default:
                    return "Online";
            }
        }
    }
}
