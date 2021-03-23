using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    public class TuoZongFans
    {
        public string openid { get; set; }

        public long lasttime { get; set; }
    }

    

    /// <summary>
    /// 拓众粉丝行为接口参数
    /// </summary>
    public class TuoZongFansParams
    {
        public string merchant_id { get; set; }

        public TuoZongFans[] fans { get; set; }
    }

    public class TZLoginParams
    {
        public string UserName { get; set; }
        public string password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string check { get; set; }

        public string put_submit { get; set; }

        public string action_flag { get; set; }
    }
}
