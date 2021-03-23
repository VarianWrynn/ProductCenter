using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    [Serializable]
    public class MediaLibrary_Model
    {
        public long MediaID { get; set; }
        public long ProductID { get; set; }
        public string HMNUM { get; set; }
        public int SerialNum { get; set; }
        public string ImgName { get; set; }
        public int MediaType { get; set; }
        public string fileFormat { get; set; }
        public string fileSize { get; set; }
        public int fileWidth { get; set; }
        public int fileHeight { get; set; }
        public string Description { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateOn { get; set; }
        public string strCreateOn { get; set; }
        public List<CMS_SKU_Model> CMS_SKU { get; set; }
       
        
        /// <summary>
        /// 2014年2月14日13:59:09
        /// </summary>
        public bool IsPrimaryImages { get; set; }

        public int CloudStatusID { get; set; }

        /// <summary>
        /// 2014年4月15日
        /// </summary>
        public MediaCloudStatus_Model MediaCloudStatus { get; set; }
    }

    /// <summary>
    /// 当前图像分组的类型，目前是根据图像的分辨率（长X宽）来决定的，用于客户端的根据分辨率进行展示
    /// CreateDate:2014年1月6日11:04:03
    /// </summary>
    public class MediaGroupBy_Model
    {
        public int fileWidth { get; set; }
        public int fileHeight { get; set; }
    }

    /// <summary>
    /// 查询类，与前端的查询条件字段一一对应
    /// </summary>
    public class MediaLibrary_QueryModel
    {
        public int page { get; set; }

        public int rows { get; set; }

        /// <summary>
        /// New add on 2014年1月18日15:03:25
        /// </summary>
        public string HMNUM { get; set; }

        public string keyWords { get; set; }

        public int Channel { get; set; }

        public int Brand { get; set; }

        public int Format { get; set; }

        /// <summary>
        /// Media search/filter field “Status” only requires two options, “Attached to SKU” and “Unattached” 2013年9月26日16:09:00  David
        /// </summary>
        public int Status { get; set; }

        public DateTime timeStart { get; set; }

        public DateTime timeEnd { get; set; }

        /// <summary>
        /// 是否排除掉和SKU关联的图像,与SKUOrde联合过滤
        /// Change:不应该和SKUOrder关联，而应该和SKUID关联。
        //ChangeDate:2013年12月6日16:16:02 原来是根据SKUOrder来排除，但是这样子在批量复制（DupliatedProduct)情况下，一个
        //渠道的SKUOrder选择了图片之后，会影响到其他渠道的SKUOrder的图片关联，故而应该用SKUID来关联排除。
        /// </summary>
        public bool IsExcludeSKU { get; set; }

        public long SKUID { get; set; }

        public String SKUOrder { get; set; }

        /// <summary>
        /// 只获取和ProductID相关的图像 2013年11月29日14:28:01 Lee
        /// </summary>
        public long ProductID { get; set; }

        public int CloudStatusID { get; set; }
        
    }

}
