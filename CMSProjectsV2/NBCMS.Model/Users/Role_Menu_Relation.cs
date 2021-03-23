using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model.Users
{
    /// <summary>
    /// Role_Menu_Relation:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Role_Menu_Relation_Model
    {
        public Role_Menu_Relation_Model()
        { }
        #region Model
        private int _id;
        private int _mr_id;
        private Guid _role_guid;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int MR_ID
        {
            set { _mr_id = value; }
            get { return _mr_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid Role_GUID
        {
            set { _role_guid = value; }
            get { return _role_guid; }
        }
        #endregion Model

    }
}
