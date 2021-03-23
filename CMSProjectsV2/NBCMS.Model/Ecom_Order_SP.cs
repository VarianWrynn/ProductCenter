using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model
{
    /// <summary>
    ///  符合HightCharts格式的强类型对象返回
    /// </summary>
    public class Ecom_Order_SP_Model
    {
        public string name { get; set; }
        //OrderItem orderItem { get; set; }
        public DateTime x { get; set; }
        public long y { get; set; }
    }

    ///// <summary>
    ///// HighChart 单个item格式
    ///// </summary>
    //public class OrderItem {
    //    //DateTime OrderDate { get; set; }
    //    //Int32 Orders { get; set; }
    //    long x { get; set; }
    //    long y { get; set; }
    //}
}
