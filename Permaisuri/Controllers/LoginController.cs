using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using PermaisuriCMS.Model;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using System.Web.Script.Serialization;
using System.Configuration;

namespace Permaisuri.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }


        public ActionResult Index3()
        {
            return View();
        }

        /// <summary>
        /// Validate the user authen. MVC可以根据前端传递进来的parameter初始化 user_Profiler对象。
        /// Change1:由于新增了ModelBinder的绑定也是用User_Profile_Model作为强类型对象返回，会和这里的冲突，改用参数传递 2014年5月20日11:47:36
        /// </summary>
        /// <param name="userAccount">user_Profiler Modle</param>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckUserLogin(string userAccount, string userPwd)
        {
            IDictionary<string, string> ls = new Dictionary<string, string>();
            var user = new User_Profile_Model
            {
                User_Account = userAccount,
                User_Pwd = userPwd
            };
            ls.Add("User_Account", user.User_Account);
            ls.Add("Logging_IP", HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
            ls.Add("Machine_Name", HttpContext.Request.UserHostName);
            ls.Add("Logging_Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            try
            {
                //NBCMSLoggerManager.Error( "123".GetPWDMD5());

                var userInfoSvr = new UserInfoServices();
                var userInfo = userInfoSvr.checkUserLogin(user);
                string remarkMsg;
                if (userInfo != null)
                {
                    if (userInfo.UserStatusID != 1)
                    {
                        remarkMsg = "User does not allow to login CMS system";
                        ls.Add("Display_Name", user.Display_Name);
                        ls.Add("LoggingStatue", "1");
                        ls.Add("Remark", remarkMsg);
                        NBCMSLoggerManager.NBCMSLogger("Login", "", ls);
                        return Json(new NBCMSResultJson
                        {
                            Status = StatusType.Error,
                            Data = remarkMsg
                        });
                    }
                    //setting cookies
                    var serializer = new JavaScriptSerializer();
                    //String userInfoCookies = serializer.Serialize(new
                    //{
                    //    User_Guid = userInfo.User_Guid,
                    //    User_Account = userInfo.User_Account,
                    //    Display_Name = userInfo.Display_Name,
                    //    User_Pwd = userInfo.User_Pwd,
                    //    IsChannelControl = userInfo.IsChannelControl//add by Lee 2013-11-4
                    //});
                    var userInfoCookies = serializer.Serialize(new
                    {
                        userInfo.User_Guid,
                        userInfo.User_Account,
                        userInfo.Display_Name, 
                        userInfo.User_Pwd, 
                        userInfo.IsChannelControl//add by Lee 2013-11-4
                    });
                    //userInfoSvr.UpdateUserStats("1", userInfo.User_Account);

                    //Add by Lee  2013年12月2日10:49:38  just for testing
                    //FormsAuthentication.SetAuthCookie(userInfo.User_Account, false);

                    var keys = ConfigurationManager.AppSettings["userInfoCookiesKey"];
                    var cookie = new HttpCookie(keys, CryptTools.Encrypt(userInfoCookies));
                    //不设置Cookies过期时间，让Cookies伴随浏览器关闭就自动结束  
                    //DateTime dt = DateTime.Now;
                    //TimeSpan ts = new TimeSpan(0, 0, 0, secExpires, 0);//过期时间为??秒钟
                    //cookie.Expires = dt.Add(ts);//设置过期时间
                    Response.AppendCookie(cookie);
                    //Session["UserInfo"] = userInfo;

                    //新增功能：记录用户最后一次登录的时间

                    ls.Add("Display_Name", userInfo.Display_Name);
                    ls.Add("LoggingStatue", "0");
                    NBCMSLoggerManager.NBCMSLogger("Login", "", ls);

                    userInfoSvr.UpdateUserLast_Logon(userInfo.User_Guid);

                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = userInfo
                    });
                }
                remarkMsg = "User does not exist or password is wrong";
                ls.Add("Display_Name", user.Display_Name);
                ls.Add("LoggingStatue", "1");
                ls.Add("Remark", remarkMsg);
                NBCMSLoggerManager.NBCMSLogger("Login", "", ls);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = remarkMsg
                });

            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = ex.Message
                });
            }
        }
        
    }
}
