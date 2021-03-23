using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers.ControllerExt
{
    public class UserProfilerBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.Model != null)
                throw new InvalidOperationException("Cannot update instances");

            var reqDecCookies = controllerContext.HttpContext.Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
            //if (reqDecCookies.Trim() == string.Empty) reqDecCookies is null, Trim() will trigger null reference object! 2014年5月20日
            if (string.IsNullOrEmpty(reqDecCookies))
            {
                //执行的顺序是: IModelerBinder-->Filter-->controller,action.所以直接返回null可以的
                //controllerContext.HttpContext.Response.Redirect("~/Login/Index");
                return null;
            }

            var serializer = new JavaScriptSerializer();
            var decCookies = CryptTools.Decrypt(reqDecCookies);
            var userCache = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;

            //if (userCache == null)
            //{
            //    //controllerContext.HttpContext.Response.Redirect("~/Login/Index");
            //    return null;
            //}
            //return userCache;

            return userCache ?? null;
        }
    }
}