using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitHMAllItem
{
    /// <summary>
    /// 消息通知事件参数类
    /// </summary>
    public class HMEventArgs : EventArgs
    {
        private string message;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        public HMEventArgs(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message
        {
            get { return message; }
        }
    }
}
