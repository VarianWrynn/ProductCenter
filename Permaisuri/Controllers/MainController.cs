using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PermaisuriCMS.Model;
using PermaisuriCMS.BLL;
using NLog;
using System.Configuration;
using PermaisuriCMS.Common;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Permaisuri.Controllers
{
   
    public class MainController : Controller
    {
        //
        // GET: /Main/

        public ActionResult Index()
        {
            string cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
            //用来阻止第一次登陆的时候没有Cookies触发的解密异常报错 2014年2月22日
            if (string.IsNullOrEmpty(cookis))
            {
                return null;
            }
           
            User_Profile_Model userInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
            return View(userInfo);
        }

        /// <summary>
        /// Handle and Display MenuTree by User Role
        /// </summary>
        /// <param name="model">User_Profile_Model</param>
        /// <returns>json foramt data</returns>
        [HttpPost]
        public ActionResult GetMenuTrees()
        {
            try
            {
                User_Profile_Model model = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                UserInfoServices uiSvs = new UserInfoServices();
                List<Menu_Resource_Model> list = uiSvs.GetMenuResourceByUserInfo(model);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = list.ToArray()
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        public ActionResult UserLogout()
        {
            try
            {
                string cookiName = ConfigurationManager.AppSettings["userInfoCookiesKey"];
                string cookis = Request[cookiName];
                var serializer = new JavaScriptSerializer();
                string decCookies = CryptTools.Decrypt(cookis);

                User_Profile_Model userInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
                UserInfoServices userInfoSvr = new UserInfoServices();
                //userInfoSvr.UpdateUserStats("1", userInfo.User_Account);

                //remove方法只是不让服务器向客户机发送那个被删除的cookie，与此cookie留不留在客户机里无关
                Response.Cookies.Remove(cookiName);
                Response.Cookies[cookiName].Expires = DateTime.Now.AddDays(-1);
                //FormsAuthentication.SignOut();

               // HttpContext.Response.Redirect("~/Login/Index");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = "User Logout successfully!"
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }
    }
}
