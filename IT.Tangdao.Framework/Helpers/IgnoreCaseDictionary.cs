using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 不区分大小写、内部统一按小写存储的字典。
    /// 键类型为 string，值类型为泛型 TValue。
    /// 所有键在内部存储时都会转换为小写形式，确保大小写不敏感的查找行为。
    /// </summary>
    /// <typeparam name="TValue">字典中值的类型。</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public sealed class IgnoreCaseDictionary<TValue> : IDictionary<string, TValue>, IDictionary
    {
        /// <summary>
        /// 内部实际存储数据的字典，所有键均为小写形式。
        /// </summary>
        private readonly Dictionary<string, TValue> _core;

        /// <summary>
        /// 初始化 <see cref="IgnoreCaseDictionary{TValue}"/> 类的新实例，该实例为空且具有默认初始容量。
        /// </summary>
        public IgnoreCaseDictionary() : this(0) { }

        /// <summary>
        /// 初始化 <see cref="IgnoreCaseDictionary{TValue}"/> 类的新实例，该实例为空且具有指定的初始容量。
        /// </summary>
        /// <param name="capacity">初始容量。</param>
        /// <exception cref="ArgumentOutOfRangeException">当 capacity 小于 0 时抛出。</exception>
        public IgnoreCaseDictionary(int capacity)
        {
            _core = new Dictionary<string, TValue>(capacity, LowerCaseKeyComparer.Instance);
        }

        /// <summary>
        /// 获取或设置与指定键关联的值。
        /// </summary>
        /// <param name="key">要获取或设置的值的键。</param>
        /// <returns>与指定键关联的值。</returns>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出。</exception>
        /// <exception cref="KeyNotFoundException">当获取操作中键不存在时抛出。</exception>
        public TValue this[string key]
        {
            get => _core[ToLower(key)];
            set => _core[ToLower(key)] = value;
        }

        /// <summary>
        /// 获取包含 <see cref="IgnoreCaseDictionary{TValue}"/> 中的键的集合。
        /// 注意：返回的键均为小写形式。
        /// </summary>
        public ICollection<string> Keys => _core.Keys;

        /// <summary>
        /// 获取包含 <see cref="IgnoreCaseDictionary{TValue}"/> 中的值的集合。
        /// </summary>
        public ICollection<TValue> Values => _core.Values;

        /// <summary>
        /// 获取包含在 <see cref="IgnoreCaseDictionary{TValue}"/> 中的键/值对的数目。
        /// </summary>
        public int Count => _core.Count;

        /// <summary>
        /// 获取一个值，该值指示字典是否为只读。始终返回 false。
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// 将指定的键和值添加到字典中。
        /// </summary>
        /// <param name="key">要添加的键。</param>
        /// <param name="value">要添加的值。</param>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当字典中已存在相同键时抛出。</exception>
        public void Add(string key, TValue value) => _core.Add(ToLower(key), value);

        /// <summary>
        /// 将键值对添加到字典中。
        /// </summary>
        /// <param name="item">要添加的键值对。</param>
        /// <exception cref="ArgumentNullException">当键为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当字典中已存在相同键时抛出。</exception>
        public void Add(KeyValuePair<string, TValue> item) => Add(item.Key, item.Value);

        /// <summary>
        /// 从字典中移除所有键和值。
        /// </summary>
        public void Clear() => _core.Clear();

        /// <summary>
        /// 确定字典是否包含特定键值对。
        /// </summary>
        /// <param name="item">要定位的键值对。</param>
        /// <returns>如果找到该项，则为 true；否则为 false。</returns>
        public bool Contains(KeyValuePair<string, TValue> item) =>
            _core.TryGetValue(ToLower(item.Key), out var v) && EqualityComparer<TValue>.Default.Equals(v, item.Value);

        /// <summary>
        /// 确定字典是否包含指定键。
        /// </summary>
        /// <param name="key">要在字典中定位的键。</param>
        /// <returns>如果字典包含具有指定键的元素，则为 true；否则为 false。</returns>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出。</exception>
        public bool ContainsKey(string key) => _core.ContainsKey(ToLower(key));

        /// <summary>
        /// 从字典中移除指定键的值。
        /// </summary>
        /// <param name="key">要移除的元素的键。</param>
        /// <returns>如果成功找到并移除该元素，则为 true；否则为 false。</returns>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出。</exception>
        public bool Remove(string key) => _core.Remove(ToLower(key));

        /// <summary>
        /// 从字典中移除特定键值对。
        /// </summary>
        /// <param name="item">要移除的键值对。</param>
        /// <returns>如果成功找到并移除该项，则为 true；否则为 false。</returns>
        public bool Remove(KeyValuePair<string, TValue> item)
        {
            var k = ToLower(item.Key);
            return _core.TryGetValue(k, out var v) &&
                   EqualityComparer<TValue>.Default.Equals(v, item.Value) &&
                   _core.Remove(k);
        }

        /// <summary>
        /// 获取与指定键关联的值。
        /// </summary>
        /// <param name="key">要获取其值的键。</param>
        /// <param name="value">当此方法返回时，如果找到指定键，则返回与该键关联的值；否则返回值的默认类型。</param>
        /// <returns>如果字典包含具有指定键的元素，则为 true；否则为 false。</returns>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出。</exception>
        public bool TryGetValue(string key, out TValue value) => _core.TryGetValue(ToLower(key), out value);

        /// <summary>
        /// 从特定的 <see cref="Array"/> 索引开始，将字典的元素复制到一个 <see cref="KeyValuePair{TKey, TValue}"/> 数组中。
        /// </summary>
        /// <param name="array">作为从字典复制的元素的目标的一维数组。</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制。</param>
        /// <exception cref="ArgumentNullException">当 array 为 null 时抛出。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当 arrayIndex 小于 0 时抛出。</exception>
        /// <exception cref="ArgumentException">当可用空间从 arrayIndex 开始到数组结尾不足时抛出。</exception>
        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<string, TValue>>)_core).CopyTo(array, arrayIndex);

        /// <summary>
        /// 返回一个循环访问字典的枚举器。
        /// </summary>
        /// <returns>用于字典的枚举器。</returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() => _core.GetEnumerator();

        /// <summary>
        /// 返回一个循环访问字典的枚举器。
        /// </summary>
        /// <returns>可用于循环访问字典的 <see cref="IEnumerator"/> 对象。</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /*------------- 显式接口实现（非泛型 IDictionary） ---------------*/

        /// <summary>
        /// 获取一个值，该值指示 <see cref="IDictionary"/> 是否具有固定大小。
        /// 始终返回 false。
        /// </summary>
        bool IDictionary.IsFixedSize => false;

        /// <summary>
        /// 获取一个值，该值指示 <see cref="IDictionary"/> 是否为只读。
        /// 始终返回 false。
        /// </summary>
        bool IDictionary.IsReadOnly => false;

        /// <summary>
        /// 获取 <see cref="ICollection"/>，包含 <see cref="IDictionary"/> 的键。
        /// </summary>
        ICollection IDictionary.Keys => (ICollection)Keys;

        /// <summary>
        /// 获取 <see cref="ICollection"/>，包含 <see cref="IDictionary"/> 的值。
        /// </summary>
        ICollection IDictionary.Values => (ICollection)Values;

        /// <summary>
        /// 获取可用于同步对 <see cref="ICollection"/> 的访问的对象。
        /// 此实现不支持同步，因此抛出 <see cref="NotSupportedException"/>。
        /// </summary>
        /// <exception cref="NotSupportedException">始终抛出。</exception>
        public object SyncRoot => throw new NotSupportedException("IgnoreCaseDictionary does not support synchronization.");

        /// <summary>
        /// 获取一个值，该值指示是否同步对 <see cref="ICollection"/> 的访问（线程安全）。
        /// 始终返回 false。
        /// </summary>
        public bool IsSynchronized => false;

        /// <summary>
        /// 获取或设置与指定键关联的值。
        /// </summary>
        /// <param name="key">要获取或设置的值的键。</param>
        /// <returns>与指定键关联的值。</returns>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当 key 不是字符串类型时抛出。</exception>
        /// <exception cref="KeyNotFoundException">当获取操作中键不存在时抛出。</exception>
        object IDictionary.this[object key]
        {
            get
            {
                if (key == null) TangdaoGuards.ThrowIfNull(nameof(key));
                if (key is string stringKey)
                {
                    return this[stringKey];
                }

                throw new ArgumentException("Key must be of type string.", nameof(key));
            }
            set
            {
                if (key == null) TangdaoGuards.ThrowIfNull(nameof(key));
                if (key is string stringKey)
                {
                    this[stringKey] = (TValue)value;
                }

                throw new ArgumentException("Key must be of type string.", nameof(key));
            }
        }

        /// <summary>
        /// 将指定的键和值添加到字典中。
        /// </summary>
        /// <param name="key">要用作要添加的元素的键的对象。</param>
        /// <param name="value">要用作要添加的元素的值对象。</param>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出。</exception>
        /// <exception cref="ArgumentException">当 key 不是字符串类型时抛出。</exception>
        void IDictionary.Add(object key, object value)
        {
            if (key == null) TangdaoGuards.ThrowIfNull(nameof(key));

            if (key is string stringKey)
            {
                Add(stringKey, (TValue)value);
            }
            throw new ArgumentException("Key must be of type string.", nameof(key));
        }

        /// <summary>
        /// 确定字典是否包含指定键。
        /// </summary>
        /// <param name="key">要在字典中定位的键。</param>
        /// <returns>如果字典包含具有该键的元素，则为 true；否则为 false。</returns>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出。</exception>
        bool IDictionary.Contains(object key)
        {
            if (key == null) TangdaoGuards.ThrowIfNull(nameof(key));

            if (key is string stringKey)
            {
                ContainsKey(stringKey);
            }
            throw new ArgumentException("Key must be of type string.", nameof(key));
        }

        /// <summary>
        /// 从字典中移除具有指定键的元素。
        /// </summary>
        /// <param name="key">要移除的元素的键。</param>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出。</exception>
        void IDictionary.Remove(object key)
        {
            if (key == null) TangdaoGuards.ThrowIfNull(nameof(key));
            if (key is string stringKey)
            {
                Remove(stringKey);
            }
            throw new ArgumentException("Key must be of type string.", nameof(key));
        }

        /// <summary>
        /// 返回 <see cref="IDictionaryEnumerator"/> 对象用于字典。
        /// </summary>
        /// <returns>用于字典的 <see cref="IDictionaryEnumerator"/> 对象。</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)_core).GetEnumerator();

        /// <summary>
        /// 从特定的 <see cref="Array"/> 索引开始，将 <see cref="ICollection"/> 的元素复制到一个 <see cref="Array"/> 中。
        /// </summary>
        /// <param name="array">作为从 <see cref="ICollection"/> 复制的元素的目标的一维 <see cref="Array"/>。</param>
        /// <param name="index">array 中从零开始的索引，从此处开始复制。</param>
        /// <exception cref="ArgumentNullException">当 array 为 null 时抛出。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当 index 小于 0 时抛出。</exception>
        /// <exception cref="ArgumentException">当 array 是多维的，或可用空间从 index 开始到数组结尾不足时抛出。</exception>
        void ICollection.CopyTo(Array array, int index) => ((ICollection)_core).CopyTo(array, index);

        /*------------- 私有工具方法 ---------------*/

        /// <summary>
        /// 将字符串转换为小写形式。
        /// </summary>
        /// <param name="key">要转换的字符串。</param>
        /// <returns>转换为小写形式的字符串。</returns>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出。</exception>
        private static string ToLower(string key) =>
            key?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(key));

        /*------------- 专用比较器 ---------------*/

        /// <summary>
        /// 用于比较字符串键的比较器，不区分大小写。
        /// </summary>
        private sealed class LowerCaseKeyComparer : IEqualityComparer<string>
        {
            /// <summary>
            /// 获取 <see cref="LowerCaseKeyComparer"/> 的单例实例。
            /// </summary>
            public static readonly LowerCaseKeyComparer Instance = new LowerCaseKeyComparer();

            /// <summary>
            /// 私有构造函数，防止外部实例化。
            /// </summary>
            private LowerCaseKeyComparer()
            { }

            /// <summary>
            /// 确定两个字符串在转换为小写后是否相等。
            /// </summary>
            /// <param name="x">要比较的第一个字符串。</param>
            /// <param name="y">要比较的第二个字符串。</param>
            /// <returns>如果两个字符串在转换为小写后相等，则为 true；否则为 false。</returns>
            public bool Equals(string x, string y) =>
                string.Equals(ToLowerInternal(x), ToLowerInternal(y), StringComparison.Ordinal);

            /// <summary>
            /// 返回指定字符串的小写形式的哈希代码。
            /// </summary>
            /// <param name="obj">要获取哈希代码的字符串。</param>
            /// <returns>指定字符串的小写形式的哈希代码。</returns>
            public int GetHashCode(string obj) => ToLowerInternal(obj).GetHashCode();

            /// <summary>
            /// 将字符串转换为小写形式，处理 null 值。
            /// </summary>
            /// <param name="s">要转换的字符串。</param>
            /// <returns>转换为小写形式的字符串，如果输入为 null 则返回空字符串。</returns>
            private static string ToLowerInternal(string s) => s?.ToLowerInvariant() ?? string.Empty;
        }
    }
}