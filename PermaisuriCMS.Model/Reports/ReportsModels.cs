using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// 报表查询类
    /// </summary>
    public class ReportsModel
    {
        public int page { get; set; }
        public int rows { get; set; }

        public int Channel { get; set; }
        public int Brand { get; set; }

        /// <summary>
        /// 1:-Sales by Channel
        /// 2:-Sales by Product
        /// 3:-Product Development Report
        /// 4:-Low Inventory Report
        /// </summary>
        public int ReportType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        /// <summary>
        /// ReportType=3 Product Devel Reports的时候用到
        /// </summary>
        public int QueueStatus { get; set; }

        /// <summary>
        /// ReportType=4 Low Inventory Reports的时候用到
        /// </summary>
        public String AffectedSKU { get; set; }

        /// <summary>
        /// 1-order by  Channel;
        /// 2-order by Sales Totals;
        /// 3-order by Units Sold;
        /// 4-order by Avg. Sale Amount;
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// 0-asc;
        /// 1-desc
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// 0- 查询,默认
        /// 1-到处CVS格式 
        /// Author:Lee
        /// Date:2013年10月16日12:02:48
        /// </summary>
        public int ActionType { get; set; }

        /// <summary>
        /// Sale By Product现在开放SKUOrder/ProductName模糊查询 （Melissa,David)
        /// CreateDate:2014年1月15日10:43:49
        /// </summary>
        public String SKUOrder { get; set; }
        public String ProductName { get; set; }

        /// <summary>
        /// 2014年1月15日16:32:50 我的表情--> 囧~
        /// </summary>
        public String HMNUM { get; set; }
        public String HMName { get; set; }
    }

  
    /// <summary>
    /// 报表选择"Low inventory Report"的时候返回的对象
    /// </summary>
    public class ReportLowInventoryModel
    {
        /// <summary>
        /// HM Number of Part
        /// </summary>
        public string Part { get; set; }

        /// <summary>
        /// Actual number of avilible items based on parts
        /// </summary>
        public int Inventory { get; set; }

        /// <summary>
        /// Projected quantity of next replenishment
        /// </summary>
        public int ExpectedInventory { get; set; }

        /// <summary>
        /// Date of expected replenishment
        /// Change:Change to Nullable to fit new requirement.
        /// </summary>
        public Nullable<DateTime> NextShipment { get; set; }

        /// <summary>
        /// The "Next Shipment" column sometimes shows invalid dates. If there is no scheduled shipment, this column value should be "n/a" or " -- "
        /// </summary>
        public String StringNextShipment { get; set; }

        public string pImage { get; set; }

        /// <summary>
        /// Comma-delimited list of SKU's affected,Linked to Product
        /// </summary>
        public List<String> AffectedSKUS { get; set; }

        public IQueryable<String> AffectedSKUS_Temp { get; set; }
    }

    /// <summary>
    /// 报表选择"Product Develoment Report"的时候返回的对象
    /// </summary>
    public class ReportProductDevelopmentModel
    {

        /// <summary>
        /// SKU+Name(if avalible,otherwise "New Product")
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Brand name for this product,if avalible
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Channel name for this Product,if available
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Queue Status
        /// </summary>
        public int QueueStatus { get; set; }

        /// <summary>
        /// Date of last status change
        /// </summary>
        public string LastUpdated { get; set; }


        /*
         * For the requirements:
         * In the Product Development report, can the product names link to the Product Configuration page for the SKU? (David)
         * Date:2013年10月19日11:08:49
         */
        public string SKUOrder { get; set; }
        public int ChannelID { get; set; }

        public long SKUID { get; set; }

    }


    /// <summary>
    /// d.	Add new report: Low Inventory by SKU
    /// i.	Same columns as Low Inventory by HM but listed by SKU instead of part number
    /// ii.	Add column for channel name to SKU inventory report
    /// Author Lee, Date:2013年10月14日16:37:48
    /// </summary>
    public class ReportLowInventoryBySKUModel
    {
        public string SKU { get; set; }

        public string ChannelName { get; set; }

        /// <summary>
        /// Actual number of avilible items based on parts
        /// </summary>
        public int Inventory { get; set; }

        /// <summary>
        /// Projected quantity of next replenishment
        /// </summary>
        public int ExpectedInventory { get; set; }

        /// <summary>
        /// Date of expected replenishment
        /// Change:Change to Nullable to fit new requirement 2014年1月21日11:51:36
        /// </summary>
        public Nullable<DateTime> NextShipment { get; set; }

        public String StringNextShipment { get; set; }

        public string pImage { get; set; }

        /// <summary>
        /// Comma-delimited list of SKU's affected,Linked to Product
        /// </summary>
        public List<String> AffectedSKUS { get; set; }

        public IQueryable<String> AffectedSKUS_Temp { get; set; }
    }
}
