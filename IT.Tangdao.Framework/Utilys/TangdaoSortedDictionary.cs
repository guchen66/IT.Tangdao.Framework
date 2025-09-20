using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Utilys
{
    /// <summary>
    /// 自动按键排序的字典——int、string 即插即排，后续可扩展汉字规则。
    /// </summary>
    public class TangdaoSortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        // 内部用 Framework 自带的 SortedDictionary，红黑树实现，插入即排序
        private readonly SortedDictionary<TKey, TValue> _core;

        /// <inheritdoc/>
        public TangdaoSortedDictionary() : this(null)
        {
        }

        /// <inheritdoc/>
        public TangdaoSortedDictionary(IComparer<TKey> comparer)
        {
            _core = new SortedDictionary<TKey, TValue>(comparer);
        }

        /*=========== IDictionary 接口直接代理 ===========*/

        public TValue this[TKey key]
        {
            get => _core[key];
            set => _core[key] = value;
        }

        /// <inheritdoc/>
        public ICollection<TKey> Keys => _core.Keys;

        /// <inheritdoc/>
        public ICollection<TValue> Values => _core.Values;

        /// <inheritdoc/>
        public int Count => _core.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(TKey key, TValue value) => _core.Add(key, value);

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> kv) => _core.Add(kv.Key, kv.Value);

        /// <inheritdoc/>
        public bool ContainsKey(TKey key) => _core.ContainsKey(key);

        /// <inheritdoc/>
        public bool Remove(TKey key) => _core.Remove(key);

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue v) => _core.TryGetValue(key, out v);

        /// <inheritdoc/>
        public void Clear() => _core.Clear();

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _core.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> kv) => ((ICollection<KeyValuePair<TKey, TValue>>)_core).Contains(kv);

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int idx) => ((ICollection<KeyValuePair<TKey, TValue>>)_core).CopyTo(array, idx);

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> kv) => Remove(kv.Key);
    }
}