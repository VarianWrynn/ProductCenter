using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PermaisuriCMS.Common;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;

namespace Permaisuri.Areas.LomoPrinter.Controllers
{
    public class APIController : Controller
    {
        private const string PrinterUrl = "http://api.lomoment.com/v2/int/int_ext/";
        //
        // GET: /LomoPrinter/API/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DoPrintTask(PermaisuriCMS.Model.LomoPrinter lomoPrinter)
        {
            try
            {
                var newPrinter =  DoSignature(lomoPrinter);
                var keyvalues = new Dictionary<string, string>
                {
                    {"terminal_id", newPrinter.terminal_id.ToString(CultureInfo.InvariantCulture)},
                    {"nonce", newPrinter.nonce.ToString(CultureInfo.InvariantCulture)},
                    {"timestamp", newPrinter.timestamp.ToString(CultureInfo.InvariantCulture)},
                    {"signature", newPrinter.signature},
                    {"job_img_url", newPrinter.job_img_url}
                };

                var pUrl = GetRequestUrl(keyvalues, "job_submit.php?");
                var httpBase = new HttpBase();
                //var responseStr = httpBase.GetResponseText(pUrl, "GET", "");
                return Json(new
                {
                    str = "暂时注释掉"
                    //responseStr
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


        public ActionResult PrintTaskQuery(PermaisuriCMS.Model.LomoPrinter lomoPrinter)
        {
            try
            {
                var newPrinter = DoSignature(lomoPrinter);
                var keyvalues = new Dictionary<string, string>
                {
                    {"terminal_id", newPrinter.terminal_id.ToString(CultureInfo.InvariantCulture)},
                    {"nonce", newPrinter.nonce.ToString(CultureInfo.InvariantCulture)},
                    {"timestamp", newPrinter.timestamp.ToString(CultureInfo.InvariantCulture)},
                    {"signature", newPrinter.signature},
                    {"job_id", newPrinter.job_id.ToString(CultureInfo.InvariantCulture)}
                };

                var pUrl = GetRequestUrl(keyvalues, "job_status.php?");
                var httpBase = new HttpBase();
                var responseStr = httpBase.GetResponseText(pUrl, "GET", "");
                return Json(data: new
                {
                    responseStr
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

        /// <summary>
        /// 更新机器设置
        /// </summary>
        /// <param name="lomoPrinter"></param>
        /// <param name="terminal_free_rcode"></param>
        /// <returns></returns>
        public ActionResult DoPrinterSetting(PermaisuriCMS.Model.LomoPrinter lomoPrinter, string terminal_free_rcode)
        {
            try
            {
                var newPrinter = DoSignature(lomoPrinter);
                var keyvalues = new Dictionary<string, string>
                {
                    {"terminal_id", newPrinter.terminal_id.ToString(CultureInfo.InvariantCulture)},
                    {"nonce", newPrinter.nonce.ToString(CultureInfo.InvariantCulture)},
                    {"timestamp", newPrinter.timestamp.ToString(CultureInfo.InvariantCulture)},
                    {"signature", newPrinter.signature},
                    {"terminal_free_rcode", "88888"}
                };
                var pUrl = GetRequestUrl(keyvalues, "terminal_settings_set.php?");

                var httpBase = new HttpBase();
               // var responseStr = httpBase.GetResponseText(pUrl, "GET", "");
                return Json(data: new
                {
                    //responseStr
                    str = "暂时注释掉"
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



        public ActionResult DoPrintQuery(PermaisuriCMS.Model.LomoPrinter lomoPrinter)
        {
            try
            {
                var newPrinter = DoSignature(lomoPrinter);
                var keyvalues = new Dictionary<string, string>
                {
                    {"terminal_id", newPrinter.terminal_id.ToString(CultureInfo.InvariantCulture)},
                    {"nonce", newPrinter.nonce.ToString(CultureInfo.InvariantCulture)},
                    {"timestamp", newPrinter.timestamp.ToString(CultureInfo.InvariantCulture)},
                    {"signature", newPrinter.signature}
                };

                var pUrl = GetRequestUrl(keyvalues, "terminal_settings_get.php?");
                var httpBase = new HttpBase();
                var responseStr = httpBase.GetResponseText(pUrl, "GET", "");
                return Json(data: new
                {
                    responseStr
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


        /// <summary>
        /// 根据传递进来的终端ID和token,调用Sha-1签名并返回
        /// </summary>
        /// <param name="lomoPrinter"></param>
        /// <returns></returns>
        private static PermaisuriCMS.Model.LomoPrinter DoSignature(PermaisuriCMS.Model.LomoPrinter lomoPrinter)
        {
            var rd = new Random();
            var nonce = rd.Next(100, 10000);
            lomoPrinter.nonce = nonce;

            var timestamp = CMSUtilityTools.DateTime2Unix(DateTime.Now);
            lomoPrinter.timestamp = timestamp;

            var str1 = lomoPrinter.terminal_id.ToString() + lomoPrinter.terminal_token +
                       lomoPrinter.nonce.ToString() + lomoPrinter.timestamp.ToString();

            //建立SHA1对象
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                var enc = new ASCIIEncoding();
                var dataToHash = enc.GetBytes(str1);

                //Hash运算
                var dataHashed = sha1.ComputeHash(dataToHash);

                //将运算结果转换成string
                var signature = BitConverter.ToString(dataHashed).ToLower().Replace("-", "");

                lomoPrinter.signature = signature;
                // Releases all resources used by the System.Security.Cryptography.HashAlgorithm.
                sha1.Clear();
            }

            return lomoPrinter;

        }

        private static string GetRequestUrl(IReadOnlyCollection<KeyValuePair<string, string>> dictonary,string relativeUrl)
        {

            var postData = string.Empty;
            if (dictonary.Count > 0)
            {
                postData = string.Join("&",
                    (from kvp in dictonary
                     let item = kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value)
                     select item
                        ).ToArray()
                    );
            }
            var pUrl = PrinterUrl + relativeUrl + postData;
            return pUrl;
        }
    }
}
