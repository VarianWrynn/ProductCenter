using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model.Reports
{

    /// <summary>
    /// Channel报表返回的信息，包含了分页列表内容和统计（total）信息。为什么用之前的ref int count的方式？
    /// 原因是因为现在返回的是多列，如果列超过5个，不仅编码看起来丑陋，以后维护也是一团糟。
    /// CreatedTime:2014年2月10日11:04:40
    /// </summary>
    public class ChannelReportWithStatisModel
    {
        public List<ChannelReportModel> ReportList { get; set; }

        public int Count { get; set; }

        public string SumSalesTotal { get; set; }

        public int SumUnitsSold { get; set; }

        public string SumAvgAmont { get; set; }
    }


    /// <summary>
    /// 报表选择"Sales by Channel"的时候返回的对象,不包含统计信息
    /// </summary>
    public class ChannelReportModel
    {
        /// <summary>
        /// Name of sales Channel
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// ID for sales Channel
        /// </summary>
        public int ChannelID { get; set; }

        /// <summary>
        /// Amount of total sales in $ for sales Channel, within range
        /// </summary>
        public String SalesTotal { get; set; }

        /// <summary>
        /// Number of sold items included for Channel, within range
        /// </summary>
        public int UnitsSold { get; set; }

        ///// <summary>
        ///// Cost of goods for sales for Channel, within range
        ///// </summary>
        //public int COG { get; set; }

        ///// <summary>
        ///// Profit % for sales for Channel, within range
        ///// </summary>
        //public int Margin { get; set; }

        public string AvgAmont { get; set; }
    }


}
