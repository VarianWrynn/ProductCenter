using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using PermaisuriCMS.Common;
using PermaisuriCMS.DAL;

namespace PermaisuriCMS.BLL
{
    public static class BllExtention
    {

        /// <summary>
        ///  该方法与过期的方法FormsAuthentication.HashPasswordForStoringInConfigFile(user.User_Pwd, "MD5");一样的作用
        ///  即使传递进来的字符串是汉字
        ///  Author: Lee ; Date:2013年10月23日11:31:14 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetPwdmd5(this string value)
        {
            var algorithm = MD5.Create();
            var data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            return data.Aggregate("", (current, t) => current + t.ToString("x2").ToUpperInvariant());
        }


        /// <summary>
        /// 对字符串时间扩展，字符串时间调用该方法可以转化为yyyy-MM-dd HH:mm:ss格式的时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime? ConvertToDate(this string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;
            return DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null);
        }

        /// <summary>
        /// 对Nullabe《Datetime》的时间格式扩展，，调用该方法可以转为yyyy-MM-dd HH:mm:ss的字符串时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ConvertToString(this DateTime? date)
        {
            return date == null ? string.Empty : date.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ConvertToStringWithoutHours(this DateTime? date)
        {
            return date == null ? string.Empty : date.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 当前值如果是null，则返回0，否则返回当前值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ConvertToNotNull(this int? val)
        {
            return val.HasValue ? val.Value : 0;
        }

        public static long ConvertToNotNull(this long? val)
        {
            return val.HasValue ? val.Value : 0;
        }

        public static double ConvertToNotNull(this double? val)
        {
            return val.HasValue ? val.Value : 0;
        }

        public static decimal ConvertToNotNull(this decimal? val)
        {
            return val.HasValue ? val.Value : 0;
        }

        /// <summary>
        /// 设置bool的扩展方法，如果是null,则返回false
        /// CreateDate:2013年11月21日18:46:50
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool ConvertToNotNull(this bool? val)
        {
            //return val.HasValue ? val.Value : false;
            return val.HasValue && val.Value;
        }

        /// <summary>
        /// 扩展一个DistinctBy。这个扩展方法还是很不错的，用起来很简洁，适合为框架添加的Distinct扩展方法。
        /// http://www.cnblogs.com/A_ming/archive/2013/05/24/3097062.html
        /// 2014年10月13日
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            //foreach (TSource element in source)
            //{
            //    if (seenKeys.Add(keySelector(element)))
            //    {
            //        yield return element;
            //    }
            //}
            return source.Where(element => seenKeys.Add(keySelector(element)));
            /*
             var list = _repository.Gets(SaleStatisticalSpecifications.PersonStatisticalCondition(condition)).ToList();
             *   var empLits = list.Select(x => new Employee
            {
                Id = x.EmployeeId.ConvertToNotNull(),
                TenantId = x.TenantId.ConvertToNotNull()
            }).DistinctBy(x => new 
            {
                Id = x.Id,
                TenantId = x.TenantId
            });
             */
        }


        //public static List<T> GetListOf<T>(Expression<Func<T, bool>> expression) where T : class
        //{
        //    using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
        //    {
        //        return db.Set<T>().Where(expression).ToList();
        //    }
        //}

        //public static List<T> GetListOf<T>(Expression<Func<T, bool>> expression) where T : class
        //{
        //    ProductsEntities _context = new ProductsEntities();
        //    return _context.CreateObjectSet<T>().Where(expression).ToList();
        //}

        /// <summary>
        /// 这个方法用户记录数据库所有产生变化的字段，方便迅捷，但是缺点就是像ID这样的字段，只能记录ID不能记录名称，不方面前端知直接查看：
        /// 比如:[ColourID] : old value = 307 , new Value = 304,另外对于强类型新增的对象的ID，记录为0....
        /// 2014年3月10日
        /// </summary>
        /// <param name="table"></param>
        /// <param name="logDetail"></param>
        public static void DbRecorder(DbEntityEntry table, LogOfUserOperatingDetails logDetail)
        {
            var logDesc = string.Empty;
            var entry = table;
            var currentPropertyNames = entry.CurrentValues.PropertyNames;//OriginalValues
            var modifiedProperties = from name in currentPropertyNames
                                                     where entry.Property(name).IsModified
                                                     select name;

            foreach (var propertyName in modifiedProperties)
            {
                logDesc += " <b> [" + propertyName + "] </b> : old value =  <span style='color:red'>  " + HttpUtility.HtmlEncode(entry.OriginalValues[propertyName]) + " </span> , new Value = <span style='color:red'>  " + HttpUtility.HtmlEncode(entry.CurrentValues[propertyName]) + "  </span>";
                logDesc += "<br>";

            }
            logDetail.Descriptions = logDesc;
            DbRecorder(logDetail);

        }

        /// <summary>
        /// 这个方法不解释...
        /// </summary>
        /// <param name="logDetail"></param>
        public static void DbRecorder(LogOfUserOperatingDetails logDetail)
        {
            IDictionary<string, string> ls = new Dictionary<string, string>();
            ls.Add("ModelName", logDetail.ModelName);
            ls.Add("ActionName", logDetail.ActionName);
            ls.Add("ActionType", logDetail.ActionType.ToString(CultureInfo.InvariantCulture));

            ls.Add("SKU", logDetail.SKU);
            ls.Add("SKUID", logDetail.SKUID.ToString());
            ls.Add("ChannelID", logDetail.ChannelID.ToString());
            ls.Add("ChannelName", logDetail.ChannelName);
            ls.Add("ProductID", logDetail.ProductID.ToString());
            ls.Add("HMNUM", logDetail.HMNUM.ToString(CultureInfo.InvariantCulture));

            ls.Add("Descriptions", logDetail.Descriptions);
            ls.Add("CreateOn", logDetail.CreateOn.ToString("yyyy-MM-dd HH:mm:ss"));
            ls.Add("CreateBy", logDetail.CreateBy);
            NBCMSLoggerManager.NBCMSLogger("OperatingDetail", "", ls);
        }


        
   //字符型转换 转为字符串  
   //12345.ToString("n");        //生成   12,345.00  
   //12345.ToString("C");        //生成 ￥12,345.00  
   //12345.ToString("e");        //生成 1.234500e+004  
   //12345.ToString("f4");        //生成 12345.0000  
   //12345.ToString("x");         //生成 3039  (16进制)  
   //12345.ToString("p");         //生成 1,234,500.00%  

    }
}
