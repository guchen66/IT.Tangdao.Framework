using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Providers
{
    public class TangdaoSortProvider
    {
        /// <summary>
        /// 用 TangdaoSortedDictionary 做底层映射，支持 Key 有序遍历
        /// </summary>
        public static IComparer<T> Priority<T>(Func<T, string> keySelector,
                                               IEnumerable<KeyValuePair<string, int>> rules)
        {
            var dict = new TangdaoSortedDictionary<string, int>();
            foreach (var r in rules) dict.Add(r.Key, r.Value);
            return new PrioritySortProvider<T>(keySelector, dict);
        }

        /// <summary>
        /// 极简委托排序（无字典，最快）
        /// </summary>
        public static IComparer<T> Delegate<T>(Func<T, T, int> compare)
        {
            return new AnonymousComparer<T>(compare);
        }

        private sealed class AnonymousComparer<T> : IComparer<T>
        {
            private readonly Func<T, T, int> _compare;

            public AnonymousComparer(Func<T, T, int> compare)
            {
                _compare = compare;
            }

            public int Compare(T x, T y) => _compare(x, y);
        }
    }

    /// <summary>
    /// 任何具备“可排序分类”的模型，都实现此接口
    /// </summary>
    public interface IPrioritySortable
    {
        /// <summary>返回用于排序的“分类字符串”</summary>
        string SortCategory { get; }
    }

    /// <summary>
    /// 优先级规则源（策略）
    /// </summary>
    public interface IPriorityRule<TKey>
    {
        /// <summary>返回分类 -> 权重映射</summary>
        IReadOnlyDictionary<TKey, int> GetPriorityMap();
    }
}