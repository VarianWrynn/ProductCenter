using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    public class LomoPrinter
    {
        /// <summary>
        /// 终端机器ID
        /// </summary>
        public int terminal_id { get; set; }

        public string terminal_token { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        public int nonce { get; set; }

        /// <summary>
        /// 时间戳（UNIX）
        /// </summary>
        public long timestamp { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string signature { get; set; }

        /// <summary>
        /// 打印图⽚片URL（JPEG,874x1240px）
        /// </summary>
        public string job_img_url { get; set; }


        public int job_id { get ; set; }
    }
}
