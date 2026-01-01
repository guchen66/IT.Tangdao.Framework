using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 字典比较器，用于比较两个字典的差异
    /// </summary>
    public class DictionaryComparer<TKey, TValue>
    {
        private readonly TangdaoSortedDictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// 初始化字典比较器
        /// </summary>
        internal DictionaryComparer(TangdaoSortedDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// 比较两个字典是否相等
        /// </summary>
        public bool Equals(IDictionary<TKey, TValue> other)
        {
            if (other == null || _dictionary.Count != other.Count)
                return false;

            return _dictionary.All(kvp => other.TryGetValue(kvp.Key, out var value) && Equals(kvp.Value, value));
        }

        /// <summary>
        /// 获取两个字典的差异
        /// </summary>
        public DictionaryDiffResult<TKey, TValue> CompareWith(IDictionary<TKey, TValue> other)
        {
            var result = new DictionaryDiffResult<TKey, TValue>();

            // 找出新增的键
            result.AddedKeys = other.Keys.Except(_dictionary.Keys).ToList();

            // 找出删除的键
            result.RemovedKeys = _dictionary.Keys.Except(other.Keys).ToList();

            // 找出修改的键
            var commonKeys = _dictionary.Keys.Intersect(other.Keys);
            result.ModifiedKeys = commonKeys.Where(key => !Equals(_dictionary[key], other[key])).ToList();

            // 找出未修改的键
            result.UnchangedKeys = commonKeys.Where(key => Equals(_dictionary[key], other[key])).ToList();

            return result;
        }
    }
}