using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

// ReSharper disable CheckNamespace
namespace Permaisuri.Controllers
// ReSharper restore CheckNamespace
{
    public class ExcelResult:ActionResult
    {
        private readonly string _fileName;
        private readonly string _content;
        private readonly string _contentType = String.Empty;

        public string FileName
        {
            get { return _fileName; }
        }

        public string Content
        {
            get { return _content; }
        }


        public string ContentType
        {
            get { return _contentType; }
        }

        public ExcelResult(string content, string fileName, string contentType)
        {
            _content = content;
            _fileName = fileName;
            _contentType = contentType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            //response.contenttype指定文件类型 能为application/ms-excel || application/ms-word || application/ms-txt || application/ms-html || 或其他浏览器可直接支持文件 
            //WriteFile(_fileName, "application/ms-excel", _content);
            WriteFile(_fileName, _contentType, _content);
        }

        private static void WriteFile(string fileName, string contentType, string content)
        {
            var context = HttpContext.Current;

            fileName = HttpUtility.UrlEncode(fileName, Encoding.UTF8);    //对中文文件名进行HTML转码

            var buffer = Encoding.UTF8.GetBytes(content);
            context.Response.ContentEncoding = Encoding.UTF8;
            var outBuffer = new byte[buffer.Length + 3];
            outBuffer[0] = 0xEF;//有BOM,解决乱码
            outBuffer[1] = 0xBB;
            outBuffer[2] = 0xBF;
            Array.Copy(buffer, 0, outBuffer, 3, buffer.Length);
            var cpara = Encoding.UTF8.GetChars(outBuffer); // byte[] to char[]

            context.Response.Clear();
            context.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            context.Response.Charset = "";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.ContentType = contentType;
            context.Response.Write(cpara, 0, cpara.Length);
            context.Response.End();
        }
    }
}