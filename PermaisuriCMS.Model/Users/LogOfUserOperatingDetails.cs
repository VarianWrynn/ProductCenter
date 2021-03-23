using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// 详细日志信息 2014年3月10日
    /// </summary>
    [Serializable]
    public class LogOfUserOperatingDetails_Model
    {
        public long ID { get; set; }

        public string ModelName { get; set; }
        public int ActionType { get; set; }
        public String ActionName { get; set; }

        public long SKUID { get; set; }
        /// <summary>
        /// 查询的时候是SKU，返回结果给前端的时候，应该是SKU+ChannelName
        /// </summary>
        public string SKU { get; set; }
        public int ChannelID { get; set; }
        public string ChannelName { get; set; }
        public string HMNUM { get; set; }
        public long ProductID { get; set; }
        public string Descriptions { get; set; }
        public String CreateOn { get; set; }
        public string CreateBy { get; set; }

        /// <summary>
        /// 查询模式特有(easyUI插件)
        /// </summary>
        public int page { get; set; }
        public int rows { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
