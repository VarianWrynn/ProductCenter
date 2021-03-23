using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Permaisuri.Filters;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    public class HtmlContentsController : Controller
    {
        private const string ZmengDomain = "http://www.zmeng123.com/web/";
        private const string TuoZongFansUrl = "http://www.tuoz.net/index.php?g=Home&m=Zmapi&a=shop_info";


        // GET: /HTMLContents/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetValidateCode()
        {
            return null;
        }


        [AuditLog(AuditLogEnum.EmployeeStatusUpdated)]
        [AuditLog(AuditLogEnum.EmployeeStatusUpdatedBy)]
        public ActionResult LoginTzPlateform(TZLoginParams tzParams)
        {
            try
            {
                /*
                string url = _domain + "login_action.php";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Cookie", "code=af1bf71c1ff9aeeddfa949ab5ec511cc");
                request.ServicePoint.Expect100Continue = false;//去掉 100-continue 的表头 2010-12-23
                string user = tzParams.UserName; //用户名
                string pass = tzParams.password; //密码

                //request.Headers.Add("Cookie", "code=306c2cec58824fa27069016627b4d50f");
                //string data = "username=云程科技&password=123456&check=6&put_submit=";//如果用Wirter 字符串传递的方式，一定要把汉字先Encode掉！
                string data = "username=%E4%BA%91%E7%A8%8B%E7%A7%91%E6%8A%80&action_flag=&password=123456&check=8&put_submit=";

                request.ContentLength = data.Length;
                //StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);//报错：Bytes to be written to the stream exceed the Content-Length bytes size specified
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                writer.Write(data);
                writer.Flush();
                writer.Close();

                /*使用byte形式传递，汉字可以直接传递而不需要Encode~*/
                // byte[] bytes = Encoding.UTF8.GetBytes(data);//byte传递使用UTF-8
                //Stream writer =request.GetRequestStream();
                //request.ContentLength = bytes.Length;
                //Stream oStreamOut = request.GetRequestStream();
                //oStreamOut.Write(bytes,0,bytes.Length);
                //oStreamOut.Flush();
                //oStreamOut.Close();

                /*
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));

                //cookie
                string allCookieString = response.Headers["Set-Cookie"];
                if (!string.IsNullOrEmpty(allCookieString))
                {
                    _responseCookies = new NameValueCollection();

                    string[] cookies = allCookieString.Split(',');//cookie里的过期GMT时间也是含有“,”号的
                    foreach (string cookieString in cookies)
                    {
                        //c = “tgc=TGT-MTExNDMwODg3Nw==-1294129295-87B74BCB1356E809B9CECFCF0B60E297; domain=login.sina.com.cn; path=/; Httponly”
                        string[] cookie = cookieString.Split(';');
                        if (cookie.Length > 0)
                        {
                            string kv = cookie[0];//第一个是：tgc=TGT-MTExNDMwODg3Nw==-1294129295-87B74BCB1356E809B9CECFCF0B60E297

                            int p = kv.IndexOf('=');//第一个=号的位置，value里也是可能含有等号的
                            if (p > -1)//kv有可能是cookie的过期时间的分隔：“expires=Tuesday, 11-Jan-11 08:21:36 GMT;”，所以这里要判断
                            {
                                string key = kv.Substring(0, p);
                                string value = kv.Substring(p + 1);
                                _responseCookies.Add(key, value);
                            }
                        }
                    }
                }
                data = reader.ReadToEnd();
                var relativeURL = data.Replace("<script>location.href='", "").Replace("';</script>", "");
                var str1Content = this.GetCustomerFlowInfo(_responseCookies, _domain + relativeURL);
                 */

                var aCookie = new HttpCookie("abc", "efg");


                var httpbase = new HttpBase();

                httpbase.RequestHeaders.Add("Cookie",
                    "code=af1bf71c1ff9aeeddfa949ab5ec511cc;Hm_lpvt_f8653f9e880c5fb05e49f824bac7a6e5=1415353797;Hm_lvt_f8653f9e880c5fb05e49f824bac7a6e5=1415276240,1415280419,1415280433,1415348269");
                var str = httpbase.GetResponseText("http://www.zmeng123.com/web/login_action.php", "POST",
                    "username=云程科技&action_flag=&password=123456&check=8&put_submit=");
                var relativeUrl = str.Replace("<script>location.href='", "").Replace("';</script>", "");

                var p = relativeUrl.IndexOf("&session", StringComparison.Ordinal); //第一个=号的位置，value里也是可能含有等号的
                if (p > -1) //kv有可能是cookie的过期时间的分隔：“expires=Tuesday, 11-Jan-11 08:21:36 GMT;”，所以这里要判断
                {
                    relativeUrl = relativeUrl.Substring(p); //&session=3508d7f0f0086410bf839eb6dab173a7
                }

                // string session = str.Substring(str.IndexOf("session=") + 8, 32);

                string newUrl = ZmengDomain + "gaikuo.php?c=gaikuo" + relativeUrl;
                var httpBase2 = new HttpBase();

                var sb = new StringBuilder();
                sb.AppendLine(
                    "code=af1bf71c1ff9aeeddfa949ab5ec511cc; Hm_lpvt_f8653f9e880c5fb05e49f824bac7a6e5=1415358623; Hm_lvt_f8653f9e880c5fb05e49f824bac7a6e5=1414396868,1415279513,1415355694;");
                foreach (string item in httpbase.ResponseCookies.AllKeys)
                {
                    sb.AppendLine(item + "=" + httpbase.ResponseCookies.Get(item) + ";");
                }
                httpBase2.RequestHeaders.Add("Cookie", sb.ToString().Replace("\r\n", ""));

                string str2 = httpBase2.GetResponseText(newUrl, "GET", "");

                //HttpBase httpbase2 = new HttpBase();
                ////<script>location.href='template_view.php?c=wifi&session=1e6d47652f0445c7daff8d24219455d6';</script>
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine("code=af1bf71c1ff9aeeddfa949ab5ec511cc; Hm_lpvt_f8653f9e880c5fb05e49f824bac7a6e5=1415358623; Hm_lvt_f8653f9e880c5fb05e49f824bac7a6e5=1414396868,1415279513,1415355694;");
                //foreach (var item in httpbase.ResponseCookies.Keys)
                //{
                //    sb.AppendLine(item + "=" + httpbase.ResponseCookies.Get(item.ToString()) + ";");
                //}
                //Console.WriteLine(sb.ToString().Replace("\r\n", " "));

                //httpbase2.RequestHeaders.Add("Cookie", sb.ToString().Replace("\r\n", " "));
                ////str = httpbase2.GetResponseText("http://www.zmeng123.com/lib/charts.php?chart=inde_cl_new&start=2014-10-08&end=2014-11-06&session=" + session + "&type=0", "GET", "");

                //str = httpbase2.GetResponseText("http://www.zmeng123.com/web/gaikuo.php?c=gaikuo&session=" + session, "GET", "");

                return Json(new
                {
                    relativeURL1 = relativeUrl,
                    responseCookies = httpbase.ResponseCookies,
                    strContent2 = str2
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

        public ActionResult GetTuoZongFans()
        {
            const string tzFansUrl = TuoZongFansUrl;
            try
            {
                var httpBase = new HttpBase();
                var listFnas = new List<TuoZongFans>
                {
                    new TuoZongFans
                    {
                        openid = "oX52ujpOaC5DKfhVjGxSDuIjY3fU",
                        lasttime = CMSUtilityTools.DateTime2Unix(new DateTime(2014, 10, 30))
                    }
                };
                var tzParams = new JavaScriptSerializer().Serialize(new TuoZongFansParams
                {
                    merchant_id = "9929",
                    fans = listFnas.ToArray()
                });


                //var str1 = httpBase.GetResponseText(tzFansUrl, "POST", tzParams);

                var postData = "&merchant_id=9929";
                var i = 0;
                foreach (var tzFan in listFnas)
                {
                    postData += "&fans[" + i + "][lasttime]=" + tzFan.lasttime;
                    postData += "&fans[" + i + "][openid]=" + tzFan.openid;
                    i++;
                }

                //var str1 = HttpHelp.Post(tzFansUrl, "&merchant_id=9929&fans[0][lasttime]=1415698101&fans[0][openid]=of3jzt8hXEhOlO-MgoiPMg0wU1AM&fans[1][lasttime]=1415698101&fans[1][openid]=of3jzt1yKVNYtcUTVNHBffddY7h4");
                var str1 = HttpHelp.Post(tzFansUrl, postData);
                return Json(new
                {
                    postData,
                    result = str1
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = ex.Message
                });
            }
        }

        /// <summary>
        ///     至此，经过一个4个小时的调试，得出结论：当用
        ///     "application/x-www-form-urlencoded" 传递给远程服务器的时候，参数应该使用 param1=val1&param2=val2的形式传递出去；
        ///     当使用"application/json"的时候，必须用{“aa”:“bb”}的形式.
        ///     说到底还是对 http://www.cnblogs.com/fish-li/archive/2011/07/17/2108884.html 实战太少 2014年12月1日
        /// </summary>
        /// <returns></returns>
        public ActionResult LocalPostTest()
        {
            const string localUrl = "http://192.168.1.123:12345/Test/Test";
            try
            {
                var str1 = HttpHelp.Post(localUrl,
                    "slogan=%E8%BF%98%E8%AF%8A%E6%96%AD%E6%9D%A1%E6%AF%9B%E5%95%8A%EF%BC%81~~~",
                    "application/x-www-form-urlencoded"); //application/x-www-form-urlencoded
                //var str1 = HttpHelp.Post(localUrl, "{\"slogan\":\"还诊断条毛啊！!!!\"}");//===>{"str1": "{\"name\":\"张全蛋\",\"age\":20,\"slogan\":null}" }
                //说明：application/json只是告诉服务端是用什么格式传递上去罢了，如果服务端是用JSON形式解析参数，则就要用 "application/json"
                return Json(new
                {
                    str1
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = ex.Message
                });
            }
            /*
             *  public ActionResult Test(string slogan)
                {
                    return Json(new
                    {
                        name = "张全蛋",
                        age = 20,
                        slogan
                    });
                }
             */
        }
    }
}