using System.Web.Mvc;
using NLog;

namespace Permaisuri.Filters
{
    public class NbcmsExceptionsAttribute : HandleErrorAttribute
    {
        private static readonly Logger Log = LogManager.GetLogger("NBCMSExceptionsAttribute");
        /// <summary>
        /// Notice if you have done the "try catch" method on your page,this filter does not execute
        /// http://www.codeproject.com/Articles/731913/Exception-Handling-in-MVC
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            var ex = filterContext.Exception;
            Log.Error(ex.Message);
            Log.Error(ex.StackTrace);

            // if the request is AJAX return JSON else view.
            if (IsAjax(filterContext))
            {
                //Because its a exception raised after ajax invocation
                //Lets return Json
                filterContext.Result = new JsonResult()
                {
                    Data = filterContext.Exception.Message,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
            }
            else
            {
                //Normal Exception
                //So let it handle by its default ways.
                base.OnException(filterContext);

            }
            base.OnException(filterContext);

            // Write error logging code here if you wish.

            //if want to get different of the request
            //var currentController = (string)filterContext.RouteData.Values["controller"];
            //var currentActionName = (string)filterContext.RouteData.Values["action"];

        }


        private bool IsAjax(ExceptionContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}