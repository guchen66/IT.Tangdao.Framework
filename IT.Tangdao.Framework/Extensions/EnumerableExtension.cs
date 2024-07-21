using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// 增加一个参数，询问它是否需要转换成Array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="callback"></param>
        /// <param name="immutable"></param>
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> callback, bool immutable = false)
        {
            var collection = immutable ? @this.ToArray() : @this;

            foreach (T item in collection)
            {
                callback(item);
            }
        }

        /// <summary>
        /// 原本的ForEach没有返回值，新增的扩展带有返回值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerable<T> TryForEach<T>(this IEnumerable<T> @this, Action<T> callback)
        {
            foreach (var item in @this)
            {
                callback?.Invoke(item);
                yield return item;
            }
        }
    }
}
