using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;


namespace Permaisuri.Controllers
{
    public class RolesController : Controller
    {

        public ActionResult RolesManagement()
        {
            return View();
        }

        /// <summary>
        /// Get Role List from DB
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRoleList(Security_Role_Model qModel)
        {
            try
            {
                var count = 0;
                var riSvg = new RoleInfoServices();
                var list = riSvg.GetRoleList(qModel, out count);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = list.Count,
                        rows = list
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }


        public ActionResult AddRole(Security_Role_Model model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request is illegal!"
                    });
                }

                if (model.Role_Name == null)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Role Name cannot be empty!"
                    });
                }

                string cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
                var serializer = new JavaScriptSerializer();
                string decCookies = CryptTools.Decrypt(cookis);
                User_Profile_Model curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;

                RoleInfoServices iSvr = new RoleInfoServices();

                //user is exiting
                if (iSvr.IsExistUser(model.Role_Name))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "this role is exiting"
                    });
                }

                if (iSvr.AddRole(model, curUserInfo.User_Account))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Successfully add user"
                    });
                }
                else
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "faile to add new user"
                    });
                }
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }


        public ActionResult DeleteRole(Security_Role_Model model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request is illegal!"
                    });
                }

                if (model.Role_GUID == null)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request paramter is null!"
                    });
                }

                string cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
                var serializer = new JavaScriptSerializer();
                string decCookies = CryptTools.Decrypt(cookis);
                User_Profile_Model curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
                RoleInfoServices uiSvr = new RoleInfoServices();

                if (uiSvr.DeleteRole(model))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Successfully delete role"
                    });
                }
                else
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "faile to delete role"
                    });
                }
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

		public ActionResult EditRole(Security_Role_Model model) {
			try {
				if (model == null) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Request is illegal!"
					});
				}

				if (model.Role_GUID == null) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Request paramter is null!"
					});
				}
				string cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
				var serializer = new JavaScriptSerializer();
				string decCookies = CryptTools.Decrypt(cookis);
				User_Profile_Model curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
				RoleInfoServices uiSvr = new RoleInfoServices();
				if (uiSvr.EditRole(model,curUserInfo.User_Account)) {
					return Json(new NBCMSResultJson {
						Status = StatusType.OK,
						Data = "Successfully edit role"
					});
				}
				else {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "faile to edit role"
					});
				}
			}
			catch (Exception ex) {
				return Json(new NBCMSResultJson {
					Status = StatusType.OK,
					Data = ex.Message
				});
			}
		}



        /// <summary>
        /// Get data for Role Modeling to displaying Role's Menu
        /// </summary>
        /// <param name="Role_Guid">Role_Guid request  from client</param>
        /// <returns>json:Correspond with jQuery easyUI tree-grid format (if have children, included _parentId)</returns>
        public ActionResult RoleMenusLoading(String Role_Guid)
        {
            try
            {
                if (string.IsNullOrEmpty(Role_Guid))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request is illegal!"
                    });
                }

                string cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
                var serializer = new JavaScriptSerializer();
                string decCookies = CryptTools.Decrypt(cookis);
                User_Profile_Model curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;

                RoleInfoServices rSvg = new RoleInfoServices();
                List<Object> result = new List<object>();
                foreach (Menu_Resource_Model m in rSvg.GetAllMenusWithUser(new Guid(Role_Guid)))
                {
                    if (m.ParentMenuID == "0")
                    {
                        result.Add(new
                        {
                           // icon = m.icon,
                            MenuID = m.MenuID,
                            ParentMenuID = m.ParentMenuID,
                            MenuUrl = m.MenuUrl,
                            MR_ID = m.MR_ID,
                            //name = m.MenuName, 2014年2月8日11:54:18
                            name =m.name,
                            SortNo = m.SortNo,
                            Visible = m.Visible,
                            Memo = m.Memo,
                            Role_Checked =m.Role_Checked

                        });
                    }
                    else
                    {
                        result.Add(new
                        {
                            //icon = m.icon,
                            MenuID = m.MenuID,
                            _parentId = m.ParentMenuID,//tree-grid must have those formate...
                            ParentMenuID = m.ParentMenuID,
                            MenuUrl = m.MenuUrl,
                            MR_ID = m.MR_ID,
                            //name = m.MenuName,
                            name = m.name,
                            SortNo = m.SortNo,
                            Visible = m.Visible,
                            Memo = m.Memo,
                            Role_Checked = m.Role_Checked
                        });
                    }
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = result.Count,
                        rows = result
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }



        public ActionResult UserRoleUpdate(string Role_Guid)
        {
            try
            {
                string sRoleses = Request["menuGuids[]"];
                if (string.IsNullOrEmpty(Role_Guid))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request is illegal!"
                    });
                }

                string cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
                var serializer = new JavaScriptSerializer();
                string decCookies = CryptTools.Decrypt(cookis);
                User_Profile_Model curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
                MenuServices mSvr = new MenuServices();
                if (sRoleses == null)
                {
                    if (mSvr.DeleteAllRoleByUser(new Guid(Role_Guid)))
                    {
                        return Json(new NBCMSResultJson
                        {
                            Status = StatusType.OK,
                            Data = "Successfully update user's role"
                        });
                    }
                    else
                    {
                        return Json(new NBCMSResultJson
                        {
                            Status = StatusType.Error,
                            Data = "faile to update user's role"
                        });
                    }
                }
                string[] arrRoles = sRoleses.Split(',');
                if (mSvr.UpdateRoleInUser(arrRoles, new Guid(Role_Guid)))
                {

                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Successfully update Role's menu"
                    });
                }
                else
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "faile to update Role's menu"
                    });
                }
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }
    }
}
