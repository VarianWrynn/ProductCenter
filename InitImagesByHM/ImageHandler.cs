using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using NLog;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data.Entity.Validation;

namespace InitImagesByHM
{
    /// <summary>
    /// 操作信息事件代理
    /// </summary>
    public delegate void OperateNotifyHandler(object sender, InitImagesEventArgs e);

    /// <summary>
    /// 图像处理类
    /// </summary>
    public class ImageHandler
    {
        private static string DestinationDir = ConfigurationManager.AppSettings["DestinationDir"];

        private static Logger ImageLog = LogManager.GetCurrentClassLogger(); 

        private int serNum = 0;//以HMNU为Base的序列号，每一次

        private string _operatePath;//操作图像的根目录

        private object locker = new object(); 

        /// <summary>
        /// 操作信息事件
        /// </summary>
        public event OperateNotifyHandler OperateNotify;

        /// <summary>
        /// 线程结束通知事件
        /// </summary>
        public event EventHandler ThreadCompleted;

        public ImageHandler(string operatePath)
        {
            _operatePath = operatePath;
        }


      
        /// <summary>
        /// 逻辑处理入口
        /// </summary>
        public void InitImages()
        {
            if (!Directory.Exists(DestinationDir))
            {
                Directory.CreateDirectory(DestinationDir);
            }
            var delegateThread = new Thread(StartToDo);
            delegateThread.Start();
        }

        private int ChannelID = 12; //Overstock 在Channel表上对于为12


        /// <summary>
        /// 线程委托函数，遍历SKUOrder文件夹下的图像并按照HMNUM去拷贝
        /// </summary>
        private void StartToDo()
        {
            //try
            //{
                List<string> ExceptionSKU = new List<string>();
                //循环遍历SKUOrder文件夹（_operatePath 是初始化对象的时候从构造函数传递进来的，代表当前图像存储的路径）
                Parallel.ForEach(Directory.GetDirectories(_operatePath).AsEnumerable(), (dir) =>
                //foreach (string dir in Directory.GetDirectories(_operatePath))
                {
                    DirectoryInfo di = new DirectoryInfo(dir);

                    OperateNotify(this, new InitImagesEventArgs("循环遍历SKUOrder文件夹路径:" + dir));
                    OperateNotify(this, new InitImagesEventArgs("SKUOrder名称:" + di.Name));

                    serNum = 0;//重置图像序列号
                    var HMNUM = GetHMNUMBySKUOrder(di.Name, ChannelID, ref serNum);
                    //serNum = HMNUM.MediaLibrary.Max(m => m.SerialNum);//reset serNum DataBase已经被关闭 无法再次获取
                    if (HMNUM == null)
                    {
                        lock (locker)
                        {

                            ExceptionSKU.Add(di.Name);//将异常的SKU保存起来，一次性导出到一个文件夹下面
                        }
                        //return;
                        //break;
                    }
                    else
                    {
                        CopyImagesFile(dir, HMNUM);
                    }
                    OperateNotify(this, new InitImagesEventArgs(String.Format("当前{0}的序列号为{1}已经处理完毕", di.Name, serNum)));
                    OperateNotify(this, new InitImagesEventArgs("==================END================="));

                });

                //打印异常信息
                foreach (var sku in ExceptionSKU)
                {
                    MyLog.Log(sku);
                }

                //通知图像操作结束
                OnThreadCompleted(this, new EventArgs());
            //}
            //catch (DbEntityValidationException e)
            //{
            //    OperateNotify(this, new InitImagesEventArgs("Error"));
            //    ImageLog.Error("Error");
            //    ImageLog.Error("");
            //    ImageLog.Error("");
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        ImageLog.Error("");
            //        ImageLog.Error("");
            //        //HMLog.Error("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            ImageLog.Error("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
            //            OperateNotify(this, new InitImagesEventArgs(String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage)));
            //        }
            //        ImageLog.Error("");
            //    }
            //    ImageLog.Error("");
            //    ImageLog.Error("");
            //    OperateNotify(this, new InitImagesEventArgs(""));
            //    OperateNotify(this, new InitImagesEventArgs(""));

            //}
            //catch (Exception ex)
            //{
            //    OperateNotify(this, new InitImagesEventArgs("异常：" + ex.Message));
            //    ImageLog.Error(ex.Message);
            //    ImageLog.Error(ex.StackTrace);
            //    ImageLog.Error(ex.Source);
            //    if (ex.InnerException != null)
            //    {
            //        ImageLog.Error("=====内部异常信息========");
            //        ImageLog.Error(ex.InnerException.Message);
            //        ImageLog.Error(ex.InnerException.StackTrace);
            //        ImageLog.Error(ex.InnerException.Source);
            //        if (ex.InnerException.InnerException != null)
            //        {
            //            ImageLog.Error(ex.InnerException.InnerException.Message);
            //            ImageLog.Error(ex.InnerException.InnerException.StackTrace);
            //            ImageLog.Error(ex.InnerException.InnerException.Source);
            //        }
            //    }
            //}
        }

       /// <summary>
        /// 拷贝当前文件夹（SKUOrder)及其子文件夹内所有的文件
       /// </summary>
       /// <param name="path"></param>
       /// <param name="HMNUM"></param>
        private void CopyImagesFile(string path,CMS_HMNUM HMNUM)
        {
            CopyImagesHandler(Directory.GetFiles(path, "*"), HMNUM);
            foreach (string dir in Directory.GetDirectories(path))
            {
                //递归
                CopyImagesFile(dir, HMNUM);
            }
        }


        public string[] imgFormat = {".jpg",".gif",".png","jpeg"};

         /// <summary>
         ///  将当前文件夹内所有的问价都全部拷贝到新的目录下
        /// </summary>
        /// <param name="file">当前文件夹内的文件</param>
        private void CopyImagesHandler(string[] files, CMS_HMNUM HMNUM)
        {
            //如果当前目录下不存在以HMNUM命名的文件夹，则应该创建
            string curHMPath = DestinationDir + "/" + HMNUM.HMNUM;
            if (!Directory.Exists(curHMPath))
            {
                Directory.CreateDirectory(curHMPath);
            }
            foreach (string file in files)
            {
                var fi = new FileInfo(file);
                if (imgFormat.Contains(fi.Extension.ToLower()))
                {
                    try//在循环体内捕获异常，防止出现一张图片有异常导致整个程序停止运行，同时异常讲打印出当前图像的位置和异常信息 2014年5月27日9:52:43
                    {
                        serNum++;
                        fi.Attributes = FileAttributes.Normal;//设置为可以读写
                        string newPath = curHMPath + "/" + HMNUM.HMNUM + "_" + serNum + fi.Extension;
                        string thumbnailPath = curHMPath + "/" + HMNUM.HMNUM + "_" + serNum + "_th" + fi.Extension;
                        fi.CopyTo(newPath, true);
                        //生成略缩图
                        // SmallImageGenerated(newPath, thumbnailPath, 200);
                        SamllImageGengeratedFixedHW(newPath, thumbnailPath, 200, 200);

                        int retResult = 0;
                        //插入数据库
                        using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
                        {
                            Image imgInfo = Image.FromFile(newPath);
                            MediaLibrary newMedia = new MediaLibrary
                            {
                                ProductID = HMNUM.ProductID,
                                HMNUM = HMNUM.HMNUM,
                                StockKeyID = HMNUM.StockKeyID,
                                SerialNum = serNum,
                                ImgName = HMNUM.HMNUM + "_" + serNum,
                                MediaType = 1,
                                PrimaryImage = serNum == 1 ? true : false,//处理逻辑：序列号为1说明是第一张图，则设置为默认的图像。2014年5月26日
                                fileFormat = fi.Extension,
                                fileSize = fi.Length.ToString(),
                                fileWidth = imgInfo.Width,
                                fileHeight = imgInfo.Height,
                                CreateOn = DateTime.Now,
                                CreateBy = "InitByProgram",
                                CloudStatusID = 1 // Not yet upload
                            };
                            db.MediaLibrary.Add(newMedia);
                            retResult = db.SaveChanges();
                            imgInfo.Dispose();
                        }
                        if (retResult > 0)
                        {
                            OperateNotify(this, new InitImagesEventArgs("成功插入数据库"));
                        }
                        else
                        {
                            OperateNotify(this, new InitImagesEventArgs("插入数据库失败！！！请查看相关日志！！！"));
                            ImageLog.Fatal(String.Format("尝试将路径为{0}的图像信息插入数据库失败！！", newPath));
                        }
                        OperateNotify(this, new InitImagesEventArgs(">>>>>>>>>>>>>>"));
                        OperateNotify(this, new InitImagesEventArgs(string.Format("将文件从:{0} 拷贝到", file)));
                        OperateNotify(this, new InitImagesEventArgs(string.Format("将文件从:{0} 拷贝到", newPath)));
                        OperateNotify(this, new InitImagesEventArgs("<<<<<<<<<<<<<<<<"));
                    }
                    catch (Exception ex)
                    {
                        OperateNotify(this, new InitImagesEventArgs("==在搬迁图像的时候捕获到异常，请查看日志！="));
                        ImageLog.Fatal("");
                        ImageLog.Fatal(String.Format("尝试将路径为{0}的图像搬迁到指定目录出错！", file));
                        ImageLog.Fatal(String.Format("报错信息{0}！", ex.Message));
                        ImageLog.Fatal(String.Format("堆栈信息{0}！", ex.StackTrace));
                        ImageLog.Fatal("");
                    }
                }
            }
        }
       

       /// <summary>
        /// 获取SKUOrder对应HMNUM
       /// </summary>
       /// <param name="SKUOrder"></param>
       /// <param name="ChannelID"></param>
       /// <param name="maxSerNum"></param>
       /// <returns></returns>
        private CMS_HMNUM GetHMNUMBySKUOrder(string SKUOrder, int ChannelID, ref int maxSerNum)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                CMS_HMNUM HMObj = null;
                var SKU = db.CMS_SKU.FirstOrDefault(w => w.SKU == SKUOrder && w.ChannelID == ChannelID);
                if (SKU == null)//处理找不到SKU情况（非正常情况）
                {
                    //如果关系不存在，则拿SKUOrder+Overstock到eCom获取StockKey(SKUBest),再拿SKUBest到CMS_HMNUM匹配HMNUM
                    ImageLog.Fatal(String.Format("SKUORDER为 {0} 的记录,没有关联任何HMNUM,开始去eCom库表查询关系", SKUOrder));
                    var StockKeyName = db.Database.SqlQuery<string>("select SKUBest  from eCom.dbo.SKU  where SKUOrder = '" + SKUOrder + "' and MerchantID = 'OverStock' ").FirstOrDefault();
                    if (StockKeyName == null)
                    {
                        ImageLog.Fatal(String.Format("SKUORDER为 {0} 的记录,在eCom库表里面也没有找到任何的记录。", SKUOrder));
                        ImageLog.Fatal("============End==================");
                        ImageLog.Fatal("");
                        return null;
                    }
                    //拿StockKeyName去CMS_HMNUM获取
                    var aHMNUM = db.CMS_HMNUM.FirstOrDefault(h => h.StockKey == StockKeyName);
                    if (aHMNUM == null)
                    {
                        ImageLog.Fatal(String.Format("SKUORDER为 {0} 的记录,在eCom库表里面查询到的StockKey为{1},根据此参数在CMS_HMNUM表里面查询不到任何信息！。", SKUOrder, StockKeyName));
                        ImageLog.Fatal("============End==================");
                        ImageLog.Fatal("");
                        return null;
                    }
                    ImageLog.Fatal("============End==================");
                    ImageLog.Fatal("");
                    HMObj = aHMNUM;
                }
                else //处理SKU 不为空的情况（正常情况）
                {

                    var rObj = SKU.SKU_HM_Relation;
                    if (rObj == null)
                    {
                        //如果关系不存在，则拿SKUOrder+Overstock到eCom获取StockKey(SKUBest),再拿SKUBest到CMS_HMNUM匹配HMNUM
                        ImageLog.Fatal(String.Format("SKUORDER为 {0} 的记录,没有关联任何HMNUM,开始去eCom库表查询关系", SKUOrder));
                        var StockKeyName = db.Database.SqlQuery<string>("select SKUBest  from eCom.dbo.SKU  where SKUOrder = '{0}' and MerchantID = 'OverStock' ", SKUOrder).FirstOrDefault();
                        if (StockKeyName == null)
                        {
                            ImageLog.Fatal(String.Format("SKUORDER为 {0} 的记录,在eCom库表里面也没有找到任何的记录。", SKUOrder));
                            ImageLog.Fatal("============End==================");
                            ImageLog.Fatal("");
                            return null;
                        }
                        //拿StockKeyName去CMS_HMNUM获取
                        var aHMNUM = db.CMS_HMNUM.FirstOrDefault(h => h.StockKey == StockKeyName);
                        if (aHMNUM == null)
                        {
                            ImageLog.Fatal(String.Format("SKUORDER为 {0} 的记录,在eCom库表里面查询到的StockKey为{1},根据此参数在CMS_HMNUM表里面查询不到任何信息！。", SKUOrder, StockKeyName));
                            ImageLog.Fatal("============End==================");
                            ImageLog.Fatal("");
                            return null;
                        }
                        ImageLog.Fatal("============End==================");
                        ImageLog.Fatal("");
                        HMObj = aHMNUM;
                    }
                    else //处理关不为空的情况（正常情况）
                    {
                        HMObj = rObj.CMS_HMNUM;
                    }
                }
                maxSerNum = HMObj.CMS_StockKey.MediaLibrary.Count == 0 ? 0 : HMObj.CMS_StockKey.MediaLibrary.Max(m => m.SerialNum);
                return HMObj;
            }
            //return null;
        }


        /// <summary>
        /// 线程结束事件通知
        /// </summary>
        /// <param name="sender">VssConverter</param>
        /// <param name="e">参数</param>
        protected virtual void OnThreadCompleted(object sender, EventArgs e)
        {
            if (ThreadCompleted != null)

                ThreadCompleted(sender, e);
        }


        /// <summary>
        /// 生成略缩图
        /// </summary>
        /// <param name="fileName">源文件路径</param>
        /// <param name="newFile">新文件路径</param>
        /// <param name="maxWidth">生成新文件的宽度,高度自动算</param>
        public static  void SmallImageGenerated(string fileName, string newFile, int maxWidth)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);
            System.Drawing.Imaging.ImageFormat
            thisFormat = img.RawFormat;
            int maxHeight = img.Height * maxWidth / img.Width;
            //int maxHeight = 200;
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
                outBmp.Save(newFile,
                thisFormat);
            }
            img.Dispose();
            outBmp.Dispose();
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
        
            
    }



}
