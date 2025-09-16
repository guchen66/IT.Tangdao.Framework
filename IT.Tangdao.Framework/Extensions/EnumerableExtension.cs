using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.DaoInterfaces;
using IT.Tangdao.Framework.Utilys;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 对Linq和列表的扩展方法
    /// </summary>
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

        /// <summary>
        /// 任意键值序列 → TangdaoSortedDictionary，可选自定义比较器
        /// </summary>
        public static TangdaoSortedDictionary<TKey, TValue> ToTangdaoSortedDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IComparer<TKey> comparer = null)
        {
            var dict = new TangdaoSortedDictionary<TKey, TValue>(comparer);
            foreach (var kv in source)
                dict.Add(kv.Key, kv.Value);
            return dict;
        }

        /// <summary>
        /// 兼容老版 (IDictionary) 的快捷重载
        /// </summary>
        public static TangdaoSortedDictionary<string, string> ToTangdaoSortedDictionary(this IEnumerable<DictionaryEntry> source, IComparer<string> comparer = null)
        {
            var dict = new TangdaoSortedDictionary<string, string>(comparer);
            foreach (DictionaryEntry de in source)
                dict.Add(de.Key.ToString(), de.Value.ToString());
            return dict;
        }

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

        /// <summary>
        /// 确保字符串集合不包含重复项（大小写敏感）
        /// </summary>
        public static IEnumerable<string> OnlyAdd(this IEnumerable<string> source, string newItem)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // 转换为HashSet提高查询效率
            var existingItems = new HashSet<string>(source, StringComparer.Ordinal);

            if (!existingItems.Contains(newItem))
            {
                return source.Concat(new[] { newItem });
            }

            return source; // 已存在则返回原集合
        }

        /// <summary>
        /// 确保实现IAddParent接口的对象集合不包含重复ID
        /// </summary>
        public static IEnumerable<T> OnlyAdd<T>(this IEnumerable<T> source, T newItem) where T : IAddParent
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem));

            // 检查是否已存在相同ID的项
            bool exists = source.Any(item => item.Id == newItem.Id);

            if (!exists)
            {
                return source.Concat(new[] { newItem });
            }

            return source;
        }
    }
}