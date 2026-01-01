using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 字典转换器，用于字典与其他数据结构的转换
    /// </summary>
    public class DictionaryConverter<TKey, TValue>
    {
        private readonly TangdaoSortedDictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// 初始化字典转换器
        /// </summary>
        internal DictionaryConverter(TangdaoSortedDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// 转换为List<KeyValuePair<TKey, TValue>>
        /// </summary>
        public List<KeyValuePair<TKey, TValue>> ToList() => _dictionary.ToList();

        /// <summary>
        /// 转换为数组
        /// </summary>
        public KeyValuePair<TKey, TValue>[] ToArray() => _dictionary.ToList().ToArray();

        /// <summary>
        /// 转换为字典
        /// </summary>
        public Dictionary<TKey, TValue> ToDictionary() => new Dictionary<TKey, TValue>(_dictionary);

        /// <summary>
        /// 转换为SortedDictionary
        /// </summary>
        public SortedDictionary<TKey, TValue> ToSortedDictionary() => new SortedDictionary<TKey, TValue>(_dictionary, _dictionary.Comparer);

        /// <summary>
        /// 转换为只读字典
        /// </summary>
        public IReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary() => _dictionary.ReadOnlyView;

        /// <summary>
        /// 转换为键集合
        /// </summary>
        public HashSet<TKey> ToKeySet() => new HashSet<TKey>(_dictionary.Keys);

        /// <summary>
        /// 转换为值列表
        /// </summary>
        public List<TValue> ToValueList() => _dictionary.Values.ToList();

        /// <summary>
        /// 转换为自定义类型列表
        /// </summary>
        public List<TResult> ToCustomList<TResult>(Func<KeyValuePair<TKey, TValue>, TResult> selector) => _dictionary.Select(selector).ToList();
    }
}