using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;

namespace PermaisuriCMS.Common
{
    public static class HttpHelp
    {

        public static string Post(string url, string postData, string sContentType = null)
        {
            var rspStatus = "";
            //return Post(url.ToLower(), postData, ref rspStatus);
            if (string.IsNullOrEmpty(sContentType))
            {
                sContentType = "application/x-www-form-urlencoded";
            }
            return Post(url, postData, sContentType, ref rspStatus);
        }

        public static string Post(string url, Dictionary<string, string> keyvalues)
        {

            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            string postData = null;
            if (keyvalues != null && keyvalues.Count > 0)
            {
                postData = string.Join("&",
                        (from kvp in keyvalues
                         let item = kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value)
                         select item
                         ).ToArray()
                     );
            }
            return Post(url, postData);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="sContentType"></param>
        /// <param name="rspStatus"></param>
        /// <returns></returns>
        public static string Post(string url, string postData, string sContentType, ref string rspStatus)
        {
            var myEncoding = Encoding.UTF8;
            const string sMode = "POST";
            //const string sContentType = "application/x-www-form-urlencoded"; 
            //const string sContentType = "application/json";
            //const string sContentType = "text/xml";


            try
            {
              
                // init
                //var req = HttpWebRequest.Create(url.ToLower()) as HttpWebRequest;
                //var req = WebRequest.Create(url.ToLower()) as HttpWebRequest;
                var req = WebRequest.Create(url) as HttpWebRequest;
                if (req == null) return "url address is invalid";
                req.Method = sMode;
                req.Accept = "*/*";
                //req.KeepAlive = false;
                //req.Timeout = 90000;  
                req.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

                var bufPost = myEncoding.GetBytes(postData);
                req.ContentType = sContentType;
                req.ContentLength = bufPost.Length;
                var newStream = req.GetRequestStream();
                newStream.Write(bufPost, 0, bufPost.Length);
                newStream.Close();

                // Response
                var res = req.GetResponse() as HttpWebResponse;
                rspStatus = ((int)res.StatusCode).ToString(CultureInfo.InvariantCulture);
                using (var myResponseStream = res.GetResponseStream())
                {
                    using (var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                    {
                        var retString = myStreamReader.ReadToEnd();
                        myStreamReader.Close();
                        if (myResponseStream != null) myResponseStream.Close();
                        return retString;
                    }
                }
            }
            catch (WebException e)
            {
                var res = (HttpWebResponse)e.Response;
                rspStatus = ((int)res.StatusCode).ToString(CultureInfo.InvariantCulture);
                var sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                var strHtml = sr.ReadToEnd();
                return strHtml;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }



      static public string Get(string url, string getDataStr, HttpCookie cooike)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url + "&" + getDataStr);
                if (string.IsNullOrEmpty(getDataStr))
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
                }
                request.Method = "Get";
                //request.ContentType = "application/x-www-form-urlencoded";
                request.ContentType = "application/json";

                if (cooike != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(new Uri(url), new Cookie("ADC4ECDeskTop", cooike.Value));

                }

                var response = (HttpWebResponse)request.GetResponse();
                using (var myResponseStream = response.GetResponseStream())
                {
                    using (var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                    {
                        var retString = myStreamReader.ReadToEnd();
                        myStreamReader.Close();
                        if (myResponseStream != null) myResponseStream.Close();
                        return retString;
                    }
                }
            }
            catch (WebException e)
            {
                var res = (HttpWebResponse)e.Response;
                if (res == null) return "error, e.Response is null";
                var sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                var strHtml = sr.ReadToEnd();
                return strHtml;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "error lee 2014年11月19日";
        }

        static public string Get(string url, string getDataStr)
        {
            return Get(url, getDataStr, null);
        }
    }
}
