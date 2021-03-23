using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
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
        OK=1,
        Exception=2,
        Error=3
    }

    /// <summary>
    /// 动作类型，用户在记录用户详细操作信息时候，标记的操作类型。2014年3月10日
    /// </summary>
    public enum LogActionTypeEnum
    {
        Inert = 1,
        Update = 2,
        Delete = 3
    }
}
