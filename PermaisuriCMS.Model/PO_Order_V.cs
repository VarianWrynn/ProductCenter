using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// User-Role-Realation ship table. 
    /// </summary>
    [Serializable]
    public partial class PO_Order_V_Model
    {
        public PO_Order_V_Model()
        { }
        #region Model
        private int _orders;
        private DateTime _order_date;
        private String _merchantID;
        /// <summary>
        /// 
        /// </summary>
        public int Orders
        {
            set { _orders = value; }
            get { return _orders; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime OrderDate
        {
            set { _order_date = value; }
            get { return _order_date; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String MerchantID
        {
            set { _merchantID = value; }
            get { return _merchantID; }
        }
        #endregion Model

    }
}
