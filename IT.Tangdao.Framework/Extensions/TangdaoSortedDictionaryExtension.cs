using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoSortedDictionaryExtension
    {
        /// <summary>
        /// 尝试将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
        /// </summary>
        public static TangdaoSortedDictionary<TKey, TValue> TryAdd<TKey, TValue>(this TangdaoSortedDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key) == false) dict.Add(key, value);
            return dict;
        }
    }
}