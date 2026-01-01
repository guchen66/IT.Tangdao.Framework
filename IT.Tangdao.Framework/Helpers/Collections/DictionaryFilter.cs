using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 字典过滤器，用于根据条件过滤字典内容
    /// </summary>
    public class DictionaryFilter<TKey, TValue>
    {
        private readonly TangdaoSortedDictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// 初始化字典过滤器
        /// </summary>
        internal DictionaryFilter(TangdaoSortedDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// 根据键过滤
        /// </summary>
        public TangdaoSortedDictionary<TKey, TValue> FilterByKey(Func<TKey, bool> predicate)
        {
            var result = new TangdaoSortedDictionary<TKey, TValue>(_dictionary.Comparer);
            foreach (var kvp in _dictionary.Where(kvp => predicate(kvp.Key)))
            {
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }

        /// <summary>
        /// 根据值过滤
        /// </summary>
        public TangdaoSortedDictionary<TKey, TValue> FilterByValue(Func<TValue, bool> predicate)
        {
            var result = new TangdaoSortedDictionary<TKey, TValue>(_dictionary.Comparer);
            foreach (var kvp in _dictionary.Where(kvp => predicate(kvp.Value)))
            {
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }

        /// <summary>
        /// 根据键值对过滤
        /// </summary>
        public TangdaoSortedDictionary<TKey, TValue> Filter(Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            var result = new TangdaoSortedDictionary<TKey, TValue>(_dictionary.Comparer);
            foreach (var kvp in _dictionary.Where(predicate))
            {
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }

        /// <summary>
        /// 获取前N个元素
        /// </summary>
        public TangdaoSortedDictionary<TKey, TValue> Take(int count)
        {
            var result = new TangdaoSortedDictionary<TKey, TValue>(_dictionary.Comparer);
            foreach (var kvp in _dictionary.Take(count))
            {
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }

        /// <summary>
        /// 跳过前N个元素，获取剩余元素
        /// </summary>
        public TangdaoSortedDictionary<TKey, TValue> Skip(int count)
        {
            var result = new TangdaoSortedDictionary<TKey, TValue>(_dictionary.Comparer);
            foreach (var kvp in _dictionary.Skip(count))
            {
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }
    }
}