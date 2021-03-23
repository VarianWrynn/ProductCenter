using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// HM产品信息，注意HM# 和HMGroup由于在数据是合体的，所以这里返回的对象也是合体的，根据IsGroup做判断
    /// Author:Lee; Date:2013年11月18日15:34:21
    /// </summary>
    [Serializable]
    public class CMS_HMNUM_Model
    {

        public long ProductID { get; set; }
        public string HMNUM { get; set; }
        public string ProductName { get; set; }
        public string StockKey { get; set; }

        public long StockKeyID { get; set; }

        /// <summary>
        /// 这个字段是2014年6月6日11:57:44新增的用来在Add New SKU的时候给 SKU_HM_Relation表的R_QTY字段赋值的
        /// </summary>
        public int R_QTY { get; set; }

        /// <summary>
        /// 装箱数就是指一个箱子里面装了几Pcs的这个东西。公式：(Pices除以MP==总箱数).
        /// 组合产品本身没有MasterPack，是根据子产品自己去计算，类似StockKey的逻辑。
        /// CreateDate:2014年2月12日10:25:48
        /// </summary>
        public long MasterPack { get; set; }

        /// <summary>
        /// 还不确定是否在库表里面新增这个字段，但是至少页面上确定必须有这个字段存在，因为前段Razor不支持动态计算
        /// CreateDate:2014年3月22日
        /// </summary>
        public int BoxNum { get; set; }

        /// <summary>
        /// 当是组合产品的时候，给前端展示的应该是N/A格式。和金额同理
        /// </summary>
        public string strMasterPack { get; set; }

        [AllowHtml]
        public string Comments { get; set; }
        public System.DateTime CreateOn { get; set; }
        public long HMCostID { get; set; }
        public long CategoryID { get; set; }

        /// <summary>
        /// 2013年12月31日16:07:00
        /// </summary>
        public int Loadability { get; set; }

        public bool IsGroup { get; set; }

        /// <summary>
        /// 当前HMNUM对应的StockKey的库存
        /// </summary>
        public int StockKeyQTY { get; set; }

        /// <summary>
        /// 用户前端的查询 2013年11月20日16:36:25
        /// </summary>
        public int queryIsGroup { get; set; }

        /// <summary>
        /// 是否只查询还未和SKU关联的HM# 2013年12月2日16:13:37
        /// </summary>
        public bool ISOrphan { get; set; }

        /// <summary>
        /// 用来排除掉那些已经被关联了某个组合产品的子产品。该过滤对需要创建组合产品的时候也别有用 2014年6月3日16:59:18
        /// </summary>
        public bool IsExcludedSubHMNUM { get; set; }

        /// <summary>
        /// 后端返回：用于标识当前HM对应的最大的图像序列，比如当前图像关联了3张图，则它的最大序列是3....
        /// </summary>
        public int MaxImaSeq { get; set; }

        /// <summary>
        /// 0-default:获取全部 1：获取非组合产品 ，2：仅获取组合产品
        /// </summary>
        public int HMType { get; set; }

        /// <summary>
        /// 目前只要用于组合产品的状态控制，基础产品目前还没有维护装他....
        /// Changed:组合产品的状态 似乎也可以不用控制了，这个字段改用来标识当前产品是否启用or禁用  Value:0(default or null)启用,1-禁用 [2014年1月10日14:05:33]
        /// Changed2:这个字段被全新定义为：　1－unaudited; 2-audited,3-disabled SKU新增的时候只能查询到 audited状态的HMNUM  [2014年5月8日18:08:00]
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// 该字段用来配合StatusID查询的 0：全部查询 ； 1：代表值查询未审核unaudit的数据； 2代表查询已审核通过的数据，包括audited 和 disabled（业务由linda建议)[2014年5月8日]
        /// </summary>
        public int RequestType { get; set; }

        public CMS_HM_Costing_Model HM_Costing { get; set; } //add 2013年11月12日15:41:56

        /// <summary>
        /// 箱规 CTN是carton的简写 纸箱就是纸箱的编号。
        /// Dimesion是SKU的尺寸，Carton是HMNUM的尺寸(Oliver)
        /// </summary>
        public List<CMS_ProductCTN_Model> CTNList { get; set; }

        /// <summary>
        /// 产品尺寸(零部件等)
        /// Dimesion是HMNUM产品的尺寸，Carton是HMNUM的尺寸(Oliver)（2-11-2014）
        /// 目前Dimesion维护的是HMNUM本身的尺寸，未来Carton本身也要有个尺寸，但是如果这个一旦改动，WEBPO,Ecom也要改动。(Olver,2014年2月27日)
        /// </summary>
        public List<CMS_ProductDimension_Model> DimList { get; set; }

        public List<CMS_SKU_Model> SKUList { get; set; }//add 2013年11月13日11:48:29

        /// <summary>
        /// 子HM#的ID，当当前的对象是组合产品的时候才拥有
        /// </summary>
        public long ChildrenProductID { get; set; }

        /// <summary>
        /// 用于组合产品使用 2013年11月20日17:45:58
        /// </summary>
        public List<CMS_HMNUM_Model> Children_CMS_HMNUM_List { get; set; }

        /// <summary>
        /// 当前HMNUM被哪些组合产品引用到了 2014年6月3日17:33:13
        /// </summary>
        public List<CMS_HMNUM_Model> Parents_CMS_HMNUM_List { get; set; }


        /// <summary>
        /// 当前对象是组合产品时候，这个字段代表的是其子产品的包含的数量，比如4条椅子或者一个桌子
        /// 2013年11月18日15:36:32
        /// AddRemark:SellPack:一个产品在Overtock 一个QTY卖一个Pieces,在eBay上组合卖，
        /// 一QTY卖2pices卖9.9折, 一QTY卖4pices卖9.5折,买多越多优惠,【不同的4piecs对应不同的SKU】(Rovien)
        /// 
        /// 
        /// Ecom中的定义： 
        /// Sellset，组合产品中的定义，在Group中，用于标识子产品在组合产品中的PCS数量
        /// Sellpack，销售数量定义，表示网站订单1QTY对应系统中销售N个产品的意思 
        /// boxnum就是sellpack,一个货号我卖6个，装3箱,则：pcs 6 mp2 sellpack 3 Add Remark:2014年4月15日15:09:08
        /// </summary>
        public int SellSets { get; set; }


        /// <summary>
        /// 当前HM#对于关联的图像(0---N 的关系)
        /// 2013年12月25日10:22:28
        /// </summary>
        public List<MediaLibrary_Model> MediaList { get; set; }

        /// <summary>
        /// 添加Webpo的图像地址 2013年12月25日16:19:10
        /// </summary>
        public OtherSystemImages webSystemImage { get; set; }

        /// <summary>
        /// HM的材料信息，正常是一对一关系，如果更换材料，则必须是产生一个新的HM#
        /// CreateDate:2014年1月8日16:03:15
        /// </summary>
        public CMS_HMNUM_Material_Model HMMaterial { get; set; }

        /// <summary>
        /// HM的颜色，正常也是一对一关系，如果颜色发生改变，也应该是一个新的HM#
        /// CreateDate:2014年1月8日16:03:08
        /// </summary>
        public CMS_HMNUM_Colour_Model HMColour { get; set; }

        /// <summary>
        /// HMNUM的类别，包含父类别和子类别 2014年6月5日10:50:40
        /// </summary>
        public WebPO_Category_V_Model Category { get; set; }

        /// <summary>
        /// 这个字段应该获取自WebPO的 WEBPO.DBO.PODetails,对应还有一个毛重LaneWeight,毛重参数参考HM-ALL Item，放在Cartons维护
        /// </summary>
        public decimal NetWeight { get; set; }

        /// <summary>
        /// 这个字段用来标识是否需..............
        /// </summary>
        public List<long> ExcludedProductIDs { get; set; }


        public CMS_HMNUM_Status_Model CMS_HMNUM_Status { get; set; }

        /// <summary>
        /// Nullable!
        /// </summary>
        public int ShipViaTypeID { get; set; }

        /// <summary>
        /// 注意关系表是nullabel!
        /// </summary>
        public bool IsCloud { get; set; }


        /// <summary>
        /// 物流还是快递？卡车还是Fexdex?2014年5月7日15:52:37,注意关系表是nullable!
        /// 所有是nullbale的关系，提取前都必须做空判断
        /// </summary>
        public CMS_ShipViaType_Model CMS_ShipVia_Type { get; set; }
    }
}
