using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// Colour,Material,Category and SubCatgetory模型
    /// 用户ProductConfiguration页面的逻辑检查
    /// </summary>
    public class MCCC
    {
        public string Material { get; set; }
        public string Colour { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }

        /// <summary>
        /// 这2个字段用于颜色材料类别AuctoCompleted查询的时候返回结果使用
        /// CreatedDate:2014年3月26日10:50:04
        /// </summary>
        public long ID { get; set; }
        public string Name { get; set; }
    }
}
