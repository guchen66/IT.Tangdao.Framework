using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 字典合并器，用于智能合并多个字典
    /// </summary>
    public class DictionaryMerger<TKey, TValue>
    {
        private readonly TangdaoSortedDictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// 初始化字典合并器
        /// </summary>
        internal DictionaryMerger(TangdaoSortedDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// 合并另一个字典
        /// </summary>
        public void Merge(IDictionary<TKey, TValue> other, bool overwriteExisting = true)
        {
            _dictionary.Merge(other, overwriteExisting);
        }

        /// <summary>
        /// 合并多个字典
        /// </summary>
        public void MergeMultiple(params IDictionary<TKey, TValue>[] dictionaries)
        {
            MergeMultiple(true, dictionaries);
        }

        /// <summary>
        /// 合并多个字典
        /// </summary>
        public void MergeMultiple(bool overwriteExisting, params IDictionary<TKey, TValue>[] dictionaries)
        {
            foreach (var dict in dictionaries)
            {
                _dictionary.Merge(dict, overwriteExisting);
            }
        }

        /// <summary>
        /// 智能合并字典，使用合并策略处理冲突
        /// </summary>
        public void SmartMerge(IDictionary<TKey, TValue> other, Func<TKey, TValue, TValue, TValue> mergeStrategy)
        {
            foreach (var kvp in other)
            {
                if (_dictionary.ContainsKey(kvp.Key))
                {
                    _dictionary[kvp.Key] = mergeStrategy(kvp.Key, _dictionary[kvp.Key], kvp.Value);
                }
                else
                {
                    _dictionary.Add(kvp.Key, kvp.Value);
                }
            }
        }
    }
}