using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Model;
using PermaisuriCMS.Common;
using System.Configuration;
using System.Web.Script.Serialization;

namespace Permaisuri.Controllers {
	public class UsersController : Controller {

		// private static Logger log = LogManager.GetCurrentClassLogger();
		/// <summary>
		/// User Management Model 
		/// </summary>
		/// <returns></returns>
		public ActionResult UserManagement() {
            var cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
            var serializer = new JavaScriptSerializer();
            var decCookies = CryptTools.Decrypt(cookis);
            var curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
            ViewBag.AllUserStatus = serializer.Serialize(new UserInfoServices().GetAllUserStatus());
            //通过ViewBag动态获取Channnel列表数据 2013年10月31日16:12:53 Lee
            ViewBag.ChannelList = serializer.Serialize(new ProductCommonServices().GetAllChannels(curUserInfo != null && curUserInfo.IsChannelControl, curUserInfo.User_Guid));
            return View();
		}


        /// <summary>
        /// Get User Info List
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public ActionResult GetUserList(User_Profile_QueryModel queryModel)
        {
            try
            {
                var uiSvg = new UserInfoServices();

                var count = 0;
                var list = uiSvg.GetUserList(queryModel, out count);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = count,
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

		/// <summary>
		/// Add New User
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public ActionResult AddUser(User_Profile_Model model) {
			try {
				if (model == null) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Request is illegal!"
					});
				}

				if (model.User_Account == null) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Request paramter is null!"
					});
				}

				var cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
				var serializer = new JavaScriptSerializer();
				var decCookies = CryptTools.Decrypt(cookis);
				var curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
				var uiSvr = new UserInfoServices();
				//user is exiting
				if (uiSvr.IsExistUser(model.User_Account)) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "user account is exiting"
					});
				}

				if (uiSvr.AddUser(model, curUserInfo.User_Account)) {
					return Json(new NBCMSResultJson {
						Status = StatusType.OK,
						Data = "Successfully add user"
					});
				}
			    return Json(new NBCMSResultJson {
			        Status = StatusType.Error,
			        Data = "faile to add new user"
			    });
			}
			catch (Exception ex) {
				NBCMSLoggerManager.Fatal(ex.Message);
				NBCMSLoggerManager.Fatal(ex.StackTrace);
				NBCMSLoggerManager.Error("");
				return Json(new NBCMSResultJson {
					Status = StatusType.Exception,
					Data = ex.Message
				});
			}
		}

		/// <summary>
		/// Edit a User
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public ActionResult EditUser(User_Profile_Model model) {
			try {
				if (model == null) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Request is illegal!"
					});
				}

			    var cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
				var serializer = new JavaScriptSerializer();
				var decCookies = CryptTools.Decrypt(cookis);
				var curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;

				var uiSvr=new UserInfoServices();
				if (uiSvr.EditUser(model,curUserInfo.User_Account)) {
					return Json(new NBCMSResultJson {
						Status = StatusType.OK,
						Data = "Successfully edit user"
					});
				}
				else {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Faile to edit user"
					});
				}
			}
			catch (Exception ex) {
				NBCMSLoggerManager.Fatal(ex.Message);
				NBCMSLoggerManager.Fatal(ex.StackTrace);
				NBCMSLoggerManager.Error("");
				return Json(new NBCMSResultJson {
					Status = StatusType.Exception,
					Data = ex.Message
				});
			}
		}

		/// <summary>
		/// Delete a user by guid
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public ActionResult DeleteUser(User_Profile_Model model) {
			try {
				if (model == null) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Request is illegal!"
					});
				}

			    var cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
				var serializer = new JavaScriptSerializer();
				var decCookies = CryptTools.Decrypt(cookis);
				var uiSvr = new UserInfoServices();

				if (uiSvr.DeleteUser(model)) {
					return Json(new NBCMSResultJson {
						Status = StatusType.OK,
						Data = "Successfully delete user"
					});
				}
				else {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "faile to delete user"
					});
				}
			}
			catch (Exception ex) {
				NBCMSLoggerManager.Fatal(ex.Message);
				NBCMSLoggerManager.Fatal(ex.StackTrace);
				NBCMSLoggerManager.Error("");
				return Json(new NBCMSResultJson {
					Status = StatusType.Exception,
					Data = ex.Message
				});
			}
		}

		/// <summary>
		/// Get Role List by User
		/// </summary>
		/// <returns></returns>
		public ActionResult GetRoleInUser(String user_guid) {
			try {
				if (string.IsNullOrEmpty(user_guid)) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Request is illegal!"
					});
				}
				var cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
				var serializer = new JavaScriptSerializer();
				var decCookies = CryptTools.Decrypt(cookis);
				var curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
				var uiSvr = new UserInfoServices();
				var list = uiSvr.GetAllRolesWithUser(new Guid(user_guid));
				return Json(new NBCMSResultJson {
					Status = StatusType.OK,
					Data = list
				});
			}
			catch (Exception ex) {
				NBCMSLoggerManager.Fatal(ex.Message);
				NBCMSLoggerManager.Fatal(ex.StackTrace);
				NBCMSLoggerManager.Error("");
				return Json(new NBCMSResultJson {
					Status = StatusType.Exception,
					Data = ex.Message
				});
			}
		}


		public ActionResult UpdateRoleInUser(String User_Guid) {
			try {
				var sRoleses = Request["ArrRoles[]"];
				if (string.IsNullOrEmpty(User_Guid)) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Request is illegal!"
					});
				}

				var cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
				var serializer = new JavaScriptSerializer();
				var decCookies = CryptTools.Decrypt(cookis);
				var rSvr = new RoleInfoServices();
				if (sRoleses == null)
				{
				    if (rSvr.DeleteAllRoleByUser(new Guid(User_Guid))) {
						return Json(new NBCMSResultJson {
							Status = StatusType.OK,
							Data = "Successfully update user's role"
						});
					}
				    return Json(new NBCMSResultJson {
				        Status = StatusType.Error,
				        Data = "faile to update user's role"
				    });
				}
			    var arrRoles = sRoleses.Split(',');
				if (rSvr.UpdateRoleInUser(arrRoles, new Guid(User_Guid))) {

					return Json(new NBCMSResultJson {
						Status = StatusType.OK,
						Data = "Successfully update user's role"
					});
				}
			    return Json(new NBCMSResultJson {
			        Status = StatusType.Error,
			        Data = "faile to update user's role"
			    });
			}
			catch (Exception ex) {
				NBCMSLoggerManager.Fatal(ex.Message);
				NBCMSLoggerManager.Fatal(ex.StackTrace);
				NBCMSLoggerManager.Error("");
				return Json(new NBCMSResultJson {
					Status = StatusType.Exception,
					Data = ex.Message
				});
			}
		}

        /// <summary>
        /// 触发CMS和WebPO的账号进行数据同步。由于一开始CMS设置了自己的用户信息，并且使用GUID进行关联。
        /// 后来要求用户统一在WEBPO进行设置,而WebPO采用自增长而非GUID的方式记录数据。因此需要采取一种
        /// 同步机制而不是简单的View视图关联....
        /// 
        /// Author:Lee Date:2013年10月22日11:56:03
        /// </summary>
        /// <returns></returns>
        public ActionResult UserSynchWithWebPo()
        {
            try
            {
                var cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
                var serializer = new JavaScriptSerializer();
                var decCookies = CryptTools.Decrypt(cookis);
                var curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;

                var uSvr = new UserInfoServices();
                var affectedRows = 0;
                if (curUserInfo != null) uSvr.UserSynchWithWebPO(curUserInfo.User_Account, out affectedRows);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = "Synchronizing Successfully"
                });
                //if (retVal > 0)
                //{
                //    return Json(new NBCMSResultJson
                //    {
                //        Status = StatusType.OK,
                //        Data = "Synchronizing Successfully" 
                //    });
                //}
                //else
                //{
                //    return Json(new NBCMSResultJson
                //    {
                //        Status = StatusType.Error,
                //        Data = "There is no data need to synchronizes"
                //    });
                //}
            }
            catch (Exception ex)//如果存储过程内部发生错，就跳到这里来了。。。。无数据返回-1
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Fatal(ex.Source);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }



        public ActionResult Test()
        {
            return View();
        }


        public ActionResult UpdateUserStats(short Status,Guid User_Guid)
        {
            try
            {
                //string cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
                //var serializer = new JavaScriptSerializer();
                //string decCookies = CryptTools.Decrypt(cookis);
                //User_Profile_Model curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;

                var uSvr = new UserInfoServices();
                if (uSvr.UpdateUserStats(Status, User_Guid))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Current user allowe to login CMS system"
                    });
                }
                else
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "fail to update this item!"
                    });
                }

            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// 更新当前用户是否启用Channel关联，用于UserManagement-OpenChannel的操作
        /// Author:Lee,Date:2013年11月2日12:12:27
        /// </summary>
        /// <param name="User_Guid"></param>
        /// <param name="IsChannelControl"></param>
        /// <returns></returns>
        public ActionResult UpdateUserChannelControl(Guid User_Guid, bool IsChannelControl)
        {
            try
            {
                var uSvr = new UserInfoServices();
                if (uSvr.UpdateUserChannelControl(User_Guid, IsChannelControl))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "OK"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = "fail to update this item!"
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        ///  通过GUID来获取当前用户对于的渠道信息，用于User Management 模块的 Channels Settings使用
        ///  Date: 2013年11月1日16:06:14 Author:Lee
        /// </summary>
        /// <param name="User_Guid"></param>
        /// <returns></returns>
        public ActionResult GetChannelByGuid(Guid User_Guid)
        {
            try
            {
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new UserInfoServices().GetChannelByGuid(User_Guid)
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }


        /// <summary>
        /// 更新当前用户对于的Channel信息，用户User Management模块的Channel Setting
        /// Author:Lee Date:2013年11月1日18:06:15
        /// </summary>
        /// <param name="User_Guid"></param>
        /// <param name="ArrChannels"></param>
        /// <returns></returns>
        public ActionResult UpdateUserChannel(Guid User_Guid, IEnumerable<int> ArrChannels)
        {
            try
            {
                var uSvr = new UserInfoServices();
                var isPudated = uSvr.UpdateUserChannel(User_Guid,ArrChannels);
                return Json(new NBCMSResultJson
                {
                    Status =isPudated? StatusType.OK:StatusType.Error,
                    Data =  isPudated?"Current user have been associated channels successfully":"failed to update"
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }


		///// <summary>
		///// Just for test ,pls delete before deployment
		///// </summary>
		///// <returns></returns>
		//public ActionResult GetMenuListForTreeGrid()
		//{
		//    try
		//    {
		//        RoleInfoServices Svg = new RoleInfoServices();
		//        List<Object> result = new List<object>();
		//        foreach (Menu_Resource_Model m in mSvg.GetAllMenuList())
		//        {
		//            if (m.ParentMenuID == "0")
		//            {
		//                result.Add(new
		//                {
		//                    icon = m.icon,
		//                    MenuID = m.MenuID,
		//                    ParentMenuID = m.ParentMenuID,
		//                    MenuUrl = m.MenuUrl,
		//                    MR_ID = m.MR_ID,
		//                    name = m.MenuName,
		//                    SortNo = m.SortNo,
		//                    Visible = m.Visible
		//                });
		//            }
		//            else
		//            {
		//                result.Add(new
		//                {
		//                    icon = m.icon,
		//                    MenuID = m.MenuID,
		//                    _parentId = m.ParentMenuID,//tree-grid must have those formate...
		//                    ParentMenuID = m.ParentMenuID,
		//                    MenuUrl = m.MenuUrl,
		//                    MR_ID = m.MR_ID,
		//                    name = m.MenuName,
		//                    SortNo = m.SortNo,
		//                    Visible = m.Visible
		//                });
		//            }
		//        }

		//        return Json(new
		//        {
		//            total = result.Count,
		//            rows = result
		//            //Status = StatusType.OK,
		//            //Data = new
		//            //{
		//            //    total = result.Count,
		//            //    rows = result
		//            //}
		//        });
		//    }
		//    catch (Exception ex)
		//    {
		//        NBCMSLoggerManager.Fatal(ex.Message);
		//        NBCMSLoggerManager.Fatal(ex.StackTrace);
		//        NBCMSLoggerManager.Error("");
		//        return Json(new NBCMSResultJson
		//        {
		//            Status = StatusType.Exception,
		//            Data = ex.Message
		//        });
		//    }
		//}
	}
}
