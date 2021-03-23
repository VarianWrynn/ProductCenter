using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Common
{
   public class Utility
    {
        /// <summary> 
        /// 取单个字符的拼音声母 
        /// </summary> 
        /// <param name="c">要转换的单个汉字</param> 
        /// <returns>拼音声母</returns> 
        public static string GetPYChar(string c)
        {
            var array = new byte[2];
            array = Encoding.Default.GetBytes(c);
            var i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "A";
            if (i < 0xB2C1) return "B";
            if (i < 0xB4EE) return "C";
            if (i < 0xB6EA) return "D";
            if (i < 0xB7A2) return "E";
            if (i < 0xB8C1) return "F";
            if (i < 0xB9FE) return "G";
            if (i < 0xBBF7) return "H";
            if (i < 0xBFA6) return "G";
            if (i < 0xC0AC) return "K";
            if (i < 0xC2E8) return "L";
            if (i < 0xC4C3) return "M";
            if (i < 0xC5B6) return "N";
            if (i < 0xC5BE) return "O";
            if (i < 0xC6DA) return "P";
            if (i < 0xC8BB) return "Q";
            if (i < 0xC8F6) return "R";
            if (i < 0xCBFA) return "S";
            if (i < 0xCDDA) return "T";
            if (i < 0xCEF4) return "W";
            if (i < 0xD1B9) return "X";
            if (i < 0xD4D1) return "Y";
            if (i < 0xD7FA) return "Z";
            return "*";
        }

       /// <summary>
        /// 通过文件流判断文件编码:
       /// </summary>
       /// <param name="stream"></param>
       /// <returns></returns>
        public static System.Text.Encoding GetFileEncode(Stream stream)
        {
            var br = new BinaryReader(stream, Encoding.Default);
            var bb = br.ReadBytes(3);
            br.Close();

            //通过头的前三位判断文件的编码
            if (bb[0] >= 0xFF)
            {
                if (bb[0] == 0xEF && bb[1] == 0xBB && bb[2] == 0xBF)
                {
                    return Encoding.UTF8;
                }
                else if (bb[0] == 0xFE && bb[1] == 0xFF)
                {
                    return Encoding.BigEndianUnicode;
                }
                else if (bb[0] == 0xFF && bb[1] == 0xFE)
                {
                    return Encoding.Unicode;
                }
                else
                {
                    return Encoding.Default;
                }
            }
            else
            {
                return Encoding.Default;
            }
        }
    }
}
