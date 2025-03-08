using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class LinqExtension
    {
        /// <summary>
        /// 为 IEnumerable<T> 添加索引
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">源集合</param>
        /// <returns>包含元素及其索引的 IEnumerable</returns>
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }

        public static IEnumerable<(T item, int index)> FilterByPredicateWithIndex<T>(this IEnumerable<T> source, Func<(T item, int index), bool> predicate)
        {
            return source.Select((item, index) => (item, index)).Where(predicate);
        }

        public static IEnumerable<TResult> TransformWithIndex<T, TResult>(this IEnumerable<T> source, Func<(T item, int index), TResult> selector)
        {
            return source.Select((item, index) => selector((item, index)));
        }
    }
}