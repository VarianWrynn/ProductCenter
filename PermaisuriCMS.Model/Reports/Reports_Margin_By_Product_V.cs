using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// 新增的报表类型
    /// David Eike: why is historical data necessary? Isn't it just Margin = (Retail - COG - Freight) for each line item (times units sold)?
    /// David Eike: In other words, an estimate based on current costing
    /// David Eike: right, it is not a guarantee of accuracy
    /// Lee.Wong: we just retrieve the laest data?
    /// David Eike: yeah, this is not an accounting application
    /// David Eike: just a guide to measure pricing strategies
    /// CreateDate:2014年1月14日15:20:16
    /// </summary>
    public partial class Reports_Margin_By_Product_V_Model
    {
        public string SKUOrder { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string ChannelName { get; set; }
        public int ChannelID { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public int Inventory { get; set; }
        public decimal NormalSelling { get; set; }
        public decimal COG { get; set; }
        public decimal Margin { get; set; }
    }
}
