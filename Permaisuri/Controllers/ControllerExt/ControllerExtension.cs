using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Permaisuri.Controllers.ControllerExt;

namespace Permaisuri.Controllers
{
    /// <summary>
    /// 对于Controller层的方法扩展
    /// </summary>
    public static class ControllerExtension
    {
        /// <summary>
        /// MD5 Hash
        /// </summary>
        /// <param name="password">user password</param>
        /// <returns></returns>
        public static string HashPasswordForStoringInConfigFile(this string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            var encoding = new UTF8Encoding();
            var hashedBytes = MD5.Create().ComputeHash(encoding.GetBytes(password));
            return encoding.GetString(hashedBytes);
        }


        public static string HashPasswordWithAscii(this string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            var encoding = new ASCIIEncoding();
            var hashedBytes = MD5.Create().ComputeHash(encoding.GetBytes(password));
            return encoding.GetString(hashedBytes);
        }

        /// <summary>
        /// 该方法与过期的方法FormsAuthentication.HashPasswordForStoringInConfigFile(user.User_Pwd, "MD5");一样的作用.
        /// 但是如果传递进来的字符串（password)是汉字，则不一样。
        /// Author: Lee ; Date:2013年10月23日11:31:14 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetPwdmdWithdefault(this string value)
        {
            var algorithm = MD5.Create();
            var data = algorithm.ComputeHash(Encoding.Default.GetBytes(value));
            return data.Aggregate("", (current, t) => current + t.ToString("x2").ToUpperInvariant());
        }

        /// <summary>
        ///  该方法与过期的方法FormsAuthentication.HashPasswordForStoringInConfigFile(user.User_Pwd, "MD5");一样的作用
        ///  即使传递进来的字符串是汉字
        ///  Author: Lee ; Date:2013年10月23日11:31:14 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetPwdmd5(this string value)
        {
            var algorithm = MD5.Create();
            var data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            return data.Aggregate("", (current, t) => current + t.ToString("x2").ToUpperInvariant());
        }

        /// <summary>
        /// Extension methods for the controller to allow jsonp.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static JsonpResult Jsonp(this Controller controller, object data)
        {
            var result = new JsonpResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
        }
    }
}