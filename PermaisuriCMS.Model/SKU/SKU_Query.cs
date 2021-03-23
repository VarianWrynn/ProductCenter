using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// Product Sarch 页面查询模型
    /// </summary>
    [Serializable]
    public class SKU_Query_Model
    {
        /// <summary>
        /// SKU#/SKUName
        /// </summary>
        public string SKUOrder { get; set; }

        /// <summary>
        /// HM#/HMName
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 新增按照用户修改进行查询 2014年2月14日16:16:07 Chinese team testing feedback
        /// </summary>
        public string UpdateBy { get; set; }

        public int ChannelID { get; set; }
        public int BrandID { get; set; }
        public int CategoryID { get; set; }
        public int Status { get; set; }
        public Nullable<long> MulPartID { get; set; }

        /// <summary>
        /// For front-end Query
        /// 0：- query All product
        /// 1: - query Not-Group
        /// 2:- query Group only
        /// </summary>
        public int multiplePartType { get; set; }

        /// <summary>
        /// For front-end Query
        /// 0：- query All Inventory
        /// 1: - query In stock inventory
        /// 2:-  Low inventory 
        /// 3:-  Out of stock inventory
        /// 4: - Low and Out of stock inventory
        /// </summary>
        public int InventoryType { get; set; }

        /// <summary>
        /// 新增枚举：排序字段.Lee 2013-10-07.
        ///  ProductName = 1,
        ///  SKU = 2,
        ///  SKU_QTY =3,
        ///  RealInventory =4,
        ///  Price = 5,
        ///  Channel = 6,
        ///  UpdateOn = 7
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// 新增枚举：排序类型.Lee 2013-10-07,
        /// ASC = 0,
        /// DESC = 1
        /// </summary>
        public int OrderType { get; set; }

        public CMS_Ecom_Sync_Model CMS_Ecom_Sync { get; set; }

        public int page { get; set; }

        public int rows { get; set; }
    }
}