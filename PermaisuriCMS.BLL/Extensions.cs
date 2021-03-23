using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;

namespace PermaisuriCMS.BLL
{

    public static class Extensions
    {

        /// <summary>
        /// 把IQueryable转为成竖线分隔的数组形式，用于Low Inventory Report报表的格式化
        /// Author: Lee ; Date: 2013年10月17日18:47:48
        /// </summary>
        /// <typeparam name="?"></typeparam>
        /// <param name="values"></param>
        /// <param name="Separator">分隔符，比如逗号，竖直线等等</param>
        /// <returns></returns>
        public static String ConvertToString(this IQueryable<string> values,char Separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var element in values)
            {
                sb.Append(element + Separator);
            }
            return sb.ToString().TrimEnd(Separator);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }

        public static IEnumerable<TResult> ForEach<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> action)
        {
            return null;
        }

        /// <summary>
        /// 如果类型是Nullable&lt;T&gt;，则返回T，否则返回自身
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNonNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        /// <summary>
        /// 是否Nullable&lt;T&gt;类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 获取Lambda表达式的参数表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static ParameterExpression[] GetParameters<T, S>(this Expression<Func<T, S>> expr)
        {
            return expr.Parameters.ToArray();
        }
    }
}
