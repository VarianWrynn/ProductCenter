using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;

namespace PermaisuriCMS.BLL
{
    public class MenuServices
    {
        /// <summary>
        /// Get All Menu List
        /// </summary>
        /// <returns></returns>
        public List<Menu_Resource_Model> GetMenuList(Menu_Resource_Model qModel, out int count)
        {
            List<Menu_Resource_Model> list = new List<Menu_Resource_Model>();
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.Menu_Resource.AsQueryable();
                if (!String.IsNullOrEmpty(qModel.name))
                {
                    query = query.Where(s => s.MenuName.Contains(qModel.name));
                }
                count = query.Count();
                query = query.OrderBy(s => s.SortNo).Skip((qModel.page - 1) * qModel.rows).Take(qModel.rows);
                foreach (var m in query)
                {
                    list.Add(new Menu_Resource_Model
                    {
                        //icon = m.Icon,
                        MenuID = m.MenuID,
                        ParentMenuID = m.ParentMenuID,
                        MenuUrl = m.MenuUrl,
                        MR_ID = m.MR_ID,
                        name = m.MenuName,
                        MenuName =m.MenuName,
                        SortNo = m.SortNo,
                        Visible = m.Visible
                    });
                }
                return list;
            }
        }

        public List<Menu_Resource_Model> GetAllMenuList()
        {
            List<Menu_Resource_Model> list = new List<Menu_Resource_Model>();
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                //var query = db.GetAllMenuList();
                var query = db.Menu_Resource.AsEnumerable();
                foreach (var m in query)
                {
                    list.Add(new Menu_Resource_Model
                    {
                        //icon = m.Icon,
                        iconSkin = m.Icon,
                        MenuID = m.MenuID,
                        ParentMenuID = m.ParentMenuID,
                        MenuUrl = m.MenuUrl,
                        MR_ID = m.MR_ID,
                        name = m.MenuName,
                        MenuName=m.MenuName,
                        SortNo = m.SortNo,
                        Visible = m.Visible
                    });
                }
                return list;
            }
        }

        public bool AddRole(Menu_Resource_Model model, String curUserAccount)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var newRole = new Menu_Resource
                {
                   // Icon = model.icon,
                    Memo = model.Memo,
                    MenuID = model.MenuID,
                    ParentMenuID = model.ParentMenuID,
                    MenuUrl = model.MenuUrl,
                    MenuName = model.MenuName,
                    SortNo = model.SortNo,
                    Visible = model.Visible,
                    Created_By = curUserAccount,
                    Created_On = DateTime.Now
                };
                db.Menu_Resource.Add(newRole);
                return db.SaveChanges() > 0;
            }
        }

        public bool DeleteUser(Menu_Resource_Model model)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var user = new Menu_Resource { MR_ID = model.MR_ID };
                db.Set<Menu_Resource>().Attach(user);
                db.Menu_Resource.Remove(user);
                return db.SaveChanges() > 0;
            }
        }

        public bool UpdateRoleInUser(string[] MR_IDs, Guid role_guid)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    db.Database.ExecuteSqlCommand("delete from Role_Menu_Relation where Role_GUID = @role_guid", new SqlParameter("@role_guid", role_guid));
                    foreach (string mr_id in MR_IDs)
                    {
                        var newRelation = new Role_Menu_Relation
                        {
                            MR_ID = Convert.ToInt32(mr_id),
                            Role_GUID = role_guid
                        };
                        db.Role_Menu_Relation.Add(newRelation);
                    }
                    int retInt = db.SaveChanges();
                    transaction.Complete();// don't be miss,or the SQL statement will never be executed
                    return retInt > 0;
                }
            }
        }

        public bool DeleteAllRoleByUser(Guid role_guid)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                return db.Database.ExecuteSqlCommand("delete from Role_Menu_Relation where Role_GUID = @Role_GUID", new SqlParameter("@role_guid", role_guid)) > 0;
            }
        }

		public bool isExistMenu(int MR_ID) {
			using (PermaisuriCMSEntities db = new PermaisuriCMSEntities()) {
				var query = db.Menu_Resource.Where(a => a.MR_ID == MR_ID).FirstOrDefault();
				return query == null ? false : true; 
			}
		}

		public bool EditMenu(Menu_Resource_Model model,String modeifer) {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var entity = db.Menu_Resource.Where(a => a.MR_ID == model.MR_ID).FirstOrDefault();
                entity.MenuName = model.MenuName;
                entity.ParentMenuID = model.ParentMenuID;
                entity.SortNo = model.SortNo;
                entity.Icon = model.iconSkin;
                entity.Memo = model.Memo;
                entity.MenuUrl = model.MenuUrl;
                entity.Visible = model.Visible;
                entity.Modified_By = modeifer;
                entity.Modified_On = System.DateTime.Now;
                return db.SaveChanges() > 0;
            }
		}
    }
}
