using System;
using System.Web.Mvc;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using System.Data;
using System.Text;
using PermaisuriCMS.Model;

namespace Permaisuri.Areas.Excel.Controllers
{
// ReSharper disable InconsistentNaming
    public class NPOIController : Controller
// ReSharper restore InconsistentNaming
    {
        //
        // GET: /Excel/NPOI/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult NpoiImport()
        {
            try
            {
                var dataTable =
                    ExcelHelper.ImportExceltoDt(@"D:\Permaisuri\Permaisuri\Permaisuri\MediaLib\销售任务跟踪查询.xlsx", 0, 1);

                //return Json(new
                //{
                //    dataTable  // 直接使用这种方式会报错！
                //});

                return Json(new
                {
                    str = DataTableToJson1(dataTable)
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = ex.Message
                });
            }
        }


        ///<summary>
        ///方法1    这个方法其实就是DataTable拼接成字符串而已，没什么出奇的
        ///</summary>
        ///<param name="dt"></param>
        ///<returns></returns>
        private static string DataTableToJson1(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return "";
            }

            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j]);
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }




        public ActionResult LoadWithUserProfilerWithLinq()
        {
            try
            {
                return Json(new
                {
                    str = new UserInfoServices().GetUserListWithLinq(new User_Profile_Model
                    {
                        User_Account = "123",
                        User_Pwd = "1234"
                    })
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    msg = ex.Message
                });
            }
           
        }

    }
}
