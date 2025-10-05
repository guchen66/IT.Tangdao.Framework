using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Interfaces;
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
        /// 对序列按索引赋值，指定成员表达式 item => item.Id
        /// </summary>
        public static IEnumerable<T> SelectWithIndex<T>(this IEnumerable<T> source, Expression<Func<T, int>> memberSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (memberSelector == null) throw new ArgumentNullException(nameof(memberSelector));

            // 编译一次 setter
            var setter = GetOrCreateSetter(memberSelector);

            return source.Select((item, idx) =>
            {
                setter(item, idx);
                return item;
            });
        }

        // 内部：把 Expression<item.Id> 编译成 Action<item,index>
        private static readonly ConcurrentDictionary<(Type, string), Delegate> _cache = new ConcurrentDictionary<(Type, string), Delegate>();

        private static Action<T, int> GetOrCreateSetter<T>(Expression<Func<T, int>> selector)
        {
            var type = typeof(T);
            var key = (type, selector.Body.ToString());

            return (Action<T, int>)_cache.GetOrAdd(key, _ =>
            {
                // 下面是原逻辑
                if (!(selector.Body is MemberExpression mem) ||
                    !(mem.Member is PropertyInfo prop) ||
                    !prop.CanWrite ||
                    prop.PropertyType != typeof(int))
                    throw new ArgumentException("Lambda 必须返回一个可写的 int 属性，如 item => item.Id");

                var paramItem = Expression.Parameter(type, "item");
                var paramIndex = Expression.Parameter(typeof(int), "index");
                var assign = Expression.Assign(Expression.Property(paramItem, prop), paramIndex);
                return Expression.Lambda<Action<T, int>>(assign, paramItem, paramIndex).Compile();
            });
        }
    }
}