using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PermaisuriCMS.Model.HMNUM
{
    /// <summary>
    /// 箱规 CTN是carton的简写 纸箱就是纸箱的编号。
    /// </summary>
    public class CMS_ProductCTN_Model
    {
        public long CTNID { get; set; }
        public long ProductID { get; set; }
        public string HMNUM { get; set; }
        public string CTNTitle { get; set; }
        public decimal CTNLength { get; set; }
        public decimal CTNWidth { get; set; }
        public decimal CTNHeight { get; set; }

        /// <summary>
        /// 相纸重量，2013年12月13日10:39:43
        /// </summary>
        public decimal CTNWeight { get; set; }
        public decimal CTNCube { get; set; }

        [AllowHtml]
        public string CTNComment { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime UpdateOn { get; set; }
        public string UpdateBy { get; set; }
    }
}
