using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PermaisuriCMS.Model.HMNUM
{
    /// <summary>
    /// 产品尺寸(零部件等)
    /// </summary>
    public class CMS_ProductDimension_Model
    {
        public long DimID { get; set; }
        public long ProductID { get; set; }
        public string HMNUM { get; set; }
        public string DimTitle { get; set; }
        public decimal DimLength { get; set; }
        public decimal DimWidth { get; set; }
        public decimal DimHeight { get; set; }
        public decimal DimCube { get; set; }

        [AllowHtml]
        public string DimComment { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime UpdateOn { get; set; }
        public string UpdateBy { get; set; }
    }
}
