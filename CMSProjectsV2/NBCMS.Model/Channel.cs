using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model
{
    /// <summary>
    /// 用户查询或者获取数据
    /// </summary>
    public class Channel_Model
    {

        public int ChannelID { get; set; }
        public string ChannelName { get; set; }
        public string ShortName { get; set; }
        public bool API { get; set; }
        public bool Export2CSV { get; set; }
        public string Modifier { get; set; }
        public DateTime Modify_Date { get; set; }
        public string strModify_Date { get; set; }

        /// <summary>
        /// 用户查询
        /// </summary>
        public int page { get; set; }
        public int rows { get; set; }

        public int queryAPI { get; set; }
        public int queryExport2CSV { get; set; }

        /// <summary>
        /// 这个字段用于图像关联SKU批量插入Channel的时候使用，其他场合无意义，不可删除。
        /// CreateDate:2014年2月14日10:51:40
        /// </summary>
        public long SKUID { get; set; }
    }
}
