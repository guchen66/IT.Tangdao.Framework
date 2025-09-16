using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Utilys
{
    /// <summary>
    /// 基于ViewModel命名约定的隐式键展开容器字典。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class MultiKeyDictionary<TValue> : IDictionary<string, TValue>
    {
        /// <summary>
        /// 内部真字典
        /// </summary>
        private readonly Dictionary<string, TValue> _inner = new Dictionary<string, TValue>();

        /// <summary>
        /// 把任意 key 映射到“裸名”
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string CoreOf(string key)
        {
            if (key.EndsWith("ViewModel")) return key.Substring(0, key.Length - 9);
            if (key.EndsWith("View")) return key.Substring(0, key.Length - 4);
            if (key.EndsWith("Page")) return key.Substring(0, key.Length - 4);
            return key;
        }

        /// <summary>
        /// 根据裸名展开成 3 个 key
        /// </summary>
        /// <param name="core"></param>
        /// <returns></returns>
        private static IEnumerable<string> AllKeys(string core)
        {
            yield return core;
            yield return core + "View";
            yield return core + "ViewModel";
        }

        /*============ 核心拦截 ============*/

        public TValue this[string key]
        {
            get => _inner[key];
            set
            {
                var core = CoreOf(key);
                foreach (var k in AllKeys(core))
                    _inner[k] = value;   // 一次性写全部
            }
        }

        /*============ 其余接口直接委托 ============*/
        public int Count => _inner.Count;
        public bool IsReadOnly => false;
        public ICollection<string> Keys => _inner.Keys;
        public ICollection<TValue> Values => _inner.Values;

        public void Add(string key, TValue value) => this[key] = value;

        public bool ContainsKey(string key) => _inner.ContainsKey(key);

        public bool Remove(string key)
        {
            var core = CoreOf(key);
            bool ok = false;
            foreach (var k in AllKeys(core))
                ok |= _inner.Remove(k);
            return ok;
        }

        public bool TryGetValue(string key, out TValue value) => _inner.TryGetValue(key, out value);

        public void Clear() => _inner.Clear();

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() => _inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /* 未实现接口，按需补全 */

        void ICollection<KeyValuePair<string, TValue>>.Add(KeyValuePair<string, TValue> item) => Add(item.Key, item.Value);

        bool ICollection<KeyValuePair<string, TValue>>.Remove(KeyValuePair<string, TValue> item) => Remove(item.Key);

        bool ICollection<KeyValuePair<string, TValue>>.Contains(KeyValuePair<string, TValue> item) => _inner.Contains(item);

        void ICollection<KeyValuePair<string, TValue>>.CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<string, TValue>>)_inner).CopyTo(array, arrayIndex);
    }
}