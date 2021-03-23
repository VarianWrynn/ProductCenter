using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PermaisuriCMS.Common
{
    public class StringRandom
    {
        #region 字母随机数
        /// <summary>
        /// 字母随机数
        /// </summary>
        /// <param name="n">生成长度</param>
        /// <returns></returns>
        private static string RandLetter(int n)
        {
            //var arrChar = new char[]
            var arrChar = new[]
            {                
                'A','B','C','D','E','F','G','H','J','K','M','N','P','Q','R','S','T','U','V','W','X','Y','Z'
            };

            var num = new StringBuilder();

            var rnd = new Random(DateTime.Now.Millisecond);
            for (var i = 0; i < n; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString(CultureInfo.InvariantCulture));

            }

            return num.ToString();
        }
        #endregion

        #region 数字随机数

        /// <summary>
        /// 数字随机数
        /// </summary>
        /// <param name="n">生成长度</param>
        /// <returns></returns>
        private static string RandDiffNum(int n)
        {
            //var arrChar = new char[] { '0', '1', '2', '3', '4', '5', '6' };
            var arrChar = new[] { '0', '1', '2', '3', '4', '5', '6' };
            if (n > 7)
            {
                return "";
            }
            var num = "";

            var rnd = new Random(DateTime.Now.Millisecond);

            //for (int i = 0; i < n; i++)
            while (true)
            {
                var s = arrChar[rnd.Next(0, 7)].ToString(CultureInfo.InvariantCulture);
                if (num.IndexOf(s, StringComparison.Ordinal) == -1)
                    num += s;
                if (num.Length >= n)
                    break;

            }
            return num;
        }


        /// <summary>
        /// 数字随机数，所有数字包含0,1
        /// </summary>
        /// <param name="n">生成长度</param>
        /// <returns></returns>
        private static string RandAllNum(int n)
        {
            var arrChar = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var num = new StringBuilder();

            var rnd = new Random(DateTime.Now.Millisecond);

            for (var i = 0; i < n; i++)
            {
                num.Append(arrChar[rnd.Next(0, 10)].ToString(CultureInfo.InvariantCulture));

            }

            return num.ToString();
        }
        #endregion

        /// <summary>
        /// 产生随机密码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string BuildPassword(int length)
        {
            var arrChar = new[]
            {
                'a','b','d','c','e','f','g','h','j','k','m','n','p','r','q','s','t','u','v','w','z','y','x',
                '2','3','4','5','6','7','8','9',
                'A','B','C','D','E','F','G','H','J','K','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
            };

            var num = new StringBuilder();

            var rnd = new Random(DateTime.Now.Millisecond);
            for (var i = 0; i < length; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString(CultureInfo.InvariantCulture));
            }
            var password = num.ToString();

            var isAccord = Regex.IsMatch(password, @"^(?![a-zA-Z]+$)(?![0-9]+$)[a-zA-Z0-9]{6,15}$");
            //如果生成的万一不是字母和数字的组合(全是数字或全是字母的情况)
            if (isAccord) return password;
            password = password.Remove(password.Length - 2);
            password += RandAllNum(1);
            password += RandLetter(1);

            return password;
        }

        /// <summary>
        /// 生成授权码字符串 规则 7位字母+3位数字 数字按顺序随机插入字符串中
        /// </summary>
        /// <returns></returns>
        public static string GetCode()
        {
            var result = RandLetter(7);
            var tamp = DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0').Substring(1, 1)
                + DateTime.Now.Second.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
            var index = RandDiffNum(3);
            for (var i = 0; i < index.Length; i++)
            {
                int num = Convert.ToInt16(index[i].ToString(CultureInfo.InvariantCulture));

                result = result.Insert(num, tamp[i].ToString(CultureInfo.InvariantCulture));
            }

            return result;
        }



        private string CreateRandomCode(int codeCount)
        {

            // 函数功能:产生数字和字符混合的随机字符串
            const string allChar = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var allCharArray = allChar.ToCharArray();
            var randomCode = "";
            var rand = new Random();
            for (var i = 0; i < codeCount; i++)
            {
                var r = rand.Next(61);
                randomCode += allCharArray.GetValue(r);
            }
            return randomCode;

        }

        private void CreateImage(string checkCode)
        {

            // 生成图象验证码函数
            var iwidth = (int)(checkCode.Length * 11.5);
            var image = new Bitmap(iwidth, 20);
            var g = Graphics.FromImage(image);
            var f = new Font("Arial", 10, System.Drawing.FontStyle.Bold);
            var b = new SolidBrush(Color.Azure);//字母白色
            //g.FillRectangle(new System.Drawing.SolidBrush(Color.Blue),0,0,image.Width, image.Height);
            g.Clear(Color.Brown);//背景灰色
            g.DrawString(checkCode, f, b, 3, 3);

            var blackPen = new Pen(Color.Black, 0);
            var rand = new Random();
            var ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //Response.ClearContent();
            //Response.ContentType = "image/Jpeg";
            //Response.BinaryWrite(ms.ToArray());
            g.Dispose();
            image.Dispose();
        }
    }
}
