using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// 图像上传云端的状态。2014年4月8日
    /// </summary>
   public class MediaCloudStatus_Model
    {
        public int CloudStatusId { get; set; }
        public string CloudStatusName { get; set; }
        public string Comments { get; set; }
        public DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateOn { get; set; }
        public string UpdateBy { get; set; }
    }
}
