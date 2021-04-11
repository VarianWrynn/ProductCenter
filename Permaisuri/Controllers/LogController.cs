using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Model;
using PermaisuriCMS.Common;
using System.Configuration;
using System.IO;
using System.Text;

namespace Permaisuri.Controllers
{
    //http://www.w3cschool.cc/bootstrap/bootstrap-tutorial.html 2014年11月12日
    public class LogController : Controller
    {
        //
        // GET: /Log/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LogManager()
        {
            return View();
        }

        public ActionResult GetLogOfUserLogin(LogOfUserLogin_Model queryModel, String order)
        {
            try
            {
                var logSrv = new LogServices();
                var count = 0;
                var list = logSrv.GetLoginInfoList(queryModel, out count);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = count,
                        rows = list
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }


        public ActionResult GetLogOfUserOperating(LogOfUserOperating_Model queryModel)
        {
            try
            {
                var logSrv = new LogServices();
                var count = 0;
                var list = logSrv.GetLogOfUserOperatingList(queryModel, out count);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = count,
                        rows = list
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// 获取所有的日志文件的列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllLogList(int page, int rows, string getType)
        {
            var sLogPath = "";
            switch (getType)
            {
                case "SystemLogFiles":
                    sLogPath = System.Web.HttpContext.Current.Server.MapPath("~/Log/");
                    break;
                case "SKUSynchFiles":
                    sLogPath = ConfigurationManager.AppSettings["HMSynchLogPath"];
                    break;
                default:
                    break;
            }
            if (Directory.Exists(sLogPath))
            {
                var files =
                    new DirectoryInfo(sLogPath)
                        .GetFiles("*", SearchOption.TopDirectoryOnly)
                        .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                        .Select(f => new
                        {
                            //fileName = f.FullName,
                            name = f.Name,
                            length = f.Length,
                            CreationTime = f.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            LastWriteTime = f.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                        }).OrderByDescending(f => f.LastWriteTime).Skip((page - 1)*rows).Take(rows);

                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = files.Count(),
                        rows = files
                    }
                });
            }
            else
            {
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = 0,
                        rows = new {}
                    }
                });
            }
        }

        /// <summary>
        /// 下载查看点击的文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="getType">请求类型，目前分为CMS log文件 和 WindowServices自动运行产生的服务</param>
        public void DownLoadFile(string fileName, string getType)
        {

            var filePath = "";
            switch (getType)
            {
                case "SystemLogFiles":
                    filePath = System.Web.HttpContext.Current.Server.MapPath("~/Log/");
                    break;
                case "SKUSynchFiles":
                    filePath = ConfigurationManager.AppSettings["HMSynchLogPath"];
                    break;
            }
            filePath = filePath + fileName;
            //var filePath = System.Web.HttpContext.Current.Server.MapPath("~/Log/") + fileName;
            if (System.IO.File.Exists(filePath))
            {
                
                //日志文件，不定时都可能由另外的程序对它进行日志记录写入操作,这样的情况，不单要与只读方式打开txt文件，
                //而且，需要共享锁。还必须要选择flieShare方式为ReadWrite。因为随时有其他程序对其进行写操作。 
                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
               // StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                var sr = new StreamReader(fs, Encoding.Unicode);
                //Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                fileName = HttpUtility.UrlEncode(fileName, Encoding.UTF8);    //对中文文件名进行HTML转码
                //byte[] buffer = Encoding.UTF8.GetBytes(sr.ReadToEnd());
                byte[] buffer = Encoding.Unicode.GetBytes(sr.ReadToEnd());

                fs.Close();
                fs.Dispose();
                sr.Close();
                sr.Dispose();

                //byte[] outBuffer = new byte[buffer.Length + 3];
                //outBuffer[0] = (byte)0xEF;//有BOM,解决乱码
                //outBuffer[1] = (byte)0xBB;
                //outBuffer[2] = (byte)0xBF;
                //Array.Copy(buffer, 0, outBuffer, 3, buffer.Length);
                //char[] cpara = Encoding.UTF8.GetChars(outBuffer); // byte[] to char[]
                //char[] cpara = Encoding.UTF8.GetChars(buffer);
                char[] cpara = Encoding.Unicode.GetChars(buffer);
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
                Response.ContentType = "application/octet-stream";
                //Response.ContentEncoding = Encoding.UTF8;
                Response.ContentEncoding = Encoding.Unicode;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(cpara, 0, cpara.Length);
                Response.End();
            }
            else
                Response.StatusCode = 404;
        }



        public ActionResult GetLogOfUserOperatingDetails(LogOfUserOperatingDetails_Model queryModel)
        {
            try
            {
                var logSrv = new LogServices();
                int count;
                var list = logSrv.GetLogOfUserOperatingDetails(queryModel, out count);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = count,
                        rows = list
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }
    }
}
