using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ActionFilterAttribute = System.Web.Mvc.ActionFilterAttribute;

//using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace Permaisuri.Filters
{
    /*
     * http://stackoverflow.com/questions/12606202/system-web-mvc-actionfilterattribute-vs-system-web-http-filters-actionfilterattr
     * 
     * System.Web.Mvc.ActionFilterAttribute and System.Web.Http.Filters.ActionFilterAttribute
     * what is different?
     * 
     * 
     * The System.Web.Http one is for Web API; the System.Web.Mvc one is for previous MVC versions.
     * You can see from the source that the Web API version has several differences.
     * It has OnResultExecuting and OnResultExecuted handlers ("Called by the ASP.NET MVC framework before/after the action result executes.")
     * It can be executed asynchronously
     * It does not let you specify an order of execution
     * 
     */

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class AuditLogAttribute : ActionFilterAttribute
    {
        private int _relatedId;
        private string _target = string.Empty;
        private string _content = string.Empty;
        private readonly AuditLogEnum _actionType;


        public int RelatedId
        {
            get { return _relatedId; }
            set { _relatedId = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }


        public string Target
        {
            get { return _target; }
            set { _target = value; }
        }


        public AuditLogAttribute(AuditLogEnum actionType)
        {
            _actionType = actionType;
        }

         //OnActionExecuted执行完之后才开始执行 OnResultExecuted
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
           
            //var routeData =  filterContext.RequestContext.RouteData.Values;
            var nameValueParams = filterContext.HttpContext.Request.Params;
            //var currentEmployee = EmployeeAuthorization.Current.User == null ?
            //    CommonAppService.Current.ValidateUser("WX", nameValueParams["userName"], nameValueParams["password"]) : EmployeeAuthorization.Current.User.Id;

            var isSuccess = true;
            var faiReason = string.Empty;
            var actionContent = filterContext.Result as ContentResult;
            if (actionContent != null)
            {
                if (actionContent.Content != null)
                {
                    if (actionContent.Content.Contains("\"Errors\""))
                    {
                        isSuccess = false;
                        faiReason = actionContent.Content.Replace("\\", "");
                    }
                }
            }
            var content = "";
            switch (_actionType)
            {
                case AuditLogEnum.CustomerCreated:
                    content = String.Format(AuditLogEnum.CustomerCreated.ToTest(), nameValueParams["name"]);
                    break;
                case AuditLogEnum.CustomerUpdated:
                    //logId
                    if (actionContent == null || actionContent.Content == null)
                    {
                        base.OnResultExecuted(filterContext);
                        return;
                    }
                    var serializer = new JavaScriptSerializer();
                    //var customer = serializer.Deserialize<CustomerSaleModifyDto>(actionContent.Content);
                    //if (customer == null)
                    //{
                    //    base.OnResultExecuted(filterContext);
                    //    return;
                    //}
                    //_relatedId = customer.LogId;
                    //content = string.Format(AuditLogEnum.CustomerUpdated.ToTest(), customer.Name);
                    break;
                case AuditLogEnum.EmployeeLogOut:
                    break;
                case AuditLogEnum.EmployeeLogin:
                    //content = String.Format(AuditLogEnum.EmployeeLogin.ToTest(), OrgAppService.Current.GetEmployee(currentEmployee).Name);
                    break;
                case AuditLogEnum.EmployeeStatusUpdated:
                    //var modifyName = OrgAppService.Current.GetEmployee(currentEmployee).Name;
                    //var employeeName = OrgAppService.Current.GetEmployee(new Guid(nameValueParams["id"])).Name;
                    content = String.Format(AuditLogEnum.EmployeeStatusUpdated.ToTest(), "", "", nameValueParams["position"]);
                    Target = nameValueParams["id"];
                    break;
                case AuditLogEnum.EmployeeStatusUpdatedBy:
                    //var employee = OrgAppService.Current.GetEmployee(new Guid(nameValueParams["id"])).Name;
                    content = String.Format(AuditLogEnum.EmployeeStatusUpdatedBy.ToTest(), "", nameValueParams["position"]);
                    break;
                default:
                    break;
            }
            //_employeeActionLogService.Create(new EmployeeActionLog
            //{
            //    ActionType = (int)_actionType,
            //    Content = content == "" ? Content : content,
            //    EmployeeId = currentEmployee,
            //    RelateId = RelatedId,
            //    Target = Target,
            //    IsSuccess = isSuccess,
            //    FaiReason = faiReason,
            //    CreateTime = DateTime.Now
            //});

            base.OnResultExecuted(filterContext);
        }
    }



     public enum AuditLogEnum
     {
         CustomerCreated = 11,
         CustomerUpdated = 12,

         EmployeeLogin = 21,
         EmployeeLogOut = 22,

         /// <summary>
         /// 主动修改某个顾问
         /// </summary>
         EmployeeStatusUpdated = 23,

         /// <summary>
         /// 某个顾问被修改
         /// </summary>
         EmployeeStatusUpdatedBy = 24
     }

     public static partial class EnumExtensions
     {
         public static string ToTest(this AuditLogEnum value)
         {
             switch (value)
             {
                 case AuditLogEnum.CustomerCreated:
                     return "新增用户:{0}";

                 case AuditLogEnum.CustomerUpdated:
                     return "更改用户 {0} 信息";

                 case AuditLogEnum.EmployeeLogin:
                     return "顾问 {0} 登录成功";

                 case AuditLogEnum.EmployeeLogOut:
                     return "";

                 case AuditLogEnum.EmployeeStatusUpdated:
                     return "{0}修改顾问 {1} 的状态为:{2}";

                 case AuditLogEnum.EmployeeStatusUpdatedBy:
                     return "顾问{0}状态被修改为:{1}";

                 default:
                     return "";
             }
         }
     }

}