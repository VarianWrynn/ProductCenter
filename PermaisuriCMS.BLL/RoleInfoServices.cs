using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;

namespace PermaisuriCMS.BLL
{
    public class RoleInfoServices
    {
        /// <summary>
        /// Get Role List by pager
        /// </summary>
        /// <returns></returns>
        public List<Security_Role_Model> GetRoleList(Security_Role_Model qModel, out int count)
        {
            List<Security_Role_Model> list = new List<Security_Role_Model>();
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.Security_Role.AsQueryable();
                if (!String.IsNullOrEmpty(qModel.Role_Name))
                {
                    query = query.Where(s => s.Role_Name.Contains(qModel.Role_Name));
                }
                count = query.Count();
                query = query.OrderByDescending(s=>s.Created_On).Skip((qModel.page - 1) * qModel.rows).Take(qModel.rows);
                foreach (var r in query)
                {
                    list.Add(new Security_Role_Model
                    {
                        Role_GUID = r.Role_GUID,
                        Role_Name = r.Role_Name,
                        Role_Desc = r.Role_Desc
                    });
                }
                return list;
            }
        }


        public bool IsExistUser(String role_Name)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.Security_Role.Where(o => o.Role_Name == role_Name).FirstOrDefault();
                return query == null ? false : true;
            }
        }

        public bool AddRole(Security_Role_Model model, String curUserAccount)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var newRole = new Security_Role
                {
                    Role_GUID = Guid.NewGuid(),
                    Role_Name = model.Role_Name,
                    Created_By = curUserAccount,
                    Created_On = DateTime.Now,
                    Role_Desc = model.Role_Desc
                };
                db.Security_Role.Add(newRole);
                return db.SaveChanges() > 0;
            }
        }

        public bool DeleteRole(Security_Role_Model model)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var role = new Security_Role { Role_GUID = model.Role_GUID };
                db.Set<Security_Role>().Attach(role);
                db.Security_Role.Remove(role);
                return db.SaveChanges() > 0;
            }
        }

		public bool EditRole(Security_Role_Model model,string curUserAccount) {
			using (PermaisuriCMSEntities db = new PermaisuriCMSEntities()) {
				var entity = db.Security_Role.Where(a => a.Role_GUID == model.Role_GUID).FirstOrDefault();
				entity.Role_Name = model.Role_Name;
				entity.Role_Desc = model.Role_Desc;
				entity.Modified_By = curUserAccount;
				entity.Modified_On = DateTime.Now;
				return db.SaveChanges()>0;
			}
		}

        /// <summary>
        /// 赋予/撤销用户权限算法
        /// </summary>
        /// <param name="role_guid">需要赋予当前用户什么权限</param>
        /// <param name="user_guid">当前用户唯一标识</param>
        /// <returns></returns>
        public bool UpdateRoleInUser(string[] role_guid, Guid user_guid)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                /***
                 * var tranny = DbContext.Database.Connection.BeginTransaction();
                 * tranny.Commit();
                 ***/
                using (TransactionScope transaction = new TransactionScope())
                {
                    db.Database.ExecuteSqlCommand("delete from User_Role_Relation where User_Guid = @User_Guid", new SqlParameter("@User_Guid", user_guid));
                    foreach (string role_guids in role_guid)
                    {
                        var newRelation = new User_Role_Relation
                        {
                            Role_Guid = new Guid(role_guids),
                            User_Guid = user_guid
                        };
                        db.User_Role_Relation.Add(newRelation);
                    }
                    int retInt = db.SaveChanges();
                    transaction.Complete();// don't be miss,or the SQL statement will never be executed
                    return retInt > 0;
                }
            }
        }

        public bool DeleteAllRoleByUser(Guid user_guid)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                return db.Database.ExecuteSqlCommand("delete from User_Role_Relation where User_Guid = @User_Guid", new SqlParameter("@User_Guid", user_guid)) > 0;
            }
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <param name="User_Guid"></param>
        /// <returns></returns>
        public List<Menu_Resource_Model> GetAllMenusWithUser(Guid Role_GUID)
        {
            List<Menu_Resource_Model> list = new List<Menu_Resource_Model>();
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var menus = db.Menu_Resource.Where(m=>m.Visible==true).OrderBy(m=>m.SortNo);
                foreach (var m in menus)
                {
                    var rmr =m.Role_Menu_Relation.Where(r => r.Role_GUID == Role_GUID).FirstOrDefault();
                    list.Add(new Menu_Resource_Model
                    {
                        MR_ID = m.MR_ID,
                        MenuID = m.MenuID,
                        //icon=m.Icon,
                        name =m.MenuName,
                        ParentMenuID = m.ParentMenuID,
                        Role_Checked = rmr == null ? false : true
                    });
                }
            }
            return list;
        }


        private bool IsHaveChild(string pid, List<Menu_Resource_Model> menuList)
        {
            var result = menuList.Where(l => l.ParentMenuID == pid).FirstOrDefault();
            return result == null ? false : true;
        }


        public List<Object> GetTree()
        {
            List<Menu_Resource_Model> menuList = GetAllMenusWithUser(new Guid());
            List<Object> objList = new List<object>();

            foreach (var m in menuList)
            {
                if (IsHaveChild(m.MenuID, menuList))
                {
                    objList.Add(new
                    {
                        id = m.MR_ID,
                        text = m.MenuName,
                        children = GetTree()
                    });
                }
                else
                {
                    objList.Add(new
                    {
                        id = m.MR_ID,
                        text = m.MenuName
                    });
                }
            }

            return objList;
        }
    }
}
