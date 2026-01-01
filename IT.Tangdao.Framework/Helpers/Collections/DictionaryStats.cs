using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 字典统计器，用于统计字典的各种信息
    /// </summary>
    public class DictionaryStats<TKey, TValue>
    {
        private readonly TangdaoSortedDictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// 初始化字典统计器
        /// </summary>
        internal DictionaryStats(TangdaoSortedDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// 获取字典大小
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// 获取键的数量
        /// </summary>
        public int KeyCount => _dictionary.Keys.Count;

        /// <summary>
        /// 获取值的数量
        /// </summary>
        public int ValueCount => _dictionary.Values.Count;

        /// <summary>
        /// 获取不同值的数量
        /// </summary>
        public int DistinctValueCount => _dictionary.Values.Distinct().Count();

        /// <summary>
        /// 获取值的频率统计
        /// </summary>
        public Dictionary<TValue, int> ValueFrequency()
        {
            var result = new Dictionary<TValue, int>();
            foreach (var value in _dictionary.Values)
            {
                if (result.ContainsKey(value))
                    result[value]++;
                else
                    result[value] = 1;
            }
            return result;
        }

        /// <summary>
        /// 获取值的最小值（如果TValue实现了IComparable）
        /// </summary>
        public TValue MinValue<TComparable>() where TComparable : IComparable<TValue>
        {
            return _dictionary.Values.Min();
        }

        /// <summary>
        /// 获取值的最大值（如果TValue实现了IComparable）
        /// </summary>
        public TValue MaxValue<TComparable>() where TComparable : IComparable<TValue>
        {
            return _dictionary.Values.Max();
        }
    }
}