using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model.Reports
{


    /// <summary>
    /// Channel报表返回的信息，包含了分页列表内容和统计（total）信息。为什么用之前的ref int count的方式？
    /// 原因是因为现在返回的是多列，如果列超过5个，不仅编码看起来丑陋，以后维护也是一团糟。
    /// CreatedTime:2014年2月11日11:00:40
    /// </summary>
    public class ReportByHMWithStatisModel
    {
        public List<ReportByHMModel> ReportList { get; set; }

        public int Count { get; set; }

        public string SumSalesTotal { get; set; }

        public int SumUnitsSold { get; set; }

        public string SumAvgAmont { get; set; }
    }



    /// <summary>
    /// 报表选择"Sales by HM#"的时候返回的对象
    /// </summary>
    public class ReportByHMModel
    {
        /// <summary>
        /// add HMNUM 2013-10-14
        /// </summary>
        public string HMNUM { get; set; }
        public string HMName { get; set; }

        public Boolean IsGroup { get; set; }

        public String SalesTotal { get; set; }
        public int UnitsSold { get; set; }
        public string AvgAmont { get; set; }

    }
}
