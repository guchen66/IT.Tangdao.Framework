using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.EventArg;
using IT.Tangdao.Framework.Events;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 自动按键排序的字典——int、string 即插即排，后续可扩展汉字规则。
    /// </summary>
    public class TangdaoSortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, INotifyPropertyChanged
    {
        // 内部用 Framework 自带的 SortedDictionary，红黑树实现，插入即排序
        private readonly SortedDictionary<TKey, TValue> _core;

        /// <summary>
        /// 当字典属性变化时发生
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 当字典内容变化时发生
        /// </summary>
        public event EventHandler<DictionaryChangedEventArgs<TKey, TValue>> DictionaryChanged;

        /// <inheritdoc/>
        public TangdaoSortedDictionary() : this(null)
        {
        }

        /// <inheritdoc/>
        public TangdaoSortedDictionary(IComparer<TKey> comparer)
        {
            _core = new SortedDictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        /// 触发属性变化事件
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 触发字典变化事件
        /// </summary>
        protected void OnDictionaryChanged(DictionaryChangedAction action, TKey key, TValue oldValue = default, TValue newValue = default)
        {
            DictionaryChanged?.Invoke(this, new DictionaryChangedEventArgs<TKey, TValue>(action, key, oldValue, newValue));
        }

        #region 字典功能扩展属性

        private DictionaryConverter<TKey, TValue> _converter;

        /// <summary>
        /// 字典转换器，用于字典与其他数据结构的转换
        /// </summary>
        public DictionaryConverter<TKey, TValue> Converter
        {
            get
            {
                if (_converter == null)
                {
                    _converter = new DictionaryConverter<TKey, TValue>(this);
                }
                return _converter;
            }
        }

        private DictionarySerializer<TKey, TValue> _serializer;

        /// <summary>
        /// 字典序列化器，用于字典的序列化和反序列化
        /// </summary>
        public DictionarySerializer<TKey, TValue> Serializer
        {
            get
            {
                if (_serializer == null)
                {
                    _serializer = new DictionarySerializer<TKey, TValue>(this);
                }
                return _serializer;
            }
        }

        private DictionaryComparer<TKey, TValue> _comparer;

        /// <summary>
        /// 字典比较器，用于比较两个字典的差异
        /// </summary>
        public DictionaryComparer<TKey, TValue> ComparerHelper
        {
            get
            {
                if (_comparer == null)
                {
                    _comparer = new DictionaryComparer<TKey, TValue>(this);
                }
                return _comparer;
            }
        }

        private DictionaryFilter<TKey, TValue> _filter;

        /// <summary>
        /// 字典过滤器，用于根据条件过滤字典内容
        /// </summary>
        public DictionaryFilter<TKey, TValue> Filter
        {
            get
            {
                if (_filter == null)
                {
                    _filter = new DictionaryFilter<TKey, TValue>(this);
                }
                return _filter;
            }
        }

        private DictionaryStats<TKey, TValue> _stats;

        /// <summary>
        /// 字典统计器，用于统计字典的各种信息
        /// </summary>
        public DictionaryStats<TKey, TValue> Stats
        {
            get
            {
                if (_stats == null)
                {
                    _stats = new DictionaryStats<TKey, TValue>(this);
                }
                return _stats;
            }
        }

        private DictionaryMerger<TKey, TValue> _merger;

        /// <summary>
        /// 字典合并器，用于智能合并多个字典
        /// </summary>
        public DictionaryMerger<TKey, TValue> Merger
        {
            get
            {
                if (_merger == null)
                {
                    _merger = new DictionaryMerger<TKey, TValue>(this);
                }
                return _merger;
            }
        }

        #endregion 字典功能扩展属性

        /*=========== IDictionary 接口直接代理 ===========*/

        public TValue this[TKey key]
        {
            get => _core[key];
            set
            {
                bool exists = _core.TryGetValue(key, out TValue oldValue);
                _core[key] = value;

                if (exists)
                {
                    OnPropertyChanged("Item");
                    OnDictionaryChanged(DictionaryChangedAction.Update, key, oldValue, value);
                }
                else
                {
                    OnPropertyChanged("Item");
                    OnPropertyChanged(nameof(Count));
                    OnPropertyChanged(nameof(IsEmpty));
                    OnPropertyChanged(nameof(FirstKey));
                    OnPropertyChanged(nameof(FirstValue));
                    OnPropertyChanged(nameof(LastKey));
                    OnPropertyChanged(nameof(LastValue));
                    OnPropertyChanged(nameof(KeysArray));
                    OnPropertyChanged(nameof(ValuesArray));
                    OnDictionaryChanged(DictionaryChangedAction.Add, key, default, value);
                }
            }
        }

        /// <inheritdoc/>
        public ICollection<TKey> Keys => _core.Keys;

        /// <inheritdoc/>
        public ICollection<TValue> Values => _core.Values;

        /// <inheritdoc/>
        public int Count => _core.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _core.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _core.Values;

        /// <summary>
        /// 获取用于对字典中的键进行排序的比较器
        /// </summary>
        public IComparer<TKey> Comparer => _core.Comparer;

        /// <summary>
        /// 获取一个值，指示字典是否为空
        /// </summary>
        public bool IsEmpty => _core.Count == 0;

        /// <summary>
        /// 获取字典中的第一个键（最小键）
        /// </summary>
        public TKey FirstKey => IsEmpty ? throw new InvalidOperationException("字典为空") : _core.Keys.First();

        /// <summary>
        /// 获取字典中的第一个值（最小键对应的值）
        /// </summary>
        public TValue FirstValue => IsEmpty ? throw new InvalidOperationException("字典为空") : _core.Values.First();

        /// <summary>
        /// 获取字典中的最后一个键（最大键）
        /// </summary>
        public TKey LastKey => IsEmpty ? throw new InvalidOperationException("字典为空") : _core.Keys.Last();

        /// <summary>
        /// 获取字典中的最后一个值（最大键对应的值）
        /// </summary>
        public TValue LastValue => IsEmpty ? throw new InvalidOperationException("字典为空") : _core.Values.Last();

        /// <summary>
        /// 获取字典的只读视图
        /// </summary>
        public IReadOnlyDictionary<TKey, TValue> ReadOnlyView => new ReadOnlyDictionary<TKey, TValue>(this);

        /// <summary>
        /// 获取字典中所有键的数组
        /// </summary>
        public TKey[] KeysArray => _core.Keys.ToArray();

        /// <summary>
        /// 获取字典中所有值的数组
        /// </summary>
        public TValue[] ValuesArray => _core.Values.ToArray();

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            _core.Add(key, value);

            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(FirstKey));
            OnPropertyChanged(nameof(FirstValue));
            OnPropertyChanged(nameof(LastKey));
            OnPropertyChanged(nameof(LastValue));
            OnPropertyChanged(nameof(KeysArray));
            OnPropertyChanged(nameof(ValuesArray));
            OnDictionaryChanged(DictionaryChangedAction.Add, key, default, value);
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> kv)
        {
            Add(kv.Key, kv.Value);
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key) => _core.ContainsKey(key);

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            bool removed = _core.TryGetValue(key, out TValue oldValue) && _core.Remove(key);

            if (removed)
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(FirstKey));
                OnPropertyChanged(nameof(FirstValue));
                OnPropertyChanged(nameof(LastKey));
                OnPropertyChanged(nameof(LastValue));
                OnPropertyChanged(nameof(KeysArray));
                OnPropertyChanged(nameof(ValuesArray));
                OnDictionaryChanged(DictionaryChangedAction.Remove, key, oldValue, default);
            }

            return removed;
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue v) => _core.TryGetValue(key, out v);

        /// <inheritdoc/>
        public void Clear()
        {
            _core.Clear();

            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(FirstKey));
            OnPropertyChanged(nameof(FirstValue));
            OnPropertyChanged(nameof(LastKey));
            OnPropertyChanged(nameof(LastValue));
            OnPropertyChanged(nameof(KeysArray));
            OnPropertyChanged(nameof(ValuesArray));
            OnDictionaryChanged(DictionaryChangedAction.Clear, default, default, default);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _core.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> kv) => ((ICollection<KeyValuePair<TKey, TValue>>)_core).Contains(kv);

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int idx) => ((ICollection<KeyValuePair<TKey, TValue>>)_core).CopyTo(array, idx);

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> kv) => Remove(kv.Key);

        public void UpdateValues(List<TValue> allDatas)
        {
            if (allDatas == null)
                throw new ArgumentNullException(nameof(allDatas));

            if (allDatas.Count != this.Count)
                throw new ArgumentException("数据数量与key数量不匹配", nameof(allDatas));

            // 使用 Zip 组合，然后逐个更新
            var updates = this.Keys.Zip(allDatas, (key, value) => new { Key = key, Value = value });

            foreach (var item in updates)
            {
                this[item.Key] = item.Value;
            }
        }

        #region 扩展功能

        /// <summary>
        /// 获取指定范围内的键值对
        /// </summary>
        public IEnumerable<KeyValuePair<TKey, TValue>> GetRange(TKey start, TKey end, bool includeStart = true, bool includeEnd = true)
        {
            return _core.Where(kvp =>
            {
                int compareToStart = Comparer.Compare(kvp.Key, start);
                int compareToEnd = Comparer.Compare(kvp.Key, end);

                bool inStartRange = includeStart ? compareToStart >= 0 : compareToStart > 0;
                bool inEndRange = includeEnd ? compareToEnd <= 0 : compareToEnd < 0;

                return inStartRange && inEndRange;
            });
        }

        /// <summary>
        /// 获取指定范围内的键值对
        /// </summary>
        public IEnumerable<KeyValuePair<TKey, TValue>> GetRange(TangdaoKeyRange range)
        {
            if (range == null)
                throw new ArgumentNullException(nameof(range));

            return GetRange(range.Start, range.End, range.IncludeStart, range.IncludeEnd);
        }

        /// <summary>
        /// 创建批量更新对象
        /// </summary>
        public BatchUpdate CreateBatchUpdate()
        {
            return new BatchUpdate(this);
        }

        /// <summary>
        /// 创建变化跟踪器
        /// </summary>
        public ChangeTracker CreateChangeTracker()
        {
            return new ChangeTracker(this);
        }

        /// <summary>
        /// 合并另一个字典到当前字典
        /// </summary>
        public void Merge(IDictionary<TKey, TValue> other, bool overwriteExisting = true)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            foreach (var kvp in other)
            {
                if (overwriteExisting || !ContainsKey(kvp.Key))
                {
                    this[kvp.Key] = kvp.Value;
                }
            }
        }

        /// <summary>
        /// 根据键分割字典，返回两个新字典
        /// </summary>
        public (TangdaoSortedDictionary<TKey, TValue> Lower, TangdaoSortedDictionary<TKey, TValue> Upper) Split(TKey splitKey)
        {
            var lower = new TangdaoSortedDictionary<TKey, TValue>(Comparer);
            var upper = new TangdaoSortedDictionary<TKey, TValue>(Comparer);

            foreach (var kvp in _core)
            {
                if (Comparer.Compare(kvp.Key, splitKey) < 0)
                {
                    lower.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    upper.Add(kvp.Key, kvp.Value);
                }
            }

            return (lower, upper);
        }

        /// <summary>
        /// 查找小于指定键的最大键
        /// </summary>
        public bool TryGetClosestLowerKey(TKey key, out TKey closestKey)
        {
            var lowerKeys = _core.Keys.Where(k => Comparer.Compare(k, key) < 0);
            if (lowerKeys.Any())
            {
                closestKey = lowerKeys.Last();
                return true;
            }

            closestKey = default;
            return false;
        }

        /// <summary>
        /// 查找大于指定键的最小键
        /// </summary>
        public bool TryGetClosestUpperKey(TKey key, out TKey closestKey)
        {
            var upperKeys = _core.Keys.Where(k => Comparer.Compare(k, key) > 0);
            if (upperKeys.Any())
            {
                closestKey = upperKeys.First();
                return true;
            }

            closestKey = default;
            return false;
        }

        /// <summary>
        /// 根据条件批量删除键值对
        /// </summary>
        public int RemoveWhere(Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var keysToRemove = _core.Where(predicate).Select(kvp => kvp.Key).ToList();
            int count = keysToRemove.Count;

            foreach (var key in keysToRemove)
            {
                Remove(key);
            }

            return count;
        }

        /// <summary>
        /// 将字典转换为列表
        /// </summary>
        public List<KeyValuePair<TKey, TValue>> ToList()
        {
            return _core.ToList();
        }

        /// <summary>
        /// 深拷贝字典
        /// </summary>
        public TangdaoSortedDictionary<TKey, TValue> DeepCopy()
        {
            var copy = new TangdaoSortedDictionary<TKey, TValue>(Comparer);
            foreach (var kvp in _core)
            {
                copy.Add(kvp.Key, kvp.Value);
            }
            return copy;
        }

        /// <summary>
        /// 获取指定键的排名（从0开始）
        /// </summary>
        public int GetKeyRank(TKey key)
        {
            if (!ContainsKey(key))
                throw new KeyNotFoundException($"键 '{key}' 不存在于字典中");

            return _core.Keys.TakeWhile(k => !Equals(k, key)).Count();
        }

        /// <summary>
        /// 获取指定排名的键（从0开始）
        /// </summary>
        public TKey GetKeyByRank(int rank)
        {
            if (rank < 0 || rank >= Count)
                throw new ArgumentOutOfRangeException(nameof(rank), "排名超出范围");

            return _core.Keys.ElementAt(rank);
        }

        /// <summary>
        /// 交换两个键的值
        /// </summary>
        public void SwapValues(TKey key1, TKey key2)
        {
            if (!ContainsKey(key1))
                throw new KeyNotFoundException($"键 '{key1}' 不存在于字典中");
            if (!ContainsKey(key2))
                throw new KeyNotFoundException($"键 '{key2}' 不存在于字典中");

            TValue temp = this[key1];
            this[key1] = this[key2];
            this[key2] = temp;
        }

        #endregion 扩展功能

        #region 内部类

        /// <summary>
        /// 表示键的范围
        /// </summary>
        public class TangdaoKeyRange
        {
            /// <summary>
            /// 起始键（包含）
            /// </summary>
            public TKey Start { get; set; }

            /// <summary>
            /// 结束键（包含）
            /// </summary>
            public TKey End { get; set; }

            /// <summary>
            /// 是否包含起始键
            /// </summary>
            public bool IncludeStart { get; set; } = true;

            /// <summary>
            /// 是否包含结束键
            /// </summary>
            public bool IncludeEnd { get; set; } = true;

            /// <summary>
            /// 初始化范围对象
            /// </summary>
            public TangdaoKeyRange(TKey start, TKey end)
            {
                Start = start;
                End = end;
            }

            /// <summary>
            /// 初始化范围对象
            /// </summary>
            public TangdaoKeyRange(TKey start, TKey end, bool includeStart, bool includeEnd)
            {
                Start = start;
                End = end;
                IncludeStart = includeStart;
                IncludeEnd = includeEnd;
            }
        }

        /// <summary>
        /// 批量更新助手类
        /// </summary>
        public class BatchUpdate
        {
            private readonly TangdaoSortedDictionary<TKey, TValue> _dictionary;
            private readonly Dictionary<TKey, TValue> _updates = new Dictionary<TKey, TValue>();

            /// <summary>
            /// 初始化批量更新对象
            /// </summary>
            internal BatchUpdate(TangdaoSortedDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            /// <summary>
            /// 添加或更新键值对
            /// </summary>
            public BatchUpdate Set(TKey key, TValue value)
            {
                _updates[key] = value;
                return this;
            }

            /// <summary>
            /// 移除指定键
            /// </summary>
            public BatchUpdate Remove(TKey key)
            {
                _updates[key] = default;
                return this;
            }

            /// <summary>
            /// 执行批量更新
            /// </summary>
            public void Execute()
            {
                foreach (var kvp in _updates)
                {
                    _dictionary[kvp.Key] = kvp.Value;
                }
            }

            /// <summary>
            /// 取消批量更新
            /// </summary>
            public void Cancel()
            {
                _updates.Clear();
            }
        }

        /// <summary>
        /// 字典变化跟踪器
        /// </summary>
        public class ChangeTracker
        {
            private readonly TangdaoSortedDictionary<TKey, TValue> _dictionary;
            private Dictionary<TKey, TValue> _snapshot;

            /// <summary>
            /// 初始化变化跟踪器
            /// </summary>
            internal ChangeTracker(TangdaoSortedDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                TakeSnapshot();
            }

            /// <summary>
            /// 拍摄字典当前状态的快照
            /// </summary>
            public void TakeSnapshot()
            {
                _snapshot = new Dictionary<TKey, TValue>(_dictionary);
            }

            /// <summary>
            /// 获取自快照以来添加的键
            /// </summary>
            public IEnumerable<TKey> AddedKeys => _dictionary.Keys.Except(_snapshot.Keys);

            /// <summary>
            /// 获取自快照以来移除的键
            /// </summary>
            public IEnumerable<TKey> RemovedKeys => _snapshot.Keys.Except(_dictionary.Keys);

            /// <summary>
            /// 获取自快照以来修改的键
            /// </summary>
            public IEnumerable<TKey> ModifiedKeys
            {
                get
                {
                    var commonKeys = _dictionary.Keys.Intersect(_snapshot.Keys);
                    return commonKeys.Where(key => !Equals(_dictionary[key], _snapshot[key]));
                }
            }

            /// <summary>
            /// 检查字典是否发生了变化
            /// </summary>
            public bool HasChanges => AddedKeys.Any() || RemovedKeys.Any() || ModifiedKeys.Any();
        }

        #endregion 内部类
    }
}