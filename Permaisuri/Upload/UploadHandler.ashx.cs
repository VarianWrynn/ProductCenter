using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Upload
{
    /// <summary>
    /// Summary description for UploadHandler
    /// 而为了能使用这个自定义的HttpHandler，我们需要在应用程序目录下的Web.config中注册它。
    /// verb指的是请求此文件的方式，可以是post或get，用*代表所有访问方式。
    /// type属性由“,”分隔成两部分，第一部分是实现了接口的类名，第二部分是位于Bin目录下的编译过的程序集名称。
    /// </summary>
    public class UploadHandler : IHttpHandler
    {
        private readonly JavaScriptSerializer js;
        

        private string StorageRoot
        {
           // get { return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/MediaLib/Files/")); } //Path should! always end with '/'
            get { return Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ImageStoragePath"])); }
        }

        public UploadHandler()
        {
            js = new JavaScriptSerializer();
            js.MaxJsonLength = 41943040;
        }

        public bool IsReusable { get { return false; } }


        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");
           
            HandleMethod(context);
        }

        // Handle request based on method
        private void HandleMethod(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    if (GivenFilename(context)) DeliverFile(context);
                    //else ListCurrentFiles(context);
                    break;

                case "POST":
                case "PUT":
                    UploadFile(context);
                    break;
                
                case "DELETE":
                    //删除的问题今天整整查了一整天，在这里老是会报：405- Methd Not Allowed.没有结果。最后只能把删除逻辑搬到Media的Controller里面进行实现
                    //2013年9月12日15:26:56
                    DeleteFile(context);
                    break;

                case "OPTIONS":
                    ReturnOptions(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

        private static void ReturnOptions(HttpContext context)
        {
            context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
            context.Response.StatusCode = 200;
        }

        // Delete file from the server
        private void DeleteFile(HttpContext context)
        {
            var filePath = StorageRoot + context.Request["f"];
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // Upload file to the server
        private void UploadFile(HttpContext context)
        {
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }

            WriteJsonIframeSafe(context, statuses);
        }

       /// <summary>
       /// 这个方法被我作废了好吗？2014年1月24日15:58:15
       /// </summary>
       /// <param name="fileName"></param>
       /// <param name="context"></param>
       /// <param name="statuses"></param>
        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
        {
            //if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            //var inputStream = context.Request.Files[0].InputStream;

            ////根据HMNUM拼凑当前需要存储的文件夹路径
            //var HMNUM = context.Request.Form["HMNUM"];
            //var curStorageRoot = Path.Combine(StorageRoot, HMNUM) + "/";
            //var StorageRootOrig = Path.Combine(curStorageRoot, "Orig") + "/";
            //var StorageRoot160 = Path.Combine(curStorageRoot, "160") + "/";
            ////如果不存在，则创建之。
            //if (!Directory.Exists(StorageRootOrig))
            //{
            //    Directory.CreateDirectory(StorageRootOrig);
            //}
            //if (!Directory.Exists(StorageRoot160))
            //{
            //    Directory.CreateDirectory(StorageRoot160);
            //}

            //using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            //{
            //    var buffer = new byte[1024];
            //    var l = inputStream.Read(buffer, 0, 1024);
            //    while (l > 0)
            //    {
            //        fs.Write(buffer, 0, l);
            //        l = inputStream.Read(buffer, 0, 1024);
            //    }
            //    fs.Flush();
            //    fs.Close();
            //}
            //statuses.Add(new FilesStatus(new FileInfo(fullName)));
        }

        /// <summary>
        /// 目前都是用这个方法.
        /// Change1:新增从SKUOrder页面上传之后，需要做的相关逻辑的操作（关联当前SKU）2014年1月24日16:05:24
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statuses"></param>
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            Image imgInfo = null;
            try
            {
                string cookis = context.Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
                var serializer = new JavaScriptSerializer();
                string decCookies = CryptTools.Decrypt(cookis);
                User_Profile_Model userInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
                MediaServices mSvr = new MediaServices();
                var ProductID = context.Request.Form["ProductID"];

                //根据HMNUM拼凑当前需要存储的文件夹路径
                var HMNUM = context.Request.Form["HMNUM"];
                var curStorageRoot = Path.Combine(StorageRoot,HMNUM)+"/";//当前HM#图像的存储总地址

                // ReqIndicator, 0:默认，1:HMNUM页面 2:SKUOrdr页面
                var strIndicator = context.Request.Form["ReqIndicator"];
                int ReqIndicator = 0;
                int.TryParse(strIndicator, out ReqIndicator);

                long SKUID = 0;
                if (ReqIndicator == 2)
                {
                    var strWPID = context.Request.Form["SKUID"];
                    long.TryParse(strWPID, out SKUID);
                }

                //如果这些路径不存在，则创建之。
                if (!Directory.Exists(curStorageRoot))
                {
                    Directory.CreateDirectory(curStorageRoot);
                }
                for (int i = 0; i < context.Request.Files.Count; i++)
                {
                    var file = context.Request.Files[i];
                    var SerialNum = context.Request.Form["SerialNum"].Split(',')[i];//用户标识当前HM关联图像的序列值，给前端重命名的时候使用！
                    var imgSize = context.Request.Form["imgSize"].Split(',')[i];//该图像多大，比如100MB
                    var rename = context.Request["rename"].Split(',')[i];//获取客户端重命名的名称
                    var extension = Path.GetExtension(file.FileName);//获取图像扩展名
                    var fullPath = curStorageRoot + rename + extension;
                    var thumbPath = curStorageRoot + rename + "_th" + extension;
                    file.SaveAs(fullPath);

                    // SendSmallImage(fullPath, thumbPath, 160);
                    SamllImageGengeratedFixedHW(fullPath, thumbPath, 200, 200);

                    //Save image infor to database
                    imgInfo = Image.FromFile(fullPath);
                    long newID = 0;

                    var newMedia = new MediaLibrary_Model
                    {
                        ProductID = Convert.ToInt64(ProductID),
                        HMNUM = HMNUM,
                        SerialNum = Convert.ToInt32(SerialNum),
                        ImgName = rename,
                        MediaType = 1,
                        fileFormat = extension,
                        fileSize = imgSize,
                        fileWidth = imgInfo.Width,
                        fileHeight = imgInfo.Height,
                        Description = ""
                        
                    };
                    if (ReqIndicator == 2)
                    {
                        mSvr.addMediaLibraryWithSKURelation(newMedia, userInfo.User_Account, SKUID, out newID);
                    }
                    else
                    {
                        mSvr.addMediaLibrary(newMedia, userInfo.User_Account, out newID);
                    }
                    imgInfo.Dispose();
                    //string fullName = Path.GetFileName(file.FileName);
                    //备注：newID即指 新增的MediaID
                    statuses.Add(new FilesStatus(rename + extension, file.ContentLength, fullPath, newID, HMNUM));
                }
            }
            catch (Exception ex)
            {
                if (imgInfo != null)
                {
                    imgInfo.Dispose();
                }
                NBCMSLoggerManager.Error("UploadWholeFile");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error(ex.Source);
            }
        }

        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            // var jsonObj = js.Serialize(statuses.ToArray());

            var jsonObj = js.Serialize(new
            {
                files = statuses.ToList()
            });
            context.Response.Write(jsonObj);
        }

        private static bool GivenFilename(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request["f"]);
        }

        private void DeliverFile(HttpContext context)
        {
            var filename = context.Request["f"];

            var HMNUM = context.Request["HMNUM"];
            var curStorageRoot = Path.Combine(StorageRoot, HMNUM) + "/";
            //var StorageRootOrig = Path.Combine(curStorageRoot, "Orig") + "/";

            var filePath = curStorageRoot + filename;

            if (File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        private void ListCurrentFiles(HttpContext context)
        {
            //var files =
            //    new DirectoryInfo(StorageRootOrig)
            //        .GetFiles("*", SearchOption.TopDirectoryOnly)
            //        .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
            //        .Select(f => new FilesStatus(f))
            //        .ToArray();

            //string jsonObj = js.Serialize(files);
            //context.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
            //context.Response.Write(jsonObj);
            //context.Response.ContentType = "application/json";
        }

        /// <summary>
        /// c#生成缩略图 图片固定大小 缩略图加空白填充
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="newFile"></param>
        /// <param name="desWidth"></param>
        /// <param name="desHeight"></param>
        public void SamllImageGengeratedFixedHW(string fileName, string newFile, int desWidth, int desHeight)
        {
            //容器高与宽
            string backcolor = "#FFFFFF";
            string borderColor = "#999999";
            int penwidth = 0;
            int penhight = 0;

            System.Drawing.Image oimage = System.Drawing.Image.FromFile(fileName);
            //System.Drawing.Image oimage = System.Drawing.Image.FromStream(ostream);
            int owidth = oimage.Width;
            int oheight = oimage.Height;
            string hw = GetImageSize(owidth, oheight);
            string[] aryhw = hw.Split(';');
            int twidth = Convert.ToInt32(aryhw[0]);
            int theight = Convert.ToInt32(aryhw[1]);
            //新建一个bmp图片                                         
            System.Drawing.Bitmap timage = new System.Drawing.Bitmap(desWidth, desHeight);
            //System.Drawing.Imaging.ImageFormat thisFormat = timage.RawFormat;
            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(timage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(System.Drawing.ColorTranslator.FromHtml(backcolor));
            if (twidth < desWidth & theight == desHeight)
            {
                penwidth = desWidth - twidth;
            }
            else if (twidth == desWidth && theight < desHeight)
            {
                penhight = desHeight - theight;
            }
            else if (twidth < desWidth && theight < desHeight)
            {
                penwidth = desWidth - twidth;
                penhight = desHeight - theight;
            }
            int top = penhight / 2;
            int left = penwidth / 2;
            g.DrawImage(oimage, new System.Drawing.Rectangle(left, top, twidth, theight), new System.Drawing.Rectangle(0, 0, owidth, oheight), System.Drawing.GraphicsUnit.Pixel);
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.ColorTranslator.FromHtml(borderColor));
            g.DrawRectangle(pen, 0, 0, desWidth - 2, desHeight - 2);
            //string pathifile = Server.HtmlEncode(Request.PhysicalApplicationPath) + "image\\" +"t"+ DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".jpg";
            try
            {
                //原图保存
                // oimage.Save(pathifile,System.Drawing.Imaging.ImageFormat.Jpeg);
                //缩图图保存
                timage.Save(newFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oimage.Dispose();//该方法如果不执行，后面删除图像等操作会提示文件被其他进程占用 2020-11-25
                g.Dispose();
                timage.Dispose();
            }
        }

        protected string GetImageSize(int LoadImgW, int LoadImgH)
        {
            int xh = 0;
            int xw = 0;
            //容器高与宽
            int oldW = 200;
            int oldH = 200;
            //图片的高宽与容器的相同
            if (LoadImgH == oldH && LoadImgW == (oldW))
            {//1.正常显示
                xh = LoadImgH;
                xw = LoadImgW;
            }
            if (LoadImgH == oldH && LoadImgW > (oldW))
            {//2、原高==容高，原宽>容宽 以原宽为基础
                xw = (oldW);
                xh = LoadImgH * xw / LoadImgW;
            }
            if (LoadImgH == oldH && LoadImgW < (oldW))
            {//3、原高==容高，原宽<容宽  正常显示   
                xw = LoadImgW;
                xh = LoadImgH;
            }
            if (LoadImgH > oldH && LoadImgW == (oldW))
            {//4、原高>容高，原宽==容宽 以原高为基础   
                xh = oldH;
                xw = LoadImgW * xh / LoadImgH;
            }
            if (LoadImgH > oldH && LoadImgW > (oldW))
            {//5、原高>容高，原宽>容宽           
                if ((LoadImgH / oldH) > (LoadImgW / (oldW)))
                {//原高大的多，以原高为基础
                    xh = oldH;
                    xw = LoadImgW * xh / LoadImgH;
                }
                else
                {//以原宽为基础
                    xw = (oldW);
                    xh = LoadImgH * xw / LoadImgW;
                }
            }
            if (LoadImgH > oldH && LoadImgW < (oldW))
            {//6、原高>容高，原宽<容宽 以原高为基础        
                xh = oldH;
                xw = LoadImgW * xh / LoadImgH;
            }
            if (LoadImgH < oldH && LoadImgW == (oldW))
            {//7、原高<容高，原宽=容宽 正常显示       
                xh = LoadImgH;
                xw = LoadImgW;
            }
            if (LoadImgH < oldH && LoadImgW > (oldW))
            {//8、原高<容高，原宽>容宽 以原宽为基础    
                xw = (oldW);
                xh = LoadImgH * xw / LoadImgW;
            }
            if (LoadImgH < oldH && LoadImgW < (oldW))
            {//9、原高<容高，原宽<容宽//正常显示    
                xh = LoadImgH;
                xw = LoadImgW;
            }
            return xw + ";" + xh;
        }

        /// <summary>
        /// 生成略缩图
        /// </summary>
        /// <param name="fileName">源文件路径</param>
        /// <param name="newFile">新文件路径</param>
        /// <param name="maxWidth">生成新文件的宽度,高度自动算</param>
        public static void SendSmallImage(string fileName, string newFile, int maxWidth)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);
            System.Drawing.Imaging.ImageFormat
            thisFormat = img.RawFormat;
            int maxHeight = img.Height * maxWidth / img.Width;
            Size newSize = new Size(maxWidth, maxHeight);
            Bitmap outBmp = new Bitmap(newSize.Width, newSize.Height);
            Graphics g = Graphics.FromImage(outBmp);        // 设置画布的描绘质量
            g.DrawImage(img, new System.Drawing.Rectangle(0, 0, newSize.Width, newSize.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时,设置压缩质量
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象.
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICI = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICI = arrayICI[x];
                    //设置JPEG编码
                    break;
                }
            }
            if (jpegICI != null)
            {
                outBmp.Save(newFile, jpegICI, encoderParams);
            }
            else
            {
                outBmp.Save(newFile, thisFormat);
            }
            img.Dispose();
            outBmp.Dispose();
        }

    }
}