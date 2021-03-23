using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    public class MediaController : Controller
    {
        private const string CustomerMediaPath = "~/MediaLib/UploadImage";

        public ActionResult MediaLibrary()
        {
            //ViewBag.CMSImgUrl = ConfigurationManager.AppSettings["CMSImgUrl"];
            if (Request.Url != null)
                ViewBag.CMSImgUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/MediaLib/Files/";
            return View();
        }


        /// <summary>
        /// 批量上传
        /// Change1:新增一个参数ReqIndicator{0:defult,1:HMNUM页面请求,2:SKUOrder页面请求};2014年1月24日14:19:38
        /// </summary>
        /// <param name="reqIndicator"></param>
        /// <returns></returns>
        public ActionResult FilesUpload(int? reqIndicator)
        {
            var reqInd = reqIndicator.ConvertToNotNull();
            ViewBag.ReqIndicator = reqInd;
            ViewBag.SKUID = Request["SKUID"];//用户传参，上传之后自动让SKUID和图像关联
            ViewBag.ProductID = Request["ProductID"];
            if (reqInd <= 0) return View(new CMS_HMNUM_Model());
            var hmSvr = new HMNUMServices();
            var newHmnum = hmSvr.GetSingleHMNUMByID(Convert.ToInt64(Request["ProductID"]));
            return View(newHmnum);
            //var newHMNUM = hmSvr.GetSingleHMNUMByID(
        }

        /// <summary>
        /// 为批量删除提供HM关联的 autoCompelted使用
        /// CreateDate:2013年11月27日15:31:37
        /// </summary>
        /// <param name="hmModel"></param>
        /// <returns></returns>
        public ActionResult GetProductInfo(CMS_HMNUM_Model hmModel)
        {
            try
            {
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new ProductCommonServices().GetProductsByKeyWords(hmModel, 0)
                });

            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = ex.Message
                });
            }
        }

        
        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        /// <summary>
        /// 不要用Void方法返回，否则客户端会报错异常.... 2013年9月12日15:32:43.
        /// </summary>
        /// <param name="id">MediaID</param>
        /// <param name="hmnum"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpDelete]
        public bool Delete(string id, string hmnum,string f)
        {
            try
            {
                //经过测试发现：
                //The process cannot access the file 'D:\ProjectFiles\Permaisuri
                //\Permaisuri\Permaisuri\MediaLib\Files\220500\220500_4.jpg' 
                //because it is being used by another process. 
                //所以这里不做删除，留在本地，这不影响业务。如果有新的相同的文件名上来，只会覆盖掉旧的 2014年2月14日14:51:27
                //var filename = f;
                //string MediaPath = ConfigurationManager.AppSettings["ImageStoragePath"];//~/MediaLib/Files/

                //var fileOrigPath = Path.Combine(Server.MapPath(MediaPath + HMNUM), filename);
                ////var file160Path = Path.Combine(Server.MapPath(MediaPath + HMNUM), filename+"_th");

                //if (System.IO.File.Exists(fileOrigPath))
                //{
                //    System.IO.File.Delete(fileOrigPath);
                //}
                //if (System.IO.File.Exists(file160Path))  时间来不及了...
                //{
                //    System.IO.File.Delete(file160Path);
                //}

                var msV = new MediaServices();
                msV.removeMediaLibraryByID(Convert.ToInt64(id));
                return true;
            }
            catch (Exception ex)
            {

                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return false;
            }
        }


		/// <summary>
		/// 初始化页面数据（第一次访问）
		/// </summary>
		/// <returns></returns>
        public ActionResult InitMediaLibrary(MediaLibrary_QueryModel queryModel)
        {
            try
            {
                int count;
                //List<MediaGroupBy_Model> groupByList = new List<MediaGroupBy_Model>();
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        MediaData = new {
                            list = new MediaServices().GetMediaLibraryList(queryModel, out count),
                            //groupByList =groupByList,
                            count
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        public ActionResult UploadImage()
        {
            var skuid = Request["SKUID"];
            if (!(String.IsNullOrEmpty(skuid)))
            {
                ViewBag.SKUID = skuid;
            }
            ViewBag.CustomerMediaPath = CustomerMediaPath;
            return View();
        }

        public ActionResult SearchMediaLibrary(MediaLibrary_QueryModel queryModel)
        {

            var ms = new MediaServices();
            try
            {
                //List<MediaGroupBy_Model> groupByList = new List<MediaGroupBy_Model>();
                int count;
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        list = ms.GetMediaLibraryList(queryModel, out count),
                        //groupByList = groupByList,
                        count
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// 修改图像的名称
        /// </summary>
        /// <param name="newFileName">新图像名称</param>
        /// <param name="oldFileName">旧图像名称</param>
        /// <param name="fileExt">图像的类型</param>
        /// <returns></returns>
        public ActionResult EditMediaName(String newFileName, String oldFileName, String fileExt)
        {
            try
            {
                var fileOrigPath = Path.Combine(Server.MapPath("~/MediaLib/Files/Orig/"), oldFileName + "." + fileExt);
                var file160Path = Path.Combine(Server.MapPath("~/MediaLib/Files/160/"), oldFileName + "." + fileExt);
                if (!System.IO.File.Exists(fileOrigPath) || !System.IO.File.Exists(file160Path))
                    return Json(new NBCMSResultJson
                    {
                        Status = (StatusType.Error),
                        Data = ("failed to change MediaName ")
                    });
                var fiOrg = new FileInfo(fileOrigPath);
                fiOrg.MoveTo(Path.Combine(Server.MapPath("~/MediaLib/Files/Orig/"), newFileName + "." + fileExt));

                var fi160 = new FileInfo(file160Path);
                fi160.MoveTo(Path.Combine(Server.MapPath("~/MediaLib/Files/160/"), newFileName + "." + fileExt));

                //isOK = new MediaServices().EditMediaName(mediaID, newFileName, model.User_Account);
                return Json(new NBCMSResultJson
                {
                    Status = (StatusType.Error),
                    Data = ("failed to change MediaName ")
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

       

        /// <summary>
        /// 作废 2013年9月25日11:43:11 Lee
        /// 已经在PictureDetails返回的和图片相关的CMS_SKU对象的信息了
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PictureDetails(long mediaId)
        {
            return Json(new MediaServices().GetCMS_SKUByMediaID(mediaId));
        }

        [HttpPost]
        public ActionResult UploadingImage2Server(HttpPostedFileBase upImage)
        {
            try
            {
                if ((upImage == null))
                {
                    return Json(new { msg = 0 });
                }
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "bmp" };
                var extension = Path.GetExtension(upImage.FileName);
                if (extension == null) return Json(new {msg = 0});
                var fileExt = extension.Substring(1).ToLower();
                if (!supportedTypes.Contains(fileExt))
                {
                    return Json(new { msg = -1 });
                }

                if (upImage.ContentLength > 1024 * 1000 * 10)
                {
                    return Json(new { msg = -2 });
                }

                var r = new Random();
                var filename = DateTime.Now.ToString("yyyyMMddHHmmss") + r.Next(10000) + "." + fileExt;
                var filepath = Path.Combine(Server.MapPath(CustomerMediaPath), filename);
                upImage.SaveAs(filepath);
                return Json(new { msg = filename });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                return Json(new { msg = -3 });
            }
        }

       
        /// <summary>
        /// 暂时作废了 2013年11月28日15:02:21
        /// 该方法用于上传哪些被剪切过的图像 2014年11月14日
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveImages()
        {
            var model = new UploadImagesModel();
            try
            {
                model.headFileName = Request.Form["headFileName"];
                model.x = Convert.ToInt32(Request.Form["x"]);
                model.y = Convert.ToInt32(Request.Form["y"]);
                model.width = Convert.ToInt32(Request.Form["width"]);
                model.height = Convert.ToInt32(Request.Form["height"]);

                var filepath = Path.Combine(Server.MapPath(CustomerMediaPath), model.headFileName);
                var fileExt = Path.GetExtension(filepath);
                var orgFileName = Path.GetFileNameWithoutExtension(filepath);

                var path160 = Server.MapPath(CustomerMediaPath + "/160/");
                if (!Directory.Exists(path160))
                {
                    Directory.CreateDirectory(path160);
                }
                var path100 = Server.MapPath(CustomerMediaPath + "/100/");
                if (!Directory.Exists(path100))
                {
                    Directory.CreateDirectory(path100);
                }
                var fullPath160 = Path.Combine(path160, orgFileName + "_160" + fileExt);
                var fullPath100 = Path.Combine(path100, orgFileName + "_100" + fileExt);
                CutAvatarWithPadding(filepath, model.x, model.y, model.width, model.height, 75L, fullPath160, 210);
                CutAvatar(filepath, model.x, model.y, model.width, model.height, 75L, fullPath100, 100);

                var imgInfo = Image.FromFile(filepath);
                const long newId = 0;
                imgInfo.Dispose();

                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        name = orgFileName,
                        ext = fileExt,
                        newID = newId
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }


        /// <summary>
        /// 创建缩略图
        /// </summary>
        public void CutAvatar(string imgSrc, int x, int y, int width, int height, long quality, string savePath, int t)
        {

            var original = Image.FromFile(imgSrc);
            var img = new Bitmap(t, t, PixelFormat.Format24bppRgb);
            try
            {
                img.MakeTransparent(img.GetPixel(0, 0));
                img.SetResolution(t, t);
                using (var gr = Graphics.FromImage(img))
                {
                    if (original.RawFormat.Equals(ImageFormat.Jpeg) || original.RawFormat.Equals(ImageFormat.Png) ||
                        original.RawFormat.Equals(ImageFormat.Bmp))
                    {
                        gr.Clear(Color.Transparent);
                    }
                    if (original.RawFormat.Equals(ImageFormat.Gif))
                    {
                        gr.Clear(Color.White);
                    }

                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.SmoothingMode = SmoothingMode.AntiAlias;
                    gr.CompositingQuality = CompositingQuality.HighQuality;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    using (var attribute = new ImageAttributes())
                    {
                        attribute.SetWrapMode(WrapMode.TileFlipXY);
                        gr.DrawImage(original, new Rectangle(0, 0, t, t), x, y, width, height, GraphicsUnit.Pixel,
                            attribute);
                    }
                }
                var myImageCodecInfo = GetEncoderInfo("image/jpeg");
                if (original.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    myImageCodecInfo = GetEncoderInfo("image/jpeg");
                }
                else if (original.RawFormat.Equals(ImageFormat.Png))
                {
                    myImageCodecInfo = GetEncoderInfo("image/png");
                }
                else if (original.RawFormat.Equals(ImageFormat.Gif))
                {
                    myImageCodecInfo = GetEncoderInfo("image/gif");
                }
                else if (original.RawFormat.Equals(ImageFormat.Bmp))
                {
                    myImageCodecInfo = GetEncoderInfo("image/bmp");
                }

                var myEncoder = Encoder.Quality;
                var myEncoderParameters = new EncoderParameters(1);
                var myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                img.Save(savePath, myImageCodecInfo, myEncoderParameters);
                original.Dispose();
                img.Dispose();
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("MediaController:cutAvatar,KeyWord:" + savePath);
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                original.Dispose();
                img.Dispose();
            }
            finally
            {
                original.Dispose();
                img.Dispose();
            }
        }


        /// <summary>
        /// 打印出带有边框（和水印）的照片
        /// Author: 王力
        /// 效果：http://img0.ph.126.net/wx-jJ9-8IEURg_ZBu1IApA==/6608567359748454180.jpg
        /// </summary>
        /// <param name="imgSrc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="quality"></param>
        /// <param name="savePath"></param>
        /// <param name="t"></param>
        public void CutAvatarWithPadding(string imgSrc, int x, int y, int width, int height, long quality, string savePath, int t)
        {

            var original = Image.FromFile(imgSrc);
            var paddingWidth = (int)((t * 0.2) + t);
            var paddingHeight = (int)((t * 0.5) + t);
            var img = new Bitmap(paddingWidth, paddingHeight, PixelFormat.Format24bppRgb);
            //var img = new Bitmap(t, t, PixelFormat.Format24bppRgb);
            try
            {
                img.MakeTransparent(img.GetPixel(0, 0));
                //img.SetResolution(paddingWidth, paddingHeight);
                //img.SetResolution(t, t);
                using (var gr = Graphics.FromImage(img))
                {
                    if (original.RawFormat.Equals(ImageFormat.Jpeg) || original.RawFormat.Equals(ImageFormat.Png) ||
                        original.RawFormat.Equals(ImageFormat.Bmp))
                    {
                        //gr.Clear(Color.Transparent);
                        gr.Clear(Color.White);
                    }
                    if (original.RawFormat.Equals(ImageFormat.Gif))
                    {
                        gr.Clear(Color.White);
                    }

                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.SmoothingMode = SmoothingMode.AntiAlias;
                    gr.CompositingQuality = CompositingQuality.HighQuality;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    using (var attribute = new ImageAttributes())
                    {
                        attribute.SetWrapMode(WrapMode.TileFlipXY);
                        //gr.DrawImage(original, new Rectangle(0, 0, t, t), x, y, width, height, GraphicsUnit.Pixel, attribute);
                        gr.DrawImage(original, new Rectangle(20, 20, t, t), x, y, width, height, GraphicsUnit.Pixel,
                            attribute);
                    }

                    //var bordcolor = Color.Red;
                    ////var bordwidth = Convert.ToInt32(img.Width * 0.1);
                    ////var bordheight = Convert.ToInt32(img.Height * 0.1);

                    //var bordwidth = 200;
                    //var bordheight =200;

                    //var newheight = img.Height + bordheight;
                    //var newwidth = img.Width + bordwidth;

                    //using (var attribute = new ImageAttributes())
                    //{
                    //    attribute.SetWrapMode(WrapMode.TileFlipXY);
                    //    gr.DrawImage(original, new Rectangle(0, 0, t, t), x, y, width, height, GraphicsUnit.Pixel,
                    //        attribute);
                    //    gr.DrawLine(new Pen(bordcolor, bordheight), 0, 0, newwidth, 0);
                    //    gr.DrawLine(new Pen(bordcolor, bordheight), 0, newheight, newwidth, newheight);
                    //}
                    //水印 http://www.codeproject.com/Tips/323990/How-to-Create-watermarked-images-in-csharp-asp-net
                    //gr.DrawString("这是一个测试", new Font("Verdana", 14, FontStyle.Bold), new SolidBrush(Color.Beige), x: (img.Width / 2), y: (img.Height / 2)); 
                    gr.DrawString(@"胸有激雷而面如平湖者", new Font("Verdana", 14, FontStyle.Bold), new SolidBrush(Color.Tomato), x: 0, y: img.Height -80);
                    gr.DrawString(@"可拜上将军也！", new Font("Verdana", 14, FontStyle.Bold), new SolidBrush(Color.Tomato), x: 0, y: img.Height - 60); 
                }
                var myImageCodecInfo = GetEncoderInfo("image/jpeg");
                if (original.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    myImageCodecInfo = GetEncoderInfo("image/jpeg");
                }
                else if (original.RawFormat.Equals(ImageFormat.Png))
                {
                    myImageCodecInfo = GetEncoderInfo("image/png");
                }
                else if (original.RawFormat.Equals(ImageFormat.Gif))
                {
                    myImageCodecInfo = GetEncoderInfo("image/gif");
                }
                else if (original.RawFormat.Equals(ImageFormat.Bmp))
                {
                    myImageCodecInfo = GetEncoderInfo("image/bmp");
                }

                var myEncoder = Encoder.Quality;
                var myEncoderParameters = new EncoderParameters(1);
                var myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                img.Save(savePath, myImageCodecInfo, myEncoderParameters);
                original.Dispose();
                img.Dispose();
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("MediaController:cutAvatar,KeyWord:" + savePath);
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                original.Dispose();
                img.Dispose();
            }
            finally
            {
                original.Dispose();
                img.Dispose();
            }
        }

        ////根据长宽自适应 按原图比例缩放 
        //private static Size GetThumbnailSize(Image original, int desiredWidth, int desiredHeight)
        //{
        //    var widthScale = (double)desiredWidth / original.Width;
        //    var heightScale = (double)desiredHeight / original.Height;
        //    var scale = widthScale < heightScale ? widthScale : heightScale;
        //    return new Size
        //    {
        //        Width = (int)(scale * original.Width),
        //        Height = (int)(scale * original.Height)
        //    };
        //}

  

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            var encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }



        /// <summary>
        /// New Delete funtion add by MediaLibray model (Mellisa and Bonnie), via MediaID using the same delete function on BLL layer as HMNUMConfig 2014年5月15日11:20:28
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public ActionResult DeleteCmsMedia(long mediaId)
        {
            try
            {
                var cSvr = new HMCommonServices();
                string errMsg;
                if (!cSvr.DeleteCMSMedia(mediaId, out errMsg))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = errMsg == string.Empty ? "Fail to delete this media" : errMsg
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = "OK"
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
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
