using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model.SKU
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
    }
}
