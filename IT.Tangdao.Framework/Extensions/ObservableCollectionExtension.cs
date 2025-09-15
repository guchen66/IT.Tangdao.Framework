using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.DaoTasks;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 实现IEnumerable转ObservableCollection
    /// </summary>
    public static class ObservableCollectionExtension
    {
        /// <summary>
        /// 列表转ObservableCollection的扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return new ObservableCollection<T>(source);
        }

        /// <summary>
        /// 临时实现，复杂度 O(n²)
        /// 方法未优化，尽量数据少使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        /// <exception cref="ArgumentNullException"></exception>
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
}