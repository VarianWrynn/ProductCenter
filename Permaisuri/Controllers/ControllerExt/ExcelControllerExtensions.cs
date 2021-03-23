using System.Web.Mvc;

namespace Permaisuri.Controllers
{
    public static class ExcelControllerExtensions
    {
        /// <summary>
        /// to load CSV version - entire result, no paging
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ActionResult ExcelCsv(this Controller controller, string content,string fileName, string contentType)
        {
            return new ExcelResult(content, fileName, contentType);
        }
    }
}