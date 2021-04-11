using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.Model;
using PermaisuriCMS.Common;

namespace Permaisuri.Filters
{
    /// <summary>
    /// Verify User Identity
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class UserAuthAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //return;

            //拿到当前请求的URL地址
            var requestUrl = filterContext.HttpContext.Request.Path;

            //对Login页面不做过滤,不能用StartWith,如果设置为二级目录，//localhost:1234/CMS/Login/Index，这时候就彻底悲剧了！
            //if (requestUrl.StartsWith("/Login/", StringComparison.InvariantCultureIgnoreCase))
            if (requestUrl.ToUpper().Contains("/LOGIN/"))
            {
                return;
            }

            //get the ClientCookies;
            /*Login的时候出现Network error错误是因为这里coolis获取到有值，而页面恰好处于/CMS/这样的请求中...解决方法：全部关闭Browser再登陆OK
              2013年10月15日12:56:38  Lee
             */
            var cookis = filterContext.HttpContext.Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
            if (String.IsNullOrEmpty(cookis))
            {
                filterContext.HttpContext.Response.Redirect("~/Login/Index");
                return;
            }

            //如果是默认的地址，强制转向Main页面，原因是为了seajs的客户端“./”需要明确的获取到当前的controller/action,否则会报错
            if (String.CompareOrdinal(requestUrl, "/") == 0 && !String.IsNullOrEmpty(cookis))
            {
                filterContext.HttpContext.Response.Redirect("~/Main/Index");
                return;
            }
           
            var serializer = new JavaScriptSerializer();
            var decCookies = CryptTools.Decrypt(cookis);
            var curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
            // curUserInfo = filterContext.HttpContext.Session["userInfo"] as User_Profile_Model;
            if (curUserInfo == null)
            {
                filterContext.HttpContext.Response.Redirect("~/Login/Index");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var actionName = filterContext.ActionDescriptor.ActionName;
            if (actionName.ToLower().Contains("update") || actionName.ToLower().Contains("insert") || actionName.ToLower().Contains("delete") ||
                actionName.ToLower().Contains("remove") || actionName.ToLower().Contains("add") || actionName.ToLower().Contains("eidt")||
                actionName.ToLower().Contains("synchron"))
            {
                //get the ClientCookies;
                var cookis = filterContext.HttpContext.Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
                if (String.IsNullOrEmpty(cookis))
                {
                    filterContext.HttpContext.Response.Redirect("~/Login/Index");
                    return;
                }

                var serializer = new JavaScriptSerializer();
                var decCookies = CryptTools.Decrypt(cookis);
                var curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;

                IDictionary<string, string> ls = new Dictionary<string, string>();
                if (curUserInfo != null)
                {
                    ls.Add("User_Account", curUserInfo.User_Account);
                    ls.Add("Display_Name", curUserInfo.Display_Name);
                }
                ls.Add("Model_Name", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
                ls.Add("Action_Name", filterContext.ActionDescriptor.ActionName);
                ls.Add("IP_Address", filterContext.HttpContext.Request.UserHostAddress);
                ls.Add("Operating_Date", filterContext.HttpContext.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                NBCMSLoggerManager.NBCMSLogger("Operating", "", ls);
            }

            base.OnActionExecuted(filterContext);

        }
    }
}