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

        public TangdaoSortedDictionary() : this(null)
        {
        }

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

        public ICollection<TKey> Keys => _core.Keys;
        public ICollection<TValue> Values => _core.Values;
        public int Count => _core.Count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value) => _core.Add(key, value);

        public void Add(KeyValuePair<TKey, TValue> kv) => _core.Add(kv.Key, kv.Value);

        public bool ContainsKey(TKey key) => _core.ContainsKey(key);

        public bool Remove(TKey key) => _core.Remove(key);

        public bool TryGetValue(TKey key, out TValue v) => _core.TryGetValue(key, out v);

        public void Clear() => _core.Clear();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _core.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(KeyValuePair<TKey, TValue> kv) => ((ICollection<KeyValuePair<TKey, TValue>>)_core).Contains(kv);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int idx) => ((ICollection<KeyValuePair<TKey, TValue>>)_core).CopyTo(array, idx);

        public bool Remove(KeyValuePair<TKey, TValue> kv) => Remove(kv.Key);
    }
}