using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model.Users
{
    /// <summary>
    /// 1
    /// </summary>
    [Serializable]
    public partial class User_Profile_Ext_Model
    {
        public User_Profile_Ext_Model()
        { }
        #region Model
        private Guid _userext_guid;
        private string _title;
        private string _home_phone;
        private string _street1;
        private string _street2;
        private string _street3;
        private string _city;
        private string _state;
        private string _postal_code;
        private string _country;
        private string _on_leave;
        private DateTime? _leave_start_date;
        private DateTime? _leave_end_date;
        private int? _leave_seq_no;
        private string _second_emaik;
        /// <summary>
        /// 
        /// </summary>
        public Guid UserExt_Guid
        {
            set { _userext_guid = value; }
            get { return _userext_guid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Home_Phone
        {
            set { _home_phone = value; }
            get { return _home_phone; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Street1
        {
            set { _street1 = value; }
            get { return _street1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Street2
        {
            set { _street2 = value; }
            get { return _street2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Street3
        {
            set { _street3 = value; }
            get { return _street3; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string City
        {
            set { _city = value; }
            get { return _city; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string State
        {
            set { _state = value; }
            get { return _state; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Postal_Code
        {
            set { _postal_code = value; }
            get { return _postal_code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Country
        {
            set { _country = value; }
            get { return _country; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string On_Leave
        {
            set { _on_leave = value; }
            get { return _on_leave; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Leave_Start_Date
        {
            set { _leave_start_date = value; }
            get { return _leave_start_date; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Leave_End_Date
        {
            set { _leave_end_date = value; }
            get { return _leave_end_date; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? Leave_Seq_No
        {
            set { _leave_seq_no = value; }
            get { return _leave_seq_no; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Second_Emaik
        {
            set { _second_emaik = value; }
            get { return _second_emaik; }
        }
        #endregion Model

    }
}
