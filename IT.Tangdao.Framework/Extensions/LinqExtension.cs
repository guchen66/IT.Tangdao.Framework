using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
        public static IEnumerable<T> OnlyAdd<T>(this IEnumerable<T> source, T newItem)
            where T : IAddParent
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

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (items == null) throw new ArgumentNullException(nameof(items));

            TangdaoTaskScheduler.Execute(dao: daoTask =>
            {
                // 先检查是否有元素
                var itemList = items as IList<T> ?? items.ToList();
                if (itemList.Count == 0) return;

                foreach (var item in itemList)
                {
                    collection.Add(item);
                }
            });
        }
    }

    public interface IAddParent
    {
        int Id { get; set; }
    }
}