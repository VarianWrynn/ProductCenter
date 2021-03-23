using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model.Reports
{

    /// <summary>
    /// Sales By Product报表返回的信息，包含了分页列表内容和统计（total）信息。为什么用之前的ref int count的方式？
    /// 原因是因为现在返回的是多列，如果列超过5个，不仅编码看起来丑陋，以后维护也是一团糟。
    /// CreatedTime:2014年2月10日18:53:53
    /// </summary>
    public class ReportByProducttWithStatisModel
    {
        public List<ReportByProductModel> ReportList { get; set; }

        public int Count { get; set; }

        public string SumSalesTotal { get; set; }

        public int SumUnitsSold { get; set; }

        public string SumAvgAmont { get; set; }
    }

    /// <summary>
    /// 报表选择"Sales by Product"的时候返回的对象
    /// •	A new report will be created that is historical sales of individual products (SKUs) by Channel
    /// •	This new report will not include any cost or margin data
    /// •	The "Units Sold" and "Total Sales" columns will be included in this report
    /// •	The date range fields will be available for this report
    /// •	The SKU search/filter field will be available for this report（Melissa,David,2014年1月15日11:32:04)
    /// </summary>
    public class ReportByProductModel
    {
        public string SKUOrder { get; set; }
        public string ProductName { get; set; }
        public String SalesTotal { get; set; }
        public int UnitsSold { get; set; }
        public string AvgAmont { get; set; }
        /// <summary>
        /// thumbnail image
        /// 经过Melissa David讨论后，这张报表取自Ecom的销售信息，不同渠道的SKUOrder都集合在一起统计其销售量，所以不能给出其图....
        /// ChangeDate:2014年1月15日11:07:37
        /// </summary>
        //public string Thumbnail { get; set; }
    }

}
