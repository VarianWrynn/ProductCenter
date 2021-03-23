using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers {
	public class MenusController : Controller {
		//
		// GET: /Menus/

		public ActionResult MenusManagement() {
			return View();
		}



        /// <summary>
        /// Get Menu List from DB
        /// </summary>
        /// <param name="qModel"></param>
        /// <returns></returns>
        public ActionResult GetMenuList(Menu_Resource_Model qModel)
        {
			try {
				int count = 0;
				MenuServices mSvg = new MenuServices();
                List<Menu_Resource_Model> list = mSvg.GetMenuList(qModel, out count);

				List<Object> result = new List<object>();
				foreach (Menu_Resource_Model m in list) {
					if (m.ParentMenuID == "0") {
						result.Add(new {
							//icon = m.icon,
                            iconSkin = m.iconSkin,
							MenuID = m.MenuID,
							ParentMenuID = m.ParentMenuID,
							MenuUrl = m.MenuUrl,
							MR_ID = m.MR_ID,
							name = m.MenuName,
							SortNo = m.SortNo,
							Visible = m.Visible
						});
					}
					else {
						result.Add(new {
							//icon = m.icon,
                            iconSkin = m.iconSkin,
							MenuID = m.MenuID,
							_parentId = m.ParentMenuID,
							ParentMenuID = m.ParentMenuID,
							MenuUrl = m.MenuUrl,
							MR_ID = m.MR_ID,
							name = m.MenuName,
							SortNo = m.SortNo,
							Visible = m.Visible
						});
					}
				}

				return Json(new NBCMSResultJson {
					Status = StatusType.OK,
					Data = new {
						total = result.Count,
						rows = result
					}
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

		public ActionResult GetMenuListForTreeGrid() {
			try {
				MenuServices mSvg = new MenuServices();
				List<Object> result = new List<object>();
				foreach (Menu_Resource_Model m in mSvg.GetAllMenuList()) {
					if (m.ParentMenuID == "0") {
						result.Add(new {
							//icon = m.icon,
                            iconSkin = m.iconSkin,
							MenuID = m.MenuID,
							ParentMenuID = m.ParentMenuID,
							MenuUrl = m.MenuUrl,
							MR_ID = m.MR_ID,
							name = m.MenuName,
							SortNo = m.SortNo,
							Visible = m.Visible
						});
					}
					else {
						result.Add(new {
							//icon = m.icon,
                            iconSkin = m.iconSkin,
							MenuID = m.MenuID,
							_parentId = m.ParentMenuID,//tree-grid must have those formate...
							ParentMenuID = m.ParentMenuID,
							MenuUrl = m.MenuUrl,
							MR_ID = m.MR_ID,
							name = m.MenuName,
							SortNo = m.SortNo,
							Visible = m.Visible
						});
					}
				}

				return Json(new NBCMSResultJson {
					Status = StatusType.OK,
					Data = new {
						total = result.Count,
						rows = result
					}
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

		public ActionResult UpdateMenu(Menu_Resource_Model model) {
			try {
				if (model == null) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Request is illegal!"
					});
				}

				if (model.MR_ID == 0) {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
						Data = "Request paramter is null!"
					});
				}
				string cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
				var serializer = new JavaScriptSerializer();
				string decCookies = CryptTools.Decrypt(cookis);
				User_Profile_Model curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
				MenuServices mns = new MenuServices();
                if (mns.EditMenu(model, curUserInfo.User_Account))
                {
					return Json(new NBCMSResultJson {
						Status = StatusType.OK,
						Data = "Successfully edit menu"
					});
				}
				else {
					return Json(new NBCMSResultJson {
						Status = StatusType.Error,
                        Data = "faile to edit menu"
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
	}
}
