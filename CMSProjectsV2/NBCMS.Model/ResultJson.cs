using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model
{
    /// <summary>
    /// 定义返回客户端的JSON的格式
    /// </summary>
    public class NBCMSResultJson
    {
        public StatusType Status { get; set; }

        public Object Data { get; set; }
    }

    public enum StatusType
    {
        OK = 1,
        Exception = 2,
        Error = 3
    }
}
