using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCMS.Model.Users
{
    /// <summary>
    /// User-Role-Realation ship table. 
    /// </summary>
    [Serializable]
    public partial class User_Role_Relation_Model
    {
        public User_Role_Relation_Model()
        { }
        #region Model
        private int _id;
        private Guid _user_guid;
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
        public Guid User_Guid
        {
            set { _user_guid = value; }
            get { return _user_guid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid Role_Guid
        {
            set { _role_guid = value; }
            get { return _role_guid; }
        }
        #endregion Model

    }
}
