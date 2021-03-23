using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model
{
    public class OtherSystemImages
    {
        /// <summary>
        /// eCom或者WebPO
        /// </summary>
        public String SystemName { get; set; }

        /// <summary>
        /// 略缩图
        /// </summary>
        public String SmallPic { get; set; }

        /// <summary>
        /// 原图
        /// </summary>
        public String Pic { get; set; }
    }
}
