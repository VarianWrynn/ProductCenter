using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Common
{
   public class CMSImageTools
    {
        /// <summary>
        /// 生成略缩图，按照传递进来的图片，如果Width比height长，则以Withd=maxWH等比缩放；
        /// 如果height比width长，则以height=maxWH等比缩放(remark date:2014年3月5日)
        /// </summary>
        /// <param name="fileName">源文件路径 Formate: D:\\eCom\\Files\\123.jpg</param>
        /// <param name="newFile">新文件路径  Formate: D:\\eCom\\Files\\123.jpg</param>
        /// <param name="maxWH">生成新文件的宽度,高度自动算</param>
        //public static void SmallImageGenerator(string fileName, string newFile, int maxWidth)
        public static void SmallImageGenerator(string fileName, string newFile, int maxWH)
        {
            var maxWidth = 0;
            var maxHeight = 0;
            var img = System.Drawing.Image.FromFile(fileName);
            var thisFormat = img.RawFormat;
            if (img.Width > img.Height)//按照宽度等比缩放
            {
                maxWidth = maxWH;
                maxHeight = img.Height * maxWidth / img.Width;
            }
            else
            {
                maxHeight = maxWH;
                maxWidth = img.Width * maxHeight / img.Height;
            }

            //int maxHeight = img.Height * maxWidth / img.Width;
            var newSize = new Size(maxWidth, maxHeight);
            var outBmp = new Bitmap(newSize.Width, newSize.Height);
            var g = Graphics.FromImage(outBmp);        // 设置画布的描绘质量
            g.DrawImage(img, new System.Drawing.Rectangle(0, 0, newSize.Width, newSize.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时,设置压缩质量
            var encoderParams = new EncoderParameters();
            var quality = new long[1];
            quality[0] = 100;
            var encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象.
            var arrayIci = ImageCodecInfo.GetImageEncoders();
            var jpegIci = arrayIci.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
            //for (int x = 0; x < arrayICI.Length; x++)
            //{
            //    if (arrayICI[x].FormatDescription.Equals("JPEG"))
            //    {
            //        jpegICI = arrayICI[x];
            //        //设置JPEG编码
            //        break;
            //    }
            //}

            if (jpegIci != null)
            {
                outBmp.Save(newFile, jpegIci, encoderParams);
            }
            else
            {
                outBmp.Save(newFile, thisFormat);
            }
            img.Dispose();
            outBmp.Dispose();
        }

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
    }
}
