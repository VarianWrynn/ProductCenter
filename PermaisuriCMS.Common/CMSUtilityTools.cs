using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Common
{
    public static class CMSUtilityTools
    {
        /// <summary>
        /// 在unix/linux或者mysql中都有类似时间戳，这个时间是从1970-1-1零点零分零秒的时间后经过的秒数。
        /// Referece address:http://outofmemory.cn/code-snippet/1730/C-jiang-unix-Timestamp-switch-Date-type-time
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // 定义其实时间
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public static long DateTime2Unix(DateTime dateTime)
        {
            //return (dateTime- new DateTime(1970,1,1).ToLocalTime()).Ticks;
            var unixTime = (dateTime.ToUniversalTime().Ticks - 621355968000000000)/10000000;
                //http://tool.chinaz.com/Tools/unixtime.aspx
            return unixTime;
        }
    }
}
